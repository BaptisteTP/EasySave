using EasySave2._0.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EasySave2._0.ValueConverters
{
    public class SaveTypeToSelectedBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((SaveType)parameter == (SaveType)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!Enum.IsDefined(typeof(SaveType), parameter.ToString()!))
                return SaveType.Full;

            return (SaveType)Enum.Parse(typeof(SaveType), parameter.ToString()!);
        }
    }
}
