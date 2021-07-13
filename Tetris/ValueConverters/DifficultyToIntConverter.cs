using System;
using System.Globalization;
using System.Windows.Data;
using Tetris.Game.BasicDataTypes;

namespace Tetris.ValueConverters
{
    class DifficultyToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (Difficulty)value;
    }
}
