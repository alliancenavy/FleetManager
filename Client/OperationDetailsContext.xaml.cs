using ANWI.FleetComp;
using System.Windows;
using System.Windows.Controls;

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
				MessageRouter.Instance.SendOps(
					new ANWI.Messaging.Ops.AddOOBElement() {
						opUUID = opUUID,
						type = ANWI.Messaging.Ops.AddOOBElement.Type.FleetShip,
						shipId = v
					},
					null);
			};
			afs.ShowDialog();
		}

		private void Context_NewCustomShip(object sender, RoutedEventArgs e) {

		}

		private void Context_NewWing(object sender, RoutedEventArgs e) {
			MessageRouter.Instance.SendOps(
				new ANWI.Messaging.Ops.AddOOBElement() {
					opUUID = opUUID,
					type = ANWI.Messaging.Ops.AddOOBElement.Type.Wing
				},
				null);
		}

		private void Context_DeleteShipWing(object sender, RoutedEventArgs e) {
			FleetCompElement elem = (sender as MenuItem).DataContext 
				as FleetCompElement;
			if(elem != null) {
				MessageRouter.Instance.SendOps(
					new ANWI.Messaging.Ops.DeleteOOBElement(opUUID, elem.uuid),
					null);
			}
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