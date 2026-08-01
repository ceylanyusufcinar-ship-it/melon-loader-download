// MelonLoader Downloader — main entry point
// melonloader download, melonloader installer — Windows Forms UI
using System;
using System.Windows.Forms;

namespace MelonLoaderDownloader
{
    internal static class MelonLoaderDownloaderProgram
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
