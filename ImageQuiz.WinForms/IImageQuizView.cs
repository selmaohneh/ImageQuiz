using System;
using System.Drawing;

namespace ImageQuiz.WinForms
{
    public interface IImageQuizView
    {
        event EventHandler EnterPressed;
        bool ImageWithBlocksIsVisible { get; }
        bool AllBlocksAreGone { get; }
        void ShowImage(Image image);
        void ShowInfo(string info);
        void EnableTimer(bool enabled);
        void RemoveBlocksFromImage();
        void PutBlocksOnImage();
    }
}