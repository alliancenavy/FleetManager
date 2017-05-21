using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ANWI;
using Client.VesselRegHelpers;
using WebSocketSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Client {
	
	/// <summary>
	/// Window for viewing the ships currently in the fleet
	/// </summary>
	public partial class FleetRegistry : MailboxWindow, INotifyPropertyChanged {

		#region Instance Members
		public event PropertyChangedEventHandler PropertyChanged;
		
		private Profile user = null;

		// List of registered vessels
		private ObservableCollection<VesselRecord> _vesselList 
			= new ObservableCollection<VesselRecord>();
		public ObservableCollection<VesselRecord> vesselList {
			get { return _vesselList; } }

		// The currently selected vessel
		// Automatically updates the window when changed via notifications
		private Vessel _currentVessel = null;
		public Vessel currentVessel {
			get { return _currentVessel; }
			set {
				if(_currentVessel != value) {
					_currentVessel = value;
					NotifyPropertyChanged("currentVessel");
					NotifyPropertyChanged("vesselSelectedAssign");
					NotifyPropertyChanged("vesselSelectedStatus");
				}
			}
		}
		#endregion

		#region WPF Helpers
		// Helper for if the current user can assign people to this ship
		public bool vesselSelectedAssign { get {
				return _currentVessel != null && user.privs.canAssign; } }

		// Helper for if the current user can edit the status of this ship
		// They can if they are the owner or if they are a fleet admin
		public bool vesselSelectedStatus { get {
				return _currentVessel != null && 
					(_currentVessel.ownerId == user.id || 
						user.privs.isFleetAdmin);
			} }
		#endregion

		#region Constructors
		public FleetRegistry(Profile user) {
			this.DataContext = this;
			InitializeComponent();
			this.user = user;

			base.AddProcessor(
				typeof(ANWI.Messaging.FullVesselReg), LoadVesselList);
			base.AddProcessor(
				typeof(ANWI.Messaging.FullVessel), LoadVesselDetail);
			base.AddProcessor(
				typeof(ANWI.Messaging.ConfirmUpdate), ProcessConfirmUpdate);

			FetchVesselList();
		}
		#endregion

		#region Requests
		/// <summary>
		/// Sends a request for the vessel list to the server
		/// </summary>
		private void FetchVesselList() {
			this.Dispatcher.Invoke(() => {
				_vesselList.Clear();
				Spinner_List.Visibility = Visibility.Visible;
			});

			MessageRouter.Instance.SendMain(
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.GetFleet),
				this
				);
		}

		/// <summary>
		/// Requests the details on a specific vessel from the server
		/// </summary>
		/// <param name="id"></param>
		private void FetchVesselDetail(int id) {
			this.Dispatcher.Invoke(() => {
				Spinner_Detail.Visibility = Visibility.Visible;
				Button_ViewShip.IsEnabled = false;
			});

			MessageRouter.Instance.SendMain(
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.GetVesselDetail, id),
				this
				);
		}

		/// <summary>
		/// Callback function for the NewAssignment window
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleId"></param>
		private void AddNewAssignment(int userId, int roleId) {
			MessageRouter.Instance.SendMain(
				new ANWI.Messaging.NewAssignment(
					userId, currentVessel.id, roleId),
				this
				);
		}

		/// <summary>
		/// Callback function for the EditStatus window
		/// </summary>
		/// <param name="id"></param>
		/// <param name="status"></param>
		private void UpdateShipStatus(int id, VesselStatus status) {
			MessageRouter.Instance.SendMain(
				new ANWI.Messaging.ChangeShipStatus(id, status),
				this
				);
		}

		/// <summary>
		/// Callback function for the NewShip window
		/// </summary>
		/// <param name="p"></param>
		private void AddNewShip(NewShip.Parameters p) {
			MessageRouter.Instance.SendMain(
				new ANWI.Messaging.NewShip(
					p.hullId, p.name, p.isLTI, p.fleetOwned ? 0 : user.id),
				this
				);
		}

		/// <summary>
		/// Callback function for removing an assigned user
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="assignmentId"></param>
		private void RemoveAssignment(int userId, int assignmentId, int ship) {
			MessageRouter.Instance.SendMain(
				new ANWI.Messaging.EndAssignment(userId, assignmentId, ship),
				this
				);
		}

		/// <summary>
		/// Adds a hull embarked on the given ship
		/// </summary>
		/// <param name="shipId"></param>
		/// <param name="hullIndex">Index of the hull in the smallHulls
		/// array.  Not the ID in the database.</param>
		private void AddEquipment(int shipId, int hullIndex) {
			MessageRouter.Instance.SendMain(
					new ANWI.Messaging.Request(
						ANWI.Messaging.Request.Type.AddEquipment,
						new ANWI.Messaging.ReqExp.TwoIDs(
								shipId,
								CommonData.smallHulls[hullIndex].id
							)),
					this
					);
		}

		/// <summary>
		/// Removes a single instance of an embarked hull from the ship
		/// </summary>
		/// <param name="shipId"></param>
		/// <param name="hullId"></param>
		private void RemoveEquipment(int shipId, int hullId) {
			MessageRouter.Instance.SendMain(
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.RemoveEquipment,
					new ANWI.Messaging.ReqExp.TwoIDs(shipId, hullId)),
				this
				);
		}
		#endregion

		#region Response Processors
		/// <summary>
		/// Called upon receiving the FullVesselReg message.
		/// Loads the data into the list.
		/// </summary>
		/// <param name="m"></param>
		private void LoadVesselList(ANWI.Messaging.IMessagePayload m) {
			ANWI.Messaging.FullVesselReg fvr 
				= m as ANWI.Messaging.FullVesselReg;

			fvr.vessels.Sort((a, b) => {
				if (a.hull.ordering < b.hull.ordering)
					return -1;
				else if (a.hull.ordering == b.hull.ordering)
					return 0;
				else return 1;
			});

			foreach(LiteVessel v in fvr.vessels) {
				// For now 100 will be the boundary between named and 
				// unnamed vessels
				if (v.hull.ordering < 100) {
					NamedVessel vr = new NamedVessel();
					vr.v = v;
					this.Dispatcher.Invoke(() => { _vesselList.Add(vr); });
				} else {
					// TODO
				}
			}

			this.Dispatcher.Invoke(() => {
				Spinner_List.Visibility = Visibility.Hidden;
			});
		}

		/// <summary>
		/// Called when the vessel details are received
		/// Loads the details into the detail pane
		/// </summary>
		/// <param name="m"></param>
		private void LoadVesselDetail(ANWI.Messaging.IMessagePayload m) {
			this.Dispatcher.Invoke(() => {
				Spinner_Detail.Visibility = Visibility.Hidden;
				Button_ViewShip.IsEnabled = true;
			});

			ANWI.Messaging.FullVessel fvd = m as ANWI.Messaging.FullVessel;
			currentVessel = fvd.vessel;
		}

		/// <summary>
		/// When an update is confirmed, requests a refresh of the vessel
		/// </summary>
		/// <param name="m"></param>
		private void ProcessConfirmUpdate(ANWI.Messaging.IMessagePayload m) {
			ANWI.Messaging.ConfirmUpdate cu = m as ANWI.Messaging.ConfirmUpdate;
			if(cu.success) {
				FetchVesselDetail(cu.updatedId);
			}
		}
		#endregion

		#region Window Event Handlers
		/// <summary>
		/// Opens up the NewShip window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_NewShip_Click(object sender, RoutedEventArgs e) {
			NewShip ns = new NewShip(user.privs.isFleetAdmin);
			ns.returnNewShip += AddNewShip;
			ns.ShowDialog();
		}

		/// <summary>
		/// View Ship button clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_ViewShip_Click(object sender, RoutedEventArgs e) {
			if(List_Fleet.SelectedItem != null) {
				NamedVessel nv = List_Fleet.SelectedItem as NamedVessel;
				if (nv != null) {
					FetchVesselDetail(nv.id);
				}
			}
		}

		/// <summary>
		/// Opens the change ship status window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_EditStatus_Click(object sender, RoutedEventArgs e) {
			List<string> statuses = new List<string>() {
				VesselStatus.ACTIVE.ToFriendlyString(),
				VesselStatus.DESTROYED.ToFriendlyString(),
				VesselStatus.DESTROYED_WAITING_REPLACEMENT.ToFriendlyString(),
				VesselStatus.DRYDOCKED.ToFriendlyString(),
				VesselStatus.DECOMMISSIONED.ToFriendlyString()
			};

			SimpleDropdownSelect select = new SimpleDropdownSelect(statuses);
			select.ReturnSelected += (s) => {
				UpdateShipStatus(currentVessel.id, (VesselStatus)s);
			};
			select.ShowDialog();
		}

		/// <summary>
		/// Refreshes the vessel list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_RefreshRegistry_Click(object sender, 
			RoutedEventArgs e) {
			FetchVesselList();
		}

		/// <summary>
		/// Called when the selection changes in the ship's company list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void List_Company_SelectionChanged(object sender, 
			SelectionChangedEventArgs e) {
			// Protect against firing this event twice when the other 
			// listbox deselects us
			if (e.AddedItems.Count > 0) {
				// Set the other list's select to null
				List_EmbarkedPersonnel.SelectedIndex = -1;
			}
		}

		/// <summary>
		/// Called when the selection changes in the ship's embarked list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void List_Embarked_SelectionChanged(object sender, 
			SelectionChangedEventArgs e) {
			if (e.AddedItems.Count > 0) {
				List_Company.SelectedIndex = -1;
			}
		}

		/// <summary>
		/// Opens the NewAssignment window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_AssignNew_Click(object sender, RoutedEventArgs e) {
			if (Tabs_Embarked.SelectedIndex != 0)
				return;

			NewAssignment naw = new NewAssignment();
			naw.returnNewAssignment += AddNewAssignment;
			naw.ShowDialog();
			naw = null;
		}

		/// <summary>
		/// Removes a selected user from their assignment
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_RemoveAssigned_Click(object sender, 
			RoutedEventArgs e) {
			if (Tabs_Embarked.SelectedIndex != 0)
				return;

			LiteProfile company = List_Company.SelectedItem as LiteProfile;
			LiteProfile embarked 
				= List_EmbarkedPersonnel.SelectedItem as LiteProfile;
			LiteProfile selected = company != null ? 
				company : embarked != null ? embarked : null;

			if(selected != null) {
				Confirm c = new Confirm("Are you sure you want to remove" +
					$"{selected.fullName} from this ship?");
				c.yesAction += () => {
					RemoveAssignment(
						selected.id, 
						selected.assignment.id,
						currentVessel.id);
				};
				c.ShowDialog();
			}
		}

		/// <summary>
		/// Adds an embarked hull to this ship
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_AddEquip_Click(object sender, RoutedEventArgs e) {
			if (Tabs_Embarked.SelectedIndex != 1)
				return;

			SimpleDropdownSelect select
				= new SimpleDropdownSelect(
					CommonData.smallHulls.ConvertAll<string>((h) => {
						return h.name;
					}));
			select.ReturnSelected += (i) => {
				AddEquipment(currentVessel.id, i);
			};
			select.ShowDialog();
		}

		/// <summary>
		/// Removes one instance of the select hull
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_RemoveEquip_Click(object sender, RoutedEventArgs e) {
			if (Tabs_Embarked.SelectedIndex != 1)
				return;

			ShipEquipment equip 
				= List_EmbarkedEquip.SelectedItem as ShipEquipment;
			if(equip != null) {
				RemoveEquipment(currentVessel.id, equip.hull.id);
			}
		}

		/// <summary>
		/// Closes the window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Close_Click(object sender, RoutedEventArgs e) {
			InvokeOnClose();
			this.Close();
		}

		#endregion

		/// <summary>
		/// Notifies the UI that a bound property has changed
		/// </summary>
		/// <param name="name"></param>
		public void NotifyPropertyChanged(string name) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
