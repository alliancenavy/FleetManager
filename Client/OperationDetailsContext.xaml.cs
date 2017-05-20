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
			Operations.AddFleetShip afs 
				= new Operations.AddFleetShip(fleet.Fleet);
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

		private void Context_DeleteUnit(object sender, RoutedEventArgs e) {
			FleetUnit elem = (sender as MenuItem).DataContext 
				as FleetUnit;
			if(elem != null) {
				MessageRouter.Instance.SendOps(
					new ANWI.Messaging.Ops.DeleteOOBElement(opUUID, elem.uuid),
					null);
			}
		}
		#endregion

		#region Shared
		private void Context_AssignSelf(object sender, RoutedEventArgs e) {
			OpPosition pos = (sender as MenuItem).DataContext as OpPosition;
			ChangeAssignment(pos, thisUser.profile.id);
		}

		private void Context_Unassign(object sender, RoutedEventArgs e) {
			OpPosition pos = (sender as MenuItem).DataContext as OpPosition;
			ChangeAssignment(pos, -1);
		}

		private void Context_CriticalPosition(object sender, RoutedEventArgs e) {
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

		private void Context_DeletePosition(object sender, RoutedEventArgs e) {
			OpPosition pos = (sender as MenuItem).DataContext as OpPosition;
			MessageRouter.Instance.SendOps(
				new ANWI.Messaging.Ops.DeletePosition() {
					opUUID = opUUID,
					posUUID = pos.uuid
				},
				null);
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
				CommonData.shipRoles.ConvertAll<string>((r) => {
					return r.name; }));
			select.ReturnSelected += (index) => {
				MessageRouter.Instance.SendOps(
					new ANWI.Messaging.Ops.AddPosition() {
						opUUID = opUUID,
						unitUUID = unit.uuid,
						roleID = CommonData.shipRoles[index].id
					},
					null);
			};
			select.ShowDialog();
		}

		private void Context_UnassignAllShip(object sender, RoutedEventArgs e) {

		}

		#endregion

		#region Wings
		private void Context_AddWingMember(object sender, RoutedEventArgs e) {
			FleetUnit wing = (sender as MenuItem).DataContext as FleetUnit;

			SimpleDropdownSelect select = new SimpleDropdownSelect(
				CommonData.smallHulls.ConvertAll<string>((h) => {
					return h.name;
				}));
			select.ReturnSelected += (index) => {
				MessageRouter.Instance.SendOps(
					new ANWI.Messaging.Ops.AddOOBUnit() {
						opUUID = opUUID,
						wingUUID = wing.uuid,
						type = ANWI.Messaging.Ops.AddOOBUnit.Type.Boat,
						hullId = CommonData.smallHulls[index].id
					},
					null);
			};
			select.ShowDialog();
		}

		private void Context_ChangeNameWing(object sender, RoutedEventArgs e) {
			Wing wing = (sender as MenuItem).DataContext as Wing;
			SimpleTextPrompt prompt = new SimpleTextPrompt(
				"Change Wing Name", wing.name);
			prompt.ReturnText += (name) => {
				MessageRouter.Instance.SendOps(
					new ANWI.Messaging.Ops.ModifyUnit() {
						opUUID = opUUID,
						unitUUID = wing.uuid,
						type = ANWI.Messaging.Ops.ModifyUnit.ChangeType.ChangeName,
						str = name
					},
					null);
			};
			prompt.ShowDialog();
		}

		private void Context_ChangeCallsign(object sender, RoutedEventArgs e) {
			Wing wing = (sender as MenuItem).DataContext as Wing;
			SimpleTextPrompt prompt = new SimpleTextPrompt(
				"Change Callsign", wing.callsign);
			prompt.ReturnText += (cs) => {
				MessageRouter.Instance.SendOps(
					new ANWI.Messaging.Ops.ModifyUnit() {
						opUUID = opUUID,
						unitUUID = wing.uuid,
						type = ANWI.Messaging.Ops.ModifyUnit.ChangeType.ChangeCallsign,
						str = cs
					},
					null);
			};
			prompt.ShowDialog();
		}

		private void Context_AddBoatPosition(object sender, RoutedEventArgs e) {
			FleetUnit unit = (sender as MenuItem).DataContext as FleetUnit;

			SimpleDropdownSelect select = new SimpleDropdownSelect(
				CommonData.boatRoles.ConvertAll<string>((r) => {
					return r.name;
				}));
			select.ReturnSelected += (index) => {
				MessageRouter.Instance.SendOps(
					new ANWI.Messaging.Ops.AddPosition() {
						opUUID = opUUID,
						unitUUID = unit.uuid,
						roleID = CommonData.boatRoles[index].id
					},
					null);
			};
			select.ShowDialog();
		}

		private void Context_SetWingCommander(object sender, RoutedEventArgs e) {
			FleetUnit unit = (sender as MenuItem).DataContext as FleetUnit;
			MessageRouter.Instance.SendOps(new ANWI.Messaging.Ops.ModifyUnit() {
				opUUID = opUUID,
				unitUUID = unit.uuid,
				type = ANWI.Messaging.Ops.ModifyUnit.ChangeType.SetWingCommander
			},
			null);
		}

		private void Context_WingRoleInterceptor(object sender, RoutedEventArgs e) {
			FleetUnit unit = (sender as MenuItem).DataContext as FleetUnit;
			ChangeWingtype(unit.uuid, Wing.Role.INTERCEPTOR);
		}

		private void Context_WingRoleCAP(object sender, RoutedEventArgs e) {
			FleetUnit unit = (sender as MenuItem).DataContext as FleetUnit;
			ChangeWingtype(unit.uuid, Wing.Role.CAP);
		}

		private void Context_WingRoleBomber(object sender, RoutedEventArgs e) {
			FleetUnit unit = (sender as MenuItem).DataContext as FleetUnit;
			ChangeWingtype(unit.uuid, Wing.Role.BOMBER);
		}

		private void Context_WingRoleDropship(object sender, RoutedEventArgs e) {
			FleetUnit unit = (sender as MenuItem).DataContext as FleetUnit;
			ChangeWingtype(unit.uuid, Wing.Role.DROPSHIP);
		}

		private void ChangeWingtype(string uuid, Wing.Role role) {
			MessageRouter.Instance.SendOps(new ANWI.Messaging.Ops.ModifyUnit() {
				opUUID = opUUID,
				unitUUID = uuid,
				type = ANWI.Messaging.Ops.ModifyUnit.ChangeType.ChangeWingRole,
				integer = (int)role
			},
			null);
		}
		#endregion

	}
}