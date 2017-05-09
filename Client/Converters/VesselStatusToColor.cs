using System;
using System.Globalization;
using System.Windows.Data;
using ANWI;
using System.Windows.Media;

namespace Client.Converters {
	[ValueConversion(typeof(VesselStatus), typeof(SolidColorBrush))]
	class VesselStatusToColor : IValueConverter {
		public object Convert(object value, System.Type targetType,
			object parameter, CultureInfo culture) {

			VesselStatus status = (VesselStatus)value;
			switch(status) {
				case VesselStatus.ACTIVE:
					return (SolidColorBrush)
						(new BrushConverter().ConvertFrom("#FF8CF699"));
				case VesselStatus.DESTROYED:
					return (SolidColorBrush)
						(new BrushConverter().ConvertFrom("#FFFFADAD"));
				case VesselStatus.DESTROYED_WAITING_REPLACEMENT:
					return (SolidColorBrush)
						(new BrushConverter().ConvertFrom("#FFFFADAD"));
				case VesselStatus.DRYDOCKED:
					return (SolidColorBrush)
						(new BrushConverter().ConvertFrom("#FFFFD5AD"));
				case VesselStatus.DECOMMISSIONED:
					return new SolidColorBrush(Colors.White);
				default:
					return new SolidColorBrush(Colors.White);
			}
		}

		public object ConvertBack(object value, System.Type targetType,
			object parameter, CultureInfo culture) {

			throw new NotSupportedException();
		}
	}
}
