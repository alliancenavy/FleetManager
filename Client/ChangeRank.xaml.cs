using System;
using System.Windows;
using System.Collections.ObjectModel;
using ANWI;
using System.Linq;

namespace Client {
	/// <summary>
	/// Interaction logic for ChangeRank.xaml
	/// </summary>
	public partial class ChangeRank : Window {

		private ObservableCollection<Rank> _rankList = null;
		public ObservableCollection<Rank> rankList { get { return _rankList; } }

		/// <summary>
		/// Subscribed to by the caller to receive the new rank
		/// </summary>
		public event Action<int> ReturnNewRank;

		public ChangeRank(int max) {
			this.DataContext = this;
			_rankList = new ObservableCollection<Rank>(
				CommonData.ranks.Where(r => r.ordering <= max));

			InitializeComponent();
		}

		/// <summary>
		/// Returns the newly selected rank to the caller
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_OK_Click(object sender, RoutedEventArgs e) {
			if(ReturnNewRank != null && ComboBox_Rank.SelectedItem != null) {
				ReturnNewRank((ComboBox_Rank.SelectedItem as Rank).id);
			}
			this.Close();
		}

		/// <summary>
		/// Closes the window with no changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}
	}
}
