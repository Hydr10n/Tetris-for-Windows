using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using Tetris.Data;
using Tetris.Game.BasicDataTypes;
using Tetris.Game.Matrices;
using static Tetris.Data.ScoreboardData;

namespace Tetris.Game
{
    class Manager
    {
        public delegate void LineClearEventHandler(Manager sender, int lineCount);

        public event LineClearEventHandler LineClear;

        public const int MatrixRowCount = 20, MatrixColumnCount = 10, MaxLevel = 10;

        private const int MatrixRowBegin = 4, MatrixRowEnd = MatrixRowBegin + MatrixRowCount, MatrixColumnBegin = 2, MatrixColumnEnd = MatrixColumnBegin + MatrixColumnCount;

        private static readonly Dictionary<TetrominoShape, Dictionary<Direction, ushort>> TetrominoTable = new Dictionary<TetrominoShape, Dictionary<Direction, ushort>>
        {
            {   TetrominoShape.I, new Dictionary<Direction, ushort>
                { { Direction.Left, 0x0f00 }, { Direction.Up, 0x2222 }, { Direction.Right, 0x00f0 }, { Direction.Down, 0x2222 } }
            },
            {   TetrominoShape.T, new Dictionary<Direction, ushort>
                { { Direction.Left, 0x0262 }, { Direction.Up, 0x0720 }, { Direction.Right, 0x0232 }, { Direction.Down, 0x0270 } }
            },
            {   TetrominoShape.L, new Dictionary<Direction, ushort>
                { { Direction.Left, 0x0170 }, { Direction.Up, 0x0622 }, { Direction.Right, 0x0074 }, { Direction.Down, 0x0223 } }
            },
            {   TetrominoShape.J, new Dictionary<Direction, ushort>
                { { Direction.Left, 0x0071 }, { Direction.Up, 0x0322 }, { Direction.Right, 0x0470 }, { Direction.Down, 0x0226 } }
            },
            {   TetrominoShape.Z, new Dictionary<Direction, ushort>
                { { Direction.Left, 0x0264 }, { Direction.Up, 0x0063 }, { Direction.Right, 0x0264 }, { Direction.Down, 0x0063 } }
            },
            {   TetrominoShape.S, new Dictionary<Direction, ushort>
                { { Direction.Left, 0x0462 }, { Direction.Up, 0x006c }, { Direction.Right, 0x0462 }, { Direction.Down, 0x006c } }
            },
            {   TetrominoShape.O, new Dictionary<Direction, ushort>
                { { Direction.Left, 0x0660 }, { Direction.Up, 0x0660 }, { Direction.Right, 0x0660 }, { Direction.Down, 0x0660 } }
            }
        };

        private static readonly Dictionary<Difficulty, int[]> TimeIntervals = new Dictionary<Difficulty, int[]>
        {
            { Difficulty.Easy, new int[MaxLevel] { 480, 460, 440, 420, 400, 380, 360, 340, 320, 300 } },
            { Difficulty.Normal, new int[MaxLevel] { 380, 360, 340, 320, 300, 280, 260, 240, 220, 200 } },
            { Difficulty.Hard, new int[MaxLevel] { 280, 260, 240, 220, 200, 180, 160, 140, 120, 100 } }
        };

        private readonly Random Random = new Random();
        private readonly DispatcherTimer DispatcherTimer = new DispatcherTimer(DispatcherPriority.Render);
        private readonly MainMatrix MainMatrix;
        private readonly PreviewMatrix PreviewMatrix;
        private readonly GameViewModel GameViewModel;

        private readonly int[] matrix = new int[MatrixRowEnd + 3];
        private readonly TetrominoShape[] tetrominoShapes = new TetrominoShape[2];
        private readonly Direction[] directions = new Direction[2];
        private readonly Mino[,] minos = new Mino[MatrixRowCount, MatrixColumnCount];

        private struct Cell { public int Row, Column; }
        private Cell cell;

        public Manager(MainMatrix mainMatrix, PreviewMatrix previewMatrix, GameViewModel gameViewModel)
        {
            MainMatrix = mainMatrix;
            PreviewMatrix = previewMatrix;
            GameViewModel = gameViewModel;
            DispatcherTimer.Tick += (sender, e) =>
            {
                GameViewModel.Time += DispatcherTimer.Interval;
                MoveTetromino(Direction.Down, true);
            };
        }

        private void SaveRecord()
        {
            if (GameViewModel.Score > 0)
                try
                {
                    XElement xElement = new XElement(nameof(Record), new object[]
                    {
                        new XElement(nameof(Record.Score), GameViewModel.Score),
                        new XElement(nameof(Record.LineCount), GameViewModel.LineCount),
                        new XElement(nameof(Record.Level), GameViewModel.Level + 1),
                        new XElement(nameof(Record.Time), new ValueConverters.TimeSpanToStringConverter().Convert(GameViewModel.Time, null, null, null)),
                        new XElement(nameof(Record.DateCreated), DateTime.Now.ToString())
                    });
                    xElement.SetAttributeValue(nameof(Record.Difficulty), GameViewModel.Difficulty);
                    MyAppData.Save(null, xElement);
                }
                catch { }
        }

        private void UpdatePreviewMatrix()
        {
            PreviewMatrix.Children.Clear();
            int tetromino = TetrominoTable[tetrominoShapes[1]][directions[1]];
            for (int i = 0; i < 16; i++)
                if ((tetromino >> i & 1) != 0)
                {
                    Mino mino = new Mino(tetrominoShapes[1], directions[1]);
                    PreviewMatrix.Children.Add(mino);
                    PreviewMatrix.SetRow(mino, i >> 2);
                    PreviewMatrix.SetColumn(mino, i & 3);
                }
        }

        private void AddTetrominoToMainMatrix()
        {
            int tetromino = TetrominoTable[tetrominoShapes[0]][directions[0]];
            for (int i = 0; i < 16; i++)
            {
                int row = (i >> 2) + cell.Row;
                if (row >= MatrixRowEnd)
                    break;
                int column = (i & 3) + cell.Column;
                if (row >= MatrixRowBegin && column >= MatrixColumnBegin && column < MatrixColumnEnd && ((tetromino >> i) & 1) != 0)
                {
                    row -= MatrixRowBegin;
                    column -= MatrixColumnBegin;
                    ref Mino mino = ref minos[row, column];
                    if (mino != null)
                    {
                        mino.TetrominoShape = tetrominoShapes[0];
                        mino.Direction = directions[0];
                    }
                    else
                    {
                        mino = new Mino(tetrominoShapes[0], directions[0]) { Visibility = Visibility.Collapsed };
                        MainMatrix.Children.Add(mino);
                    }
                    MainMatrix.SetRow(mino, row);
                    MainMatrix.SetColumn(mino, column);
                }
            }
        }

        private void UpdateMainMatrix()
        {
            for (int row = 0; row < MatrixRowCount; row++)
                for (int column = 0; column < MatrixColumnCount; column++)
                {
                    ref Mino mino = ref minos[row, column];
                    if (((matrix[MatrixRowBegin + row] >> (MatrixColumnBegin + column)) & 1) != 0)
                    {
                        MainMatrix.SetRow(mino, row);
                        MainMatrix.SetColumn(mino, column);
                        mino.Visibility = Visibility.Visible;
                    }
                    else if (mino != null)
                    {
                        MainMatrix.Children.Remove(mino);
                        mino = null;
                    }
                }
        }

        private void InsertTetromino()
        {
            int tetromino = TetrominoTable[tetrominoShapes[0]][directions[0]];
            for (int i = 0; i < 4; i++)
                matrix[cell.Row + i] |= ((tetromino >> (i << 2)) & 0xf) << cell.Column;
        }

        private void RemoveTetromino()
        {
            int tetromino = TetrominoTable[tetrominoShapes[0]][directions[0]];
            for (int i = 0; i < 4; i++)
                matrix[cell.Row + i] &= ~(((tetromino >> (i << 2)) & 0xf) << cell.Column);
        }

        private bool CheckCollision()
        {
            int tetromino = TetrominoTable[tetrominoShapes[0]][directions[0]];
            for (int i = 0; i < 4; i++)
                if ((matrix[cell.Row + i] >> cell.Column << (i << 2) & 0xf << (i << 2) & tetromino) != 0)
                    return true;
            return false;
        }

        private void GenerateNextTetromino()
        {
            tetrominoShapes[1] = (TetrominoShape)Random.Next(Enum.GetValues(typeof(TetrominoShape)).Length);
            directions[1] = (Direction)Random.Next(Enum.GetValues(typeof(Direction)).Length);
        }

        private void GenerateTetrominos()
        {
            tetrominoShapes[0] = tetrominoShapes[1];
            directions[0] = directions[1];
            int tetromino = TetrominoTable[tetrominoShapes[0]][directions[0]];
            cell.Row = (tetromino & 0xf000) != 0 ? 0 : (tetromino & 0xff00) != 0 ? 1 : 2;
            cell.Column = MatrixColumnBegin + MatrixColumnCount / 2 - 2;
            if (CheckCollision())
                StopGame();
            else
            {
                InsertTetromino();
                AddTetrominoToMainMatrix();
                UpdateMainMatrix();
                GenerateNextTetromino();
                UpdatePreviewMatrix();
            }
        }

        private int ClearLines()
        {
            int[] scores = { 10, 20, 40, 60 };
            int count = 0, i = 0, row = cell.Row + 3;
            do
                if (row < MatrixRowEnd && (matrix[row] | 0x30 << MatrixColumnCount) == 0xffff)
                {
                    count++;
                    Array.Copy(matrix, 0, matrix, 1, row);
                    for (int column = 0; column < MatrixColumnCount; column++)
                    {
                        Mino mino = minos[row - MatrixRowBegin, column];
                        if (mino != null)
                            MainMatrix.Children.Remove(mino);
                    }
                    Array.Copy(minos, 0, minos, MatrixColumnCount, (row - MatrixRowBegin) * MatrixColumnCount);
                }
                else
                {
                    row--;
                    i++;
                }
            while (row >= cell.Row && i < 4);
            if (count > 0)
            {
                GameViewModel.Score += scores[count - 1];
                GameViewModel.LineCount += count;
                LineClear?.Invoke(this, count);
            }
            return count;
        }

        private void MoveTetromino(Direction direction, bool auto)
        {
            if (GameViewModel.State != State.Running)
                return;
            int row = cell.Row, column = cell.Column;
            RemoveTetromino();
            switch (direction)
            {
                case Direction.Down: cell.Row++; break;
                case Direction.Left:
                case Direction.Right: cell.Column += direction == Direction.Left ? -1 : 1; break;
                default: return;
            }
            if (CheckCollision())
            {
                cell.Row = row;
                cell.Column = column;
                InsertTetromino();
                if (direction == Direction.Down)
                {
                    int count = ClearLines();
                    GenerateTetrominos();
                    if (count > 0)
                        UpdateMainMatrix();
                }
            }
            else
            {
                if (direction == Direction.Down && !auto)
                    GameViewModel.Score++;
                InsertTetromino();
                AddTetrominoToMainMatrix();
                UpdateMainMatrix();
            }
        }

        public void MoveTetromino(Direction direction) => MoveTetromino(direction, false);

        public void DropTetromino()
        {
            if (GameViewModel.State != State.Running)
                return;
            RemoveTetromino();
            int row = cell.Row;
            while (cell.Row++ < MatrixRowEnd && !CheckCollision())
                ;
            cell.Row--;
            GameViewModel.Score += (cell.Row - row) << 1;
            InsertTetromino();
            AddTetrominoToMainMatrix();
            ClearLines();
            GenerateTetrominos();
            UpdateMainMatrix();
        }

        public void RotateTetromino(bool clockwise)
        {
            Direction direction = directions[0];
            RemoveTetromino();
            directions[0] = (Direction)(((int)direction + (clockwise ? 1 : 3)) & 3);
            if (CheckCollision())
            {
                directions[0] = direction;
                InsertTetromino();
            }
            else
            {
                InsertTetromino();
                AddTetrominoToMainMatrix();
                UpdateMainMatrix();
            }
        }

        public void StartGame(bool restart)
        {
            if (restart)
            {
                MainMatrix.Children.Clear();
                PreviewMatrix.Children.Clear();
                Array.Clear(minos, 0, minos.Length);
                for (int i = 0; i < matrix.Length; i++)
                    matrix[i] = i < MatrixRowEnd ? 0xc << MatrixColumnCount | 0x3 : 0xffff;
                GenerateNextTetromino();
                GenerateTetrominos();
                GameViewModel.Score = GameViewModel.LineCount = 0;
                GameViewModel.Time = TimeSpan.Zero;
                DispatcherTimer.Interval = TimeSpan.FromMilliseconds(TimeIntervals[GameViewModel.Difficulty][GameViewModel.Level]);
            }
            else if (GameViewModel.State != State.Paused)
                return;
            GameViewModel.State = State.Running;
            DispatcherTimer.Start();
        }

        public void PauseGame()
        {
            if (GameViewModel.State == State.Running)
            {
                GameViewModel.State = State.Paused;
                DispatcherTimer.Stop();
            }
        }

        public void StopGame()
        {
            if (GameViewModel.State != State.NotStarted && GameViewModel.State != State.Over)
            {
                GameViewModel.State = State.Over;
                DispatcherTimer.Stop();
                SaveRecord();
            }
        }
    }
}