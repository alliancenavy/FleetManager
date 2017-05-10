using System.Windows;

namespace Client {
	/// <summary>
	/// Portion of the OperationDetails code-behind for making context
	/// menus function.
	/// </summary>
	public partial class OperationDetails : MailboxWindow {
		#region Composition
		private void Context_NewFleetShip(object sender, RoutedEventArgs e) {
			Operations.AddFleetShip afs = new Operations.AddFleetShip();
			afs.returnNewShip += (v) => {
				AddNewShip(v);
			};
			afs.ShowDialog();
		}

		private void Context_NewCustomShip(object sender, RoutedEventArgs e) {

		}

		private void Context_NewWing(object sender, RoutedEventArgs e) {
			AddNewWing();
		}
		#endregion

		#region Fleet Ships
		private void Context_SetAsFlagship(object sender, RoutedEventArgs e) {

		}

		private void Context_AddShipPosition(object sender, RoutedEventArgs e) {

		}

		private void Context_UnassignAllShip(object sender, RoutedEventArgs e) {

		}

		private void Context_AssignSelfShip(object sender, RoutedEventArgs e) {

		}

		private void Context_UnassignShip(object sender, RoutedEventArgs e) {

		}

		private void Context_CriticalShip(object sender, RoutedEventArgs e) {

		}

		private void Context_DeleteShipPosition(object sender, RoutedEventArgs e) {

		}

		#endregion

		#region Wings
		private void Context_AddWingMember(object sender, RoutedEventArgs e) {

		}

		private void Context_ChangeCallsign(object sender, RoutedEventArgs e) {

		}

		private void Context_DeleteWing(object sender, RoutedEventArgs e) {

		}

		private void Context_AssignSelfWing(object sender, RoutedEventArgs e) {

		}

		private void Context_UnassignWing(object sender, RoutedEventArgs e) {

		}

		private void Context_CriticalWing(object sender, RoutedEventArgs e) {

		}

		private void Context_DeleteWingPosition(object sender, RoutedEventArgs e) {

		}
		#endregion

	}
}