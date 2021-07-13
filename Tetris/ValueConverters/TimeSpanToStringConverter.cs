using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tetris
{
    namespace ValueConverters
    {
        class TimeSpanToStringConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                TimeSpan timeSpan = (TimeSpan)value;
                return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => TimeSpan.TryParse(value as string, out TimeSpan timeSpan) ? timeSpan : DependencyProperty.UnsetValue;
        }
    }
}
