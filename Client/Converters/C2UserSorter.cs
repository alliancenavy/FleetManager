using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client.Converters {
	public class C2UserSorter :IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var view = new ListCollectionView((System.Collections.IList)value);
			view.SortDescriptions.Add(new SortDescription("commander", ListSortDirection.Descending));
			view.SortDescriptions.Add(new SortDescription("name", ListSortDirection.Ascending));
			return view;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			// not really necessary could just throw notsupportedexception
			var view = (CollectionView)value;
			return view.SourceCollection;
		}
	}
}
