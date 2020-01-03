using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ImageQuiz.WinForms
{
    public partial class ImageQuizForm : Form, IImageQuizView
    {
        public event EventHandler EnterPressed;

        public bool ImageWithBlocksIsVisible => !label.Visible;
        public bool AllBlocksAreGone => !_visibleBlocks.Any();

        public void EnableTimer(bool enabled)
        {
            timer.Enabled = enabled;
        }

        public ImageQuizForm()
        {
            InitializeComponent();

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            int height = e.ClipRectangle.Size.Height / 10 + 1;
            int width = e.ClipRectangle.Size.Width / 10 + 1;

            for (int col = 0; col < 10; col++)
            {
                int x = col * width;

                for (int row = 0; row < 10; row++)
                {
                    if (_visibleBlocks.Contains((col, row)))
                    {
                        int y = row * height;
                        var rect = new Rectangle(x, y, width, height);
                        e.Graphics.FillRectangle(new SolidBrush(SystemColors.ControlDark), rect);
                    }
                }
            }
        }

        public void ShowImage(Image image)
        {
            BackgroundImage = image;

            label.Visible = false;
        }

        public void RemoveBlocksFromImage()
        {
            _visibleBlocks.Clear();
            Invalidate();
        }

        public void PutBlocksOnImage()
        {
            _visibleBlocks = new List<(int x, int y)>();

            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    _visibleBlocks.Add((col, row));
                }
            }

            Invalidate();
        }

        public void ShowInfo(string info)
        {
            label.Visible = true;
            label.Text = info;
        }

        private void ImageQuizForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                EnterPressed?.Invoke(this, e);
            }
        }

        private List<(int x, int y)> _visibleBlocks;

        private static readonly Random Rnd = new Random();

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!ImageWithBlocksIsVisible || !_visibleBlocks.Any())
            {
                return;
            }

            int index = Rnd.Next(0, _visibleBlocks.Count);

            _visibleBlocks.RemoveAt(index);
            Invalidate();
        }
    }
}