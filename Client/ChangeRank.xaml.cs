using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using ANWI;

namespace Client {
	/// <summary>
	/// Interaction logic for ChangeRank.xaml
	/// </summary>
	public partial class ChangeRank : Window {

		private ObservableCollection<Rank> rankList = null;

		public ObservableCollection<Rank> wpfRankList { get { return rankList; } }

		public event Action<int> ReturnNewRank;

		public ChangeRank() {
			this.DataContext = this;
			rankList = new ObservableCollection<Rank>(CommonData.ranks);

			InitializeComponent();
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e) {
			if(ReturnNewRank != null && ComboBox_Rank.SelectedItem != null) {
				ReturnNewRank((ComboBox_Rank.SelectedItem as Rank).id);
			}
			this.Close();
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}
	}
}
