using System.Windows;
using System.Windows.Input;
using ModernWpf.Controls;
using Tetris.Game.BasicDataTypes;
using Tetris.Pages;
using Page = System.Windows.Controls.Page;

namespace Tetris
{
    public partial class MainWindow : Window
    {
        private Page settingsPage, gamePage;

        public MainWindow()
        {
            InitializeComponent();
            NavigationView.SelectedItem = NavigationView.MenuItems[0];
        }

        private void NavigationView_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) => e.NewFocus.Focusable = false;

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
                ContentFrame.Navigate(settingsPage = settingsPage ?? new SettingsPage());
            else
            {
                Page page = null;
                switch ((args.SelectedItem as FrameworkElement).Tag)
                {
                    case "Home": page = new HomePage(); break;
                    case "Game": page = gamePage = gamePage ?? new GamePage(); break;
                    case "ScoreboardEasyDifficulty": page = new ScoreboardPage(Difficulty.Easy); break;
                    case "ScoreboardNormalDifficulty": page = new ScoreboardPage(Difficulty.Normal); break;
                    case "ScoreboardHardDifficulty": page = new ScoreboardPage(Difficulty.Hard); break;
                }
                ContentFrame.Navigate(page);
            }
        }
    }
}
