using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageQuiz.WinForms
{
    public class ImageQuizViewPresenter
    {
        private readonly List<Image> _allImages;
        private readonly Stack<Image> _remainingImages;
        public IImageQuizView View { get; }

        public ImageQuizViewPresenter(IImageQuizView view, IEnumerable<Image> images)
        {
            _allImages = images.ToList();
            _remainingImages = new Stack<Image>(_allImages);

            View = view;

            if (!_remainingImages.Any())
            {
                View.ShowInfo("No images were found. Please try again with a different directory.");

                return;
            }

            View.EnterPressed += ViewOnEnterPressed;

            View.ShowInfo($"{_allImages.Count} images were found. Press [ENTER] to show the first one!");
        }

        private void ViewOnEnterPressed(object sender, EventArgs e)
        {
            if (View.ImageWithBlocksIsVisible && !View.AllBlocksAreGone)
            {
                View.EnableTimer(false);
                View.RemoveBlocksFromImage();

                return;
            }

            if (!_remainingImages.Any())
            {
                View.EnableTimer(false);
                View.ShowInfo($"Game over. All {_allImages.Count} images were presented.");

                return;
            }

            if (View.ImageWithBlocksIsVisible && View.AllBlocksAreGone)
            {
                View.EnableTimer(false);
                View.ShowInfo($"Press [ENTER] to show image {_allImages.Count - _remainingImages.Count + 1} of {_allImages.Count}!");

                return;
            }

            Image nextImage = _remainingImages.Pop();

            View.PutBlocksOnImage();
            View.ShowImage(nextImage);
            View.EnableTimer(true);
        }
    }
}