using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Client {
	/// <summary>
	/// A simple dropdown selection modal window
	/// </summary>
	public partial class SimpleDropdownSelect : Window {

		// List of items that will appear in the combobox
		private ObservableCollection<object> _itemList = null;
		public ObservableCollection<object> itemList {
			get { return _itemList; }
		}

		// Subscribe to receive the selected item's index
		public event Action<int> ReturnSelected;

		public SimpleDropdownSelect(List<object> l) {
			_itemList = new ObservableCollection<object>(l);
			this.DataContext = this;
			InitializeComponent();
		}

		public SimpleDropdownSelect(List<string> l) {
			_itemList = new ObservableCollection<object>(l);
			this.DataContext = this;
			InitializeComponent();
		}

		/// <summary>
		/// Returns the selected index and closes the window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_OK_Click(object sender, RoutedEventArgs e) {
			if (Combo_ItemList.SelectedIndex != -1) {
				if (ReturnSelected != null)
					ReturnSelected(Combo_ItemList.SelectedIndex);
				this.Close();
			}
		}
		
		/// <summary>
		/// Closes the window without returning anything
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}
	}
}
