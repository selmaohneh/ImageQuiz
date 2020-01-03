using System;
using System.IO.Abstractions;
using System.Linq;
using System.Windows.Forms;

namespace ImageQuiz.WinForms
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var fbd = new FolderBrowserDialog
            {
                Description = "Select the folder that contains the images to guess. Supported image formats are jpg, jpeg, png and bmp.",
                ShowNewFolderButton = false
            };

            DialogResult dr = fbd.ShowDialog();

            if (dr != DialogResult.OK)
            {
                return;
            }

            string path = fbd.SelectedPath;

            var fileSystem = new FileSystem();
            IDirectory directory = fileSystem.Directory;
            var imageLoader = new ImageLoader();
            var imageProvider = new ImageProvider(directory, imageLoader);

            var images = imageProvider.GetImages(path).ToList();

            var view = new ImageQuizForm();
            var presenter = new ImageQuizViewPresenter(view, images);

            Application.Run((Form)presenter.View);
        }
    }
}