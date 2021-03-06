﻿using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace SmartCardExplorer.Windows.WPF
{
	public class EnumerateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return new List<object> { value };
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
