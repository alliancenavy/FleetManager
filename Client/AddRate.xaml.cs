using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ANWI;
using System.ComponentModel;

namespace Client {

	/// <summary>
	/// Window used to add rates to a user
	/// </summary>
	public partial class AddRate : Window, INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;

		private ObservableCollection<Rate> _rateList = null;
		public ObservableCollection<Rate> rateList { get { return _rateList; } }

		/// <summary>
		/// Subscribed to by the caller to receive the newly chosen rate
		/// </summary>
		public event Action<int, int> returnNewRate;

		public AddRate() {
			this.DataContext = this;
			InitializeComponent();

			// Create list of all rates, excluding undesignated
			_rateList = new ObservableCollection<Rate>(
				CommonData.rates.Where(x => x.rateId != 0));

			UpdateListWithRank();
		}

		/// <summary>
		/// Returns the newly selected rate and rank to the caller
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_OK_Click(object sender, RoutedEventArgs e) {
			if (returnNewRate != null && ComboBox_Rate.SelectedItem != null) {
				returnNewRate((
					ComboBox_Rate.SelectedItem as Rate).rateId, 
					GetSelectedRank());
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

		/// <summary>
		/// Called when the rank radio button has changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Rank_Changed(object sender, RoutedEventArgs e) {
			UpdateListWithRank();
		}

		/// <summary>
		/// Returns the rank of the selected radio button
		/// </summary>
		/// <returns></returns>
		private int GetSelectedRank() {
			if((bool)Radio_Rank1.IsChecked) {
				return 1;
			} else if((bool)Radio_Rank2.IsChecked) {
				return 2;
			} else {
				return 3;
			}
		}

		/// <summary>
		/// Updates all of the rates with the newly selected rank so
		/// their text and images change.
		/// </summary>
		private void UpdateListWithRank() {
			if (_rateList != null) {
				int rank = GetSelectedRank();

				this.Dispatcher.Invoke(() => {
					foreach (Rate r in _rateList) {
						r.rank = rank;
					}
				});

				NotifyPropertyChanged("wpfRateList");
				ComboBox_Rate.Items.Refresh();
			}
		}

		/// <summary>
		/// Notifies the window that a bound property has changed.
		/// </summary>
		/// <param name="name"></param>
		private void NotifyPropertyChanged(string name) {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
