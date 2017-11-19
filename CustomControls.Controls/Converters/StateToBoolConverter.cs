using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CustomControls.Controls.Converters
{
    public class StateToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var checkState = (WindowState) value;
            return checkState == WindowState.Maximized;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var windowState = (bool) value;
            return windowState ? WindowState.Maximized : WindowState.Normal;

        }
    }
}
