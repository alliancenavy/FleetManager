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
using ANWI;
using System.ComponentModel;

namespace Client {
	public partial class AddRate : Window, INotifyPropertyChanged { 

		private ObservableCollection<Rate> rateList = null;

		public ObservableCollection<Rate> wpfRateList { get { return rateList; } }
		public event PropertyChangedEventHandler PropertyChanged;

		public event Action<int, int> returnNewRate;

		public AddRate() {
			this.DataContext = this;
			InitializeComponent();

			rateList = new ObservableCollection<Rate>(CommonData.rates.Where(x => x.id != 0));

			UpdateListWithRank();
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e) {
			if (returnNewRate != null && ComboBox_Rate.SelectedItem != null) {
				returnNewRate((
					ComboBox_Rate.SelectedItem as Rate).id, GetSelectedRank());
			}
			this.Close();
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void Rank_Changed(object sender, RoutedEventArgs e) {
			UpdateListWithRank();
		}

		private int GetSelectedRank() {
			if((bool)Radio_Rank1.IsChecked) {
				return 1;
			} else if((bool)Radio_Rank2.IsChecked) {
				return 2;
			} else {
				return 3;
			}
		}

		private void UpdateListWithRank() {
			if (rateList != null) {
				int rank = GetSelectedRank();

				this.Dispatcher.Invoke(() => {
					foreach (Rate r in rateList) {
						r.rank = rank;
					}
				});

				NotifyPropertyChanged("wpfRateList");
				ComboBox_Rate.Items.Refresh();
			}
		}

		public void NotifyPropertyChanged(string name) {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
