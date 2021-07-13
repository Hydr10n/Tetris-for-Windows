using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Game.BasicDataTypes;

namespace Tetris.Game
{
    enum TetrominoShape { I, T, L, J, Z, S, O }

    class Mino : Border
    {
        private static readonly Dictionary<TetrominoShape, Color[]> TetriminoColors = new Dictionary<TetrominoShape, Color[]>
        {
            { TetrominoShape.I, new Color[] { Color.FromRgb(38, 169, 170), Color.FromRgb(50, 203, 204) } },
            { TetrominoShape.T, new Color[] { Color.FromRgb(146, 49, 146), Color.FromRgb(164, 78, 156) } },
            { TetrominoShape.L, new Color[] { Color.FromRgb(253, 143, 14), Color.FromRgb(255, 172, 76) } },
            { TetrominoShape.J, new Color[] { Color.FromRgb(8, 123, 212), Color.FromRgb(0, 162, 232) } },
            { TetrominoShape.Z, new Color[] { Color.FromRgb(200, 25, 93), Color.FromRgb(235, 60, 115) } },
            { TetrominoShape.S, new Color[] { Color.FromRgb(57, 140, 43), Color.FromRgb(88, 191, 65) } },
            { TetrominoShape.O, new Color[] { Color.FromRgb(210, 166, 37), Color.FromRgb(255, 232, 48) } }
        };

        public Mino(TetrominoShape tetrominoShape, Direction direction)
        {
            RenderTransformOrigin = new Point(0.5, 0.5);
            Margin = new Thickness(0.5);
            CornerRadius = new CornerRadius(3);
            TetrominoShape = tetrominoShape;
            Direction = direction;
        }

        private TetrominoShape tetrominoShape;
        public TetrominoShape TetrominoShape
        {
            get => tetrominoShape;
            set
            {
                tetrominoShape = value;
                var tetriminoColor = TetriminoColors[value];
                Background = new LinearGradientBrush(tetriminoColor[0], tetriminoColor[1], 45);
            }
        }

        private Direction direction;
        public Direction Direction
        {
            get => direction;
            set
            {
                direction = value;
                RenderTransform = new RotateTransform(((int)value - (int)Direction.Left) * 90);
            }
        }
    }
}
