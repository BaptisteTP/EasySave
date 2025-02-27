using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Remote_app_easysave.ValueConverters
{
    public class BooleanAndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is bool isExecuting && values[1] is bool isPaused)
            {
                return isExecuting && !isPaused ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
