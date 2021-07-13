using Hydr10n.Data;
using System;
using Tetris.Game.BasicDataTypes;

namespace Tetris.Data
{
    class GameViewModel : ViewModel
    {
        private int score;
        public int Score
        {
            get => score;
            set
            {
                score = value;
                OnPropertyChanged();
            }
        }

        private int lineCount;
        public int LineCount
        {
            get => lineCount;
            set
            {
                lineCount = value;
                OnPropertyChanged();
            }
        }

        private int level;
        public int Level
        {
            get => level;
            set
            {
                level = value;
                OnPropertyChanged();
            }
        }

        private Difficulty difficulty = Difficulty.Normal;
        public Difficulty Difficulty
        {
            get => difficulty;
            set
            {
                difficulty = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan time;
        public TimeSpan Time
        {
            get => time;
            set
            {
                time = value;
                OnPropertyChanged();
            }
        }

        private State state;
        public virtual State State
        {
            get => state;
            set
            {
                state = value;
                OnPropertyChanged();
            }
        }
    }
}
