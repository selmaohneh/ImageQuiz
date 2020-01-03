using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using ImageQuiz.WinForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ImageQuiz.Tests
{
    [TestClass]
    public class ImageProviderTests
    {
        [TestMethod]
        public void VerifyDirectoryGetsSearchedForAllImageFormats_VerifyFoundImagesGetLoaded()
        {
            var directory = new Mock<IDirectory>();

            directory.Setup(d => d.GetFiles("Homer Simpson", It.IsAny<string>(), It.IsAny<SearchOption>()))
                     .Returns<string, string, SearchOption>((path, searchPattern, searchOption) => new[]
                      {
                          searchPattern
                      });

            var imageLoader = new Mock<IImageLoader>();
            imageLoader.Setup(i => i.Load(It.IsAny<string>())).Returns(new Bitmap(1, 1));

            var imageProvider = new ImageProvider(directory.Object, imageLoader.Object);

            var images = imageProvider.GetImages("Homer Simpson");

            var imageFormats = new List<string>
            {
                "*.jpg",
                "*.jpeg",
                "*.png",
                "*.bmp"
            };

            foreach (string imageFormat in imageFormats)
            {
                directory.Verify(x => x.GetFiles("Homer Simpson", imageFormat, SearchOption.TopDirectoryOnly), Times.Once);
                imageLoader.Verify(i => i.Load(imageFormat), Times.Once);
            }

            Assert.AreEqual(4, images.Count());
        }
    }
}