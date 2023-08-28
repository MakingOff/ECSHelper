using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ECSHelper.Converters; 

public class ServerRunningToTextColorConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is bool val) {
            return val ? Brushes.Green : Brushes.Black;
        } else {
            return Brushes.Black;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}