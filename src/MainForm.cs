// Main Windows Forms window for MelonLoader Downloader
// melonloader installer UI — game selection, progress bar, install button
using System;
using System.Windows.Forms;

namespace MelonLoaderDownloader
{
    public partial class MainForm : Form
    {
        private readonly GameDetector _detector = new();
        private readonly Installer _installer = new();

        public MainForm()
        {
            InitializeComponent();
            LoadGames();
        }

        private void LoadGames()
        {
            var games = _detector.Detect();
            gameListBox.DataSource = games;
            gameListBox.DisplayMember = "Name";
        }

        private async void btnInstall_Click(object sender, EventArgs e)
        {
            if (gameListBox.SelectedItem is not DetectedGame game) return;

            btnInstall.Enabled = false;
            progressBar.Value = 0;

            var progress = new Progress<int>(p => progressBar.Value = p);
            try
            {
                await _installer.InstallAsync(
                    System.IO.Path.GetDirectoryName(game.ExePath)!, progress);
                MessageBox.Show($"MelonLoader installed for {game.Name}!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Install failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnInstall.Enabled = true;
            }
        }

        private ListBox gameListBox = new() { Dock = DockStyle.Fill };
        private Button btnInstall = new() { Text = "Install MelonLoader", Dock = DockStyle.Bottom };
        private ProgressBar progressBar = new() { Dock = DockStyle.Bottom, Minimum = 0, Maximum = 100 };

        private void InitializeComponent()
        {
            Text = "MelonLoader Downloader";
            Size = new System.Drawing.Size(500, 400);
            Controls.Add(gameListBox);
            Controls.Add(progressBar);
            Controls.Add(btnInstall);
            btnInstall.Click += btnInstall_Click;
        }
    }
}
