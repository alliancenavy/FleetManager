using System;
using System.Collections.ObjectModel;
using System.Windows;
using ANWI;

namespace Client {
	/// <summary>
	/// Window for registering a new ship
	/// </summary>
	public partial class NewShip : Window {

		/// <summary>
		/// Parameters of the new ship.
		/// There are enough to make it easier to return a class
		/// </summary>
		public class Parameters {
			public int hullId;
			public string name;
			public bool fleetOwned;
			public bool isLTI;

			public Parameters() {

			}
		}

		public bool fleetAdmin { get; set; }

		// List of all available hulls
		private ObservableCollection<Hull> hullList = null;
		public ObservableCollection<Hull> wpfHullList {
			get { return hullList; }
		}

		// Subscribe to receive the new ship
		public event Action<Parameters> returnNewShip;

		public NewShip(bool fleetAdmin) {
			this.DataContext = this;
			hullList = new ObservableCollection<Hull>(CommonData.largeHulls);

			this.fleetAdmin = fleetAdmin;

			InitializeComponent();
		}

		/// <summary>
		/// Returns the new ship's information to the caller
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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

		/// <summary>
		/// Closes the window with no changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		/// <summary>
		/// Returns whether or not the fleet will own this ship.
		/// If false, the current user is the owner.
		/// </summary>
		/// <returns></returns>
		private bool getFleetOwned() {
			return Radio_OwnerFleet.IsChecked.Value;
		}
	}
}
