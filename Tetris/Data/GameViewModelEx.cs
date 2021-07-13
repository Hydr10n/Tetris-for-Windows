using System.Collections;
using System.Linq;
using Tetris.Game;
using Tetris.Game.BasicDataTypes;

namespace Tetris.Data
{
    class GameViewModelEx : GameViewModel
    {
        public IEnumerable Difficulties { get => new string[] { "Easy", "Normal", "Hard" }; }

        public IEnumerable Levels { get => Enumerable.Range(1, Manager.MaxLevel).Select(x => x.ToString()); }

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

        public bool IsDifficultyChangable { get => State == State.NotStarted || State == State.Over; }

        public bool IsStateTextVisible { get => State == State.Over; }

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
