using System.Windows.Controls;

namespace Tetris.Game.Matrices
{
    class Matrix : Grid
    {
        public Matrix(int rowCount, int columnCount)
        {
            for (int i = 0; i < rowCount; i++)
                RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < columnCount; i++)
                ColumnDefinitions.Add(new ColumnDefinition());
        }
    }

    class MainMatrix : Matrix { public MainMatrix() : base(Manager.MatrixRowCount, Manager.MatrixColumnCount) { } }

    class PreviewMatrix : Matrix { public PreviewMatrix() : base(4, 4) { } }
}
