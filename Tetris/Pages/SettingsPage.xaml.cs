using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Tetris.Pages
{
    public partial class SettingsPage : Page
    {
        public SettingsPage() => InitializeComponent();

        private void GitHubRepositoryHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
