using System;
using System.Collections.Generic;
using System.Drawing;
using ImageQuiz.WinForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ImageQuiz.Tests
{
    [TestClass]
    public class ImageQuizViewPresenterTests
    {
        private Mock<IImageQuizView> _view;
        private List<Image> _images;
        private ImageQuizViewPresenter _presenter;

        [TestInitialize]
        public void Init()
        {
            _view = new Mock<IImageQuizView>();

            _images = new List<Image>
            {
                new Bitmap(1, 1),
                new Bitmap(2, 2),
                new Bitmap(3, 3)
            };

            _presenter = new ImageQuizViewPresenter(_view.Object, _images);
        }

        [TestMethod]
        public void ThreeImages_ShowInitialInfoLabel()
        {
            _view.Verify(v => v.ShowInfo(It.Is<string>(s => s.Contains("3 images were found."))), Times.Once);
        }

        [TestMethod]
        public void NoImages_ShowErrorLabel()
        {
            _presenter = new ImageQuizViewPresenter(_view.Object, new List<Image>());
            _view.Verify(v => v.ShowInfo(It.Is<string>(s => s.Contains("No images were found."))), Times.Once);
        }

        [TestMethod]
        public void InfoVisible_PressEnter_ImagesLeft_ImageGetsShown_BlocksGetAdded_TimerGetsStarted()
        {
            _view.Setup(v => v.ImageWithBlocksIsVisible).Returns(false);

            _view.Raise(v => v.EnterPressed += null, this, EventArgs.Empty);

            _view.Verify(v => v.ShowImage(It.IsAny<Image>()), Times.Once);
            _view.Verify(v => v.PutBlocksOnImage());
            _view.Verify(v => v.EnableTimer(true), Times.Once);
        }

        [TestMethod]
        public void ImageVisible_BlocksGone_PressEnter_ImagesLeft_InfoGetsShown_TimerGetsStopped()
        {
            _view.Setup(v => v.AllBlocksAreGone).Returns(true);

            // should show first image
            _view.Setup(v => v.ImageWithBlocksIsVisible).Returns(false);
            _view.Raise(v => v.EnterPressed += null, this, EventArgs.Empty);

            // should show info
            _view.Setup(v => v.ImageWithBlocksIsVisible).Returns(true);
            _view.Raise(v => v.EnterPressed += null, this, EventArgs.Empty);

            _view.Verify(v => v.ShowInfo(It.Is<string>(s => s.Contains("2 of 3"))), Times.Once);
            _view.Verify(v => v.EnableTimer(false), Times.Once);
        }

        [TestMethod]
        public void ImageVisible_BlocksStillShown_PressEnter_FullImageWithoutBlocksGetsShown_TimerGetsStopped()
        {
            // should show first image
            _view.Setup(v => v.AllBlocksAreGone).Returns(true);
            _view.Setup(v => v.ImageWithBlocksIsVisible).Returns(false);
            _view.Raise(v => v.EnterPressed += null, this, EventArgs.Empty);

            // should show full image without blocks
            _view.Setup(v => v.AllBlocksAreGone).Returns(false);
            _view.Setup(v => v.ImageWithBlocksIsVisible).Returns(true);
            _view.Raise(v => v.EnterPressed += null, this, EventArgs.Empty);

            _view.Verify(v => v.EnableTimer(false), Times.Once);
            _view.Verify(v => v.RemoveBlocksFromImage());
        }

        [TestMethod]
        public void ImageVisible_BlocksGone_PressEnter_NoImagesLeft_GameOverInfoGetsShown_TimerGetsStopped()
        {
            var view = new Mock<IImageQuizView>();

            view.Setup(v => v.AllBlocksAreGone).Returns(true);

            _presenter = new ImageQuizViewPresenter(view.Object, new List<Image>
            {
                new Bitmap(1, 1)
            });

            // should show last image
            view.Setup(v => v.ImageWithBlocksIsVisible).Returns(false);
            view.Raise(v => v.EnterPressed += null, this, EventArgs.Empty);

            view.Invocations.Clear();

            // should show game over info
            view.Setup(v => v.ImageWithBlocksIsVisible).Returns(true);
            view.Raise(v => v.EnterPressed += null, this, EventArgs.Empty);

            view.Verify(v => v.ShowInfo(It.Is<string>(s => s.Contains("Game over"))), Times.Once);
            view.Verify(v => v.EnableTimer(false), Times.Once);
        }

        [TestMethod]
        public void GameOver_PressEnter_DontShowAnythingOtherThanTheGameOverInfo()
        {
            _presenter = new ImageQuizViewPresenter(_view.Object, new List<Image>
            {
                new Bitmap(1, 1)
            });

            // should show last image
            _view.Setup(v => v.ImageWithBlocksIsVisible).Returns(false);
            _view.Raise(v => v.EnterPressed += null, this, EventArgs.Empty);

            // should show game over info
            _view.Setup(v => v.ImageWithBlocksIsVisible).Returns(true);
            _view.Raise(v => v.EnterPressed += null, this, EventArgs.Empty);

            _view.Invocations.Clear();
            _view.Setup(v => v.ImageWithBlocksIsVisible).Returns(false);

            for (int i = 0; i <= 42; i++)
            {
                _view.Raise(v => v.EnterPressed += null, this, EventArgs.Empty);
            }

            _view.Verify(v => v.ShowInfo(It.Is<string>(s => !s.Contains("Game over"))), Times.Never);
        }

        [TestMethod]
        public void AllThreeImagesGetShown()
        {
            _view.Setup(v => v.ImageWithBlocksIsVisible).Returns(false);

            _view.Raise(v => v.EnterPressed += null, this, EventArgs.Empty);
            _view.Raise(v => v.EnterPressed += null, this, EventArgs.Empty);
            _view.Raise(v => v.EnterPressed += null, this, EventArgs.Empty);

            _view.Verify(v => v.ShowImage(It.Is<Image>(i => i.Size.Equals(new Size(1, 1)))), Times.Once);
            _view.Verify(v => v.ShowImage(It.Is<Image>(i => i.Size.Equals(new Size(2, 2)))), Times.Once);
            _view.Verify(v => v.ShowImage(It.Is<Image>(i => i.Size.Equals(new Size(3, 3)))), Times.Once);
        }
    }
}