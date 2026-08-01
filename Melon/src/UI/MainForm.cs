using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace MelonLoaderDownloader
{
    public partial class MainForm : Form
    {
        private readonly MelonInstaller _installer = new();

        public MainForm()
        {
            InitializeComponent();
            LoadGames();
        }

        private async void LoadGames()
        {
            lstGames.Items.Clear();
            foreach (var (name, path, fw) in GameDetector.FindInstalledGames())
                lstGames.Items.Add(new ListViewItem(new[] { name, fw, path }));
            lblStatus.Text = $"Found {lstGames.Items.Count} supported game(s).";
        }

        private async void btnInstall_Click(object sender, EventArgs e)
        {
            if (lstGames.SelectedItems.Count == 0) { MessageBox.Show("Select a game first."); return; }
            var gamePath = lstGames.SelectedItems[0].SubItems[2].Text;

            btnInstall.Enabled = false;
            var progress = new Progress<(string msg, int pct)>(p =>
            {
                lblStatus.Text    = p.msg;
                progressBar.Value = p.pct;
            });

            try
            {
                var (version, url) = await _installer.GetLatestReleaseAsync();
                lblStatus.Text = $"Installing MelonLoader {version}…";
                await _installer.InstallAsync(gamePath, url, progress);
                MessageBox.Show($"MelonLoader {version} installed to {gamePath}", "Done");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Install failed: {ex.Message}", "Error");
            }
            finally
            {
                btnInstall.Enabled = true;
            }
        }
    }
}