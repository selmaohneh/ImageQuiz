using System.Drawing;

namespace ImageQuiz.WinForms
{
    public interface IImageLoader
    {
        Image Load(string path);
    }
}