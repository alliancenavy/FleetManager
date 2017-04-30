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
using ANWI;
using Client.VesselRegHelpers;
using WebSocketSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Client {
	/// <summary>
	/// Interaction logic for FleetRegistry.xaml
	/// </summary>
	public partial class FleetRegistry : MailboxWindow, INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;

		private WebSocket socket = null;
		private Profile user = null;

		private ObservableCollection<VesselRecord> vesselList = new ObservableCollection<VesselRecord>();
		public ObservableCollection<VesselRecord> wpfVesselList { get { return vesselList; } }

		private NewAssignment newAssignmentWindow = null;

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
		public bool vesselSelectedAssign { get { return _currentVessel != null && user.privs.canAssign; } }
		public bool vesselSelectedStatus { get { return _currentVessel != null && (_currentVessel.ownerId == user.id || user.privs.isFleetAdmin); } }

		public FleetRegistry(WebSocket ws, Profile user) {
			this.DataContext = this;
			InitializeComponent();
			socket = ws;
			this.user = user;

			base.AddProcessor(typeof(ANWI.Messaging.FullVesselReg), LoadVesselList);
			base.AddProcessor(typeof(ANWI.Messaging.FullVessel), LoadVesselDetail);
			base.AddProcessor(typeof(ANWI.Messaging.ConfirmUpdate), ProcessConfirmUpdate);
			base.AddProcessor(typeof(ANWI.Messaging.FullRoster), ProcessUnassignedRoster);

			FetchVesselList();
		}

		private void FetchVesselList() {
			this.Dispatcher.Invoke(() => {
				vesselList.Clear();
				Spinner_List.Visibility = Visibility.Visible;
			});

			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.FleetReg,
				new ANWI.Messaging.Request(ANWI.Messaging.Request.Type.GetFleet));
		}

		private void LoadVesselList(ANWI.Messaging.IMessagePayload m) {
			ANWI.Messaging.FullVesselReg fvr = m as ANWI.Messaging.FullVesselReg;

			fvr.vessels.Sort((a, b) => {
				if (a.hull.ordering < b.hull.ordering)
					return -1;
				else if (a.hull.ordering == b.hull.ordering)
					return 0;
				else return 1;
			});

			foreach(LiteVessel v in fvr.vessels) {
				// For now 100 will be the boundary between named and unnamed vessels
				if (v.hull.ordering < 100) {
					NamedVessel vr = new NamedVessel();
					vr.v = v;
					this.Dispatcher.Invoke(() => { vesselList.Add(vr); });
				} else {
					// TODO
				}
			}

			this.Dispatcher.Invoke(() => { Spinner_List.Visibility = Visibility.Hidden; });
		}

		private void FetchVesselDetail(int id) {
			this.Dispatcher.Invoke(() => {
				Spinner_Detail.Visibility = Visibility.Visible;
				Button_ViewShip.IsEnabled = false;
			});

			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.FleetReg,
				new ANWI.Messaging.Request(ANWI.Messaging.Request.Type.GetVesselDetail, id));
		}

		private void LoadVesselDetail(ANWI.Messaging.IMessagePayload m) {
			this.Dispatcher.Invoke(() => {
				Spinner_Detail.Visibility = Visibility.Hidden;
				Button_ViewShip.IsEnabled = true;
			});

			ANWI.Messaging.FullVessel fvd = m as ANWI.Messaging.FullVessel;
			currentVessel = fvd.vessel;
		}

		private void ProcessConfirmUpdate(ANWI.Messaging.IMessagePayload m) {
			ANWI.Messaging.ConfirmUpdate cu = m as ANWI.Messaging.ConfirmUpdate;
			if(cu.success) {
				FetchVesselDetail(currentVessel.id);
			}
		}

		private void ProcessUnassignedRoster(ANWI.Messaging.IMessagePayload m) {
			ANWI.Messaging.FullRoster fr = m as ANWI.Messaging.FullRoster;
			if (newAssignmentWindow != null)
				newAssignmentWindow.SetUnassignedPersonnel(fr.members);
		}

		private void Button_NewShip_Click(object sender, RoutedEventArgs e) {

		}

		private void Button_ViewShip_Click(object sender, RoutedEventArgs e) {
			if(List_Fleet.SelectedItem != null) {
				NamedVessel nv = List_Fleet.SelectedItem as NamedVessel;
				if (nv != null) {
					FetchVesselDetail(nv.id);
				}
			}
		}

		private void Button_Close_Click(object sender, RoutedEventArgs e) {
			InvokeOnClose();
			this.Close();
		}

		private void Button_EditStatus_Click(object sender, RoutedEventArgs e) {
			List<string> statuses = new List<string>() {
				VesselStatus.ACTIVE.ToFriendlyString(),
				VesselStatus.DESTROYED.ToFriendlyString(),
				VesselStatus.DESTROYED_WAITING_REPLACEMENT.ToFriendlyString(),
				VesselStatus.DRYDOCKED.ToFriendlyString(),
				VesselStatus.DECOMMISSIONED.ToFriendlyString()
			};

			SimpleDropdownSelect select = new SimpleDropdownSelect(statuses);
			select.returnSelected += (s) => { UpdateShipStatus(currentVessel.id, (VesselStatus)s); };
			select.ShowDialog();
		}

		private void Button_RefreshRegistry_Click(object sender, RoutedEventArgs e) {
			FetchVesselList();
		}

		public void NotifyPropertyChanged(string name) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		private void List_Company_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			// Protect against firing this event twice when the other listbox deselects us
			if (e.AddedItems.Count > 0) {
				// Set the other list's select to null
				List_Embarked.SelectedIndex = -1;
			}
		}

		private void List_Embarked_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (e.AddedItems.Count > 0) {
				List_Company.SelectedIndex = -1;
			}
		}

		private void Button_AssignNew_Click(object sender, RoutedEventArgs e) {
			newAssignmentWindow = new NewAssignment(socket);
			newAssignmentWindow.returnNewAssignment += AddNewAssignment;
			newAssignmentWindow.ShowDialog();
			newAssignmentWindow = null;
		}

		private void Button_RemoveAssigned_Click(object sender, RoutedEventArgs e) {
			LiteProfile company = List_Company.SelectedItem as LiteProfile;
			LiteProfile embarked = List_Embarked.SelectedItem as LiteProfile;
			LiteProfile selected = company != null ? company : embarked != null ? embarked : null;
			if(selected != null) {
				ANWI.Messaging.Message.Send(
					socket,
					ANWI.Messaging.Message.Routing.FleetReg,
					new ANWI.Messaging.EndAssignment(selected.id, selected.assignment.id));
			}
		}

		private void AddNewAssignment(int userId, int roleId) {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.FleetReg,
				new ANWI.Messaging.NewAssignment(userId, currentVessel.id, roleId));
		}

		private void UpdateShipStatus(int id, VesselStatus status) {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.FleetReg,
				new ANWI.Messaging.ChangeShipStatus(id, status));
		}
	}
}
