using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Client.Converters {
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class InverseBoolToVisibility : IValueConverter {
		public object Convert(object value, System.Type targetType,
			object parameter, CultureInfo culture) {

			bool b = (bool)value;
			if (b)
				return Visibility.Hidden;
			else
				return Visibility.Visible;
		}

		public object ConvertBack(object value, System.Type targetType,
			object parameter, CultureInfo culture) {

			throw new NotSupportedException();
		}
	}
}
