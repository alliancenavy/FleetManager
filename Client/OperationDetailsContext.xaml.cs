using ANWI;
using ANWI.FleetComp;
using System.Collections.Generic;
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
					new ANWI.Messaging.Ops.AddOOBUnit() {
						opUUID = opUUID,
						type = ANWI.Messaging.Ops.AddOOBUnit.Type.FleetShip,
						shipId = v
					},
					null);
			};
			afs.ShowDialog();
		}

		private void Context_NewCustomShip(object sender, RoutedEventArgs e) {
			// TODO
		}

		private void Context_NewWing(object sender, RoutedEventArgs e) {
			MessageRouter.Instance.SendOps(
				new ANWI.Messaging.Ops.AddOOBUnit() {
					opUUID = opUUID,
					type = ANWI.Messaging.Ops.AddOOBUnit.Type.Wing
				},
				null);
		}

		private void Context_DeleteShipWing(object sender, RoutedEventArgs e) {
			FleetUnit elem = (sender as MenuItem).DataContext 
				as FleetUnit;
			if(elem != null) {
				MessageRouter.Instance.SendOps(
					new ANWI.Messaging.Ops.DeleteOOBElement(opUUID, elem.uuid),
					null);
			}
		}
		#endregion

		#region Fleet Ships
		private void Context_SetAsFlagship(object sender, RoutedEventArgs e) {
			FleetUnit unit = (sender as MenuItem).DataContext as FleetUnit;
			MessageRouter.Instance.SendOps(new ANWI.Messaging.Ops.ModifyUnit() {
				opUUID = opUUID,
				unitUUID = unit.uuid,
				type = ANWI.Messaging.Ops.ModifyUnit.ChangeType.SetFlagship
			},
			null);
		}

		private void Context_AddShipPosition(object sender, RoutedEventArgs e) {
			FleetUnit unit = (sender as MenuItem).DataContext as FleetUnit;

			SimpleDropdownSelect select = new SimpleDropdownSelect(
				CommonData.assignmentRoles.ConvertAll<string>((r) => {
					return r.name; }));
			select.ReturnSelected += (index) => {
				MessageRouter.Instance.SendOps(
					new ANWI.Messaging.Ops.AddPosition() {
						opUUID = opUUID,
						unitUUID = unit.uuid,
						roleID = CommonData.assignmentRoles[index].id
					},
					null);
			};
			select.ShowDialog();
		}

		private void Context_UnassignAllShip(object sender, RoutedEventArgs e) {

		}

		private void Context_AssignSelfShip(object sender, RoutedEventArgs e) {
			OpPosition pos = (sender as MenuItem).DataContext as OpPosition;
			ChangeAssignment(pos.uuid, thisUser.profile.id);
		}

		private void Context_UnassignShip(object sender, RoutedEventArgs e) {
			OpPosition pos = (sender as MenuItem).DataContext as OpPosition;
			ChangeAssignment(pos.uuid, -1);
		}

		private void Context_CriticalShip(object sender, RoutedEventArgs e) {
			MenuItem item = sender as MenuItem;
			OpPosition pos = (item).DataContext as OpPosition;
			MessageRouter.Instance.SendOps(
				new ANWI.Messaging.Ops.SetPositionCritical() {
					opUUID = opUUID,
					posUUID = pos.uuid,
					critical = !item.IsChecked
				},
				null);
		}

		private void Context_DeleteShipPosition(object sender, RoutedEventArgs e) {
			OpPosition pos = (sender as MenuItem).DataContext as OpPosition;
			MessageRouter.Instance.SendOps(
				new ANWI.Messaging.Ops.DeletePosition() {
					opUUID = opUUID,
					posUUID = pos.uuid
				},
				null);
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