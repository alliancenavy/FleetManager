using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Client.Converters {
	[ValueConversion(typeof(bool), typeof(bool))]
	public class InverseBool : IValueConverter {
		public object Convert(object value, System.Type targetType,
			object parameter, CultureInfo culture) {

			bool b = (bool)value;
			return !b;
		}

		public object ConvertBack(object value, System.Type targetType,
			object parameter, CultureInfo culture) {

			throw new NotSupportedException();
		}
	}
}
