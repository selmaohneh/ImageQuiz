using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace ImageQuiz.WinForms
{
    public class ImageProvider
    {
        private readonly IDirectory _directory;
        private readonly IImageLoader _imageLoader;

        public ImageProvider(IDirectory directory, IImageLoader imageLoader)
        {
            _directory = directory;
            _imageLoader = imageLoader;
        }

        public IEnumerable<Image> GetImages(string directory)
        {
            var imageFormats = new List<string>
            {
                "*.jpg",
                "*.jpeg",
                "*.png",
                "*.bmp"
            };

            var images = new List<Image>();

            foreach (string imageFormat in imageFormats)
            {
                var imagesOfCurrentFormat = GetImages(directory, imageFormat);
                images.AddRange(imagesOfCurrentFormat);
            }

            return images;
        }

        private IEnumerable<Image> GetImages(string directory, string searchPattern)
        {
            return _directory.GetFiles(directory,
                                       searchPattern,
                                       SearchOption.TopDirectoryOnly)
                             .OrderBy(f => f)
                             .Select(x => _imageLoader.Load(x));
        }
    }
}