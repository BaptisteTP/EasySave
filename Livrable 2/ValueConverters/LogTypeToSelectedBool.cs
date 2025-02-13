using EasySave2._0.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace EasySave2._0.ValueConverters
{
	public class LogTypeToSelectedBool : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((LogType)parameter == (LogType)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!Enum.IsDefined(typeof(LogType), parameter.ToString()!))
				return LogType.JSON;

			return (LogType)Enum.Parse(typeof(LogType), parameter.ToString()!);
		}
	}
}
