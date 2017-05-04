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

namespace Client {
	/// <summary>
	/// Interaction logic for NewShip.xaml
	/// </summary>
	public partial class NewShip : Window {

		public class Parameters {
			public int hullId;
			public string name;
			public bool fleetOwned;
			public bool isLTI;

			public Parameters() {

			}
		}

		private ObservableCollection<Hull> hullList = null;
		public ObservableCollection<Hull> wpfHullList { get { return hullList; } }

		public event Action<Parameters> returnNewShip;

		public NewShip() {
			this.DataContext = this;
			hullList = new ObservableCollection<Hull>(CommonData.largeHulls);

			InitializeComponent();
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e) {
			Hull selectedHull = Combobox_Hull.SelectedItem as Hull;
			if (Textbox_Name.Text.Length > 0 && selectedHull != null) {
				if (returnNewShip != null) {
					Parameters p = new Parameters() {
						hullId = selectedHull.id,
						name = "ANS " + Textbox_Name.Text,
						fleetOwned = getFleetOwned(),
						isLTI = Checkbox_LTI.IsChecked.Value
					};
					returnNewShip(p);
				}
				this.Close();
			}
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private bool getFleetOwned() {
			return Radio_OwnerFleet.IsChecked.Value;
		}
	}
}
