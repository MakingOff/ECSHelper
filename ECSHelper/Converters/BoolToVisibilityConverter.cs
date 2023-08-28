using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ECSHelper.Converters; 

public class BoolToVisibilityConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is bool val) {
            return val ? Visibility.Visible : Visibility.Collapsed;
        } else {
            return Visibility.Collapsed;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}