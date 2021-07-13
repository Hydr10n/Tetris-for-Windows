using System.Windows.Controls;
using System.Windows.Input;
using Tetris.Data;
using Tetris.Game.BasicDataTypes;

namespace Tetris.Pages
{
    public partial class ScoreboardPage : Page
    {
        public ScoreboardData ScoreboardData { get; private set; }

        public ScoreboardPage(Difficulty difficulty)
        {
            ScoreboardData = new ScoreboardData(difficulty);
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
