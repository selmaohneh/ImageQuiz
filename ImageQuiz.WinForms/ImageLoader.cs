using System.Drawing;

namespace ImageQuiz.WinForms
{
    public class ImageLoader : IImageLoader
    {
        public Image Load(string path)
        {
            return Image.FromFile(path);
        }
    }
}
