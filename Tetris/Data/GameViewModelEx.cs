using System.Collections;
using System.Linq;
using Tetris.Game;
using Tetris.Game.BasicDataTypes;

namespace Tetris.Data
{
    class GameViewModelEx : GameViewModel
    {
        public static IEnumerable Difficulties => new string[] { "EASY", "NORMAL", "HARD" };

        public static IEnumerable Levels => Enumerable.Range(1, Manager.MaxLevel).Select(x => x.ToString());

        private bool isGamepadActive;
        public bool IsGamepadActive
        {
            get => isGamepadActive;
            set
            {
                isGamepadActive = value;
                OnPropertyChanged();
            }
        }

        public bool IsDifficultyChangable => State == State.NotStarted || State == State.Over;

        public bool IsStateTextVisible => State == State.Over;

        public override State State
        {
            get => base.State;
            set
            {
                base.State = value;
                OnPropertyChanged(nameof(IsDifficultyChangable));
                OnPropertyChanged(nameof(IsStateTextVisible));
            }
        }
    }
}
