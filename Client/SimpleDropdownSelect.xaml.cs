using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client {
	/// <summary>
	/// Interaction logic for SimpleDropdownSelect.xaml
	/// </summary>
	public partial class SimpleDropdownSelect : Window {

		private ObservableCollection<object> itemList = null;
		public ObservableCollection<object> wpfItemList { get { return itemList; } }

		public event Action<int> returnSelected;

		public SimpleDropdownSelect(List<object> l) {
			itemList = new ObservableCollection<object>(l);
			this.DataContext = this;
			InitializeComponent();
		}

		public SimpleDropdownSelect(List<string> l) {
			itemList = new ObservableCollection<object>(l);
			this.DataContext = this;
			InitializeComponent();
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e) {
			if (Combo_ItemList.SelectedIndex != -1) {
				if (returnSelected != null)
					returnSelected(Combo_ItemList.SelectedIndex);
				this.Close();
			}
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}
	}
}
