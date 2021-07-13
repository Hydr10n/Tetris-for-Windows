using Hydr10n.Data;
using System.Reflection;

namespace Tetris.Data
{
    class SettingsViewModel : ViewModel
    {
        public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
