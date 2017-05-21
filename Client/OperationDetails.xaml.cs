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
using ANWI.FleetComp;
using Client.C2;
using System.ComponentModel;

namespace Client {
	/// <summary>
	/// Interaction logic for OperationDetails.xaml
	/// </summary>
	public partial class OperationDetails : MailboxWindow, INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;

		private string opUUID;
		private int userId;

		private OrderOfBattle fleet = new OrderOfBattle();

		#region Double-Bound WPF Variables

		#endregion

		#region Single-Bound WPF Variables
		//
		// Whether or not the user is the fleet commander
		private bool _isFC = false;
		public bool isFC {
			get { return _isFC; }
			set {
				if(_isFC != value) {
					_isFC = value;
					NotifyPropertyChanged("isFC");
					NotifyPropertyChanged("FCControlsEnabled");
				}
			}
		}

		//
		// Free move enabled or disabled
		private bool _freeMove = false;
		public bool freeMove {
			get { return _freeMove; }
			set {
				if (_freeMove != value) {
					_freeMove = value;
					NotifyPropertyChanged("freeMove");
				}
			}
		}

		//
		// Fleet Composition
		public ReadOnlyCollection<FleetUnit> fleetComp {
			get { return fleet.Fleet; }
		}

		//
		// Name of the Operation
		public string _operationName = "Operation Name";
		public string operationName {
			get {
				return _operationName;
			}
			set {
				if (_operationName != value) {
					_operationName = value;
					NotifyPropertyChanged("operationName");
				}
			}
		}

		//
		// Operation Type
		private OperationType _type = OperationType.PATROL;
		public OperationType type {
			get { return _type; }
			set {
				if (_type != value) {
					_type = value;
					NotifyPropertyChanged("type");
					NotifyPropertyChanged("typeString");
				}
			}
		}
		public string typeString {
			get { return type.ToFriendlyString(); }
		}

		//
		// Stage in lifecycle
		private OperationStatus _status;
		public OperationStatus status {
			get { return _status; }
			set {
				if(_status != value) {
					_status = value;
					NotifyPropertyChanged("status");
					NotifyPropertyChanged("statusString");
					NotifyPropertyChanged("statusButtonTooltip");
					NotifyPropertyChanged("statusButtonEnabled");
				}
			}
		}
		public string statusString { get { return _status.ToFriendlyString(); } }
		public string statusButtonTooltip { get {
				if (_status == OperationStatus.DISMISSING)
					return "Next Stage: None";
				else
					return "Next Stage: " + _status.Next().ToFriendlyString();
			} }

		//
		// Participant Roster
		private OpParticipant thisUser = null;
		private ObservableCollection<OpParticipant> _roster 
			= new ObservableCollection<OpParticipant>();
		public ObservableCollection<OpParticipant> roster {
			get { return _roster; }
			set {
				if(_roster != value) {
					_roster = value;
					NotifyPropertyChanged("roster");
				}
			}
		}

		//
		// C2 Structure
		private bool _C2Unified = true;
		public bool C2Unified {
			get { return _C2Unified; }
			set {
				if(_C2Unified != value) {
					_C2Unified = value;
					NotifyPropertyChanged("C2Unified");
				}
			}
		}
		private List<C2.Channel> _C2Channels = null;
		public List<C2.Channel> C2Channels {
			get { return _C2Channels; }
			set {
				if(_C2Channels != value) {
					_C2Channels = value;
					NotifyPropertyChanged("C2Channels");
				}
			}
		}

		//
		// Join button
		public bool thisUserIsJoined { get { return thisUser != null; } }
		public string joinButtonText { get {
				if (thisUserIsJoined)
					return "Leave Operation";
				else
					return "Join Operation";
			} }

		//
		// Participant Numbers
		public int currentUserNumber { get { return _roster.Count; } }
		public int totalCriticalSlots {
			get { return fleet.TotalCriticalPositions; }
		}
		public int totalSlots { get { return fleet.TotalPositions; } }

		//
		// Working/Busy
		private bool _working = true;
		public bool working {
			get { return _working; }
			set {
				if (_working != value) {
					_working = value;
					NotifyPropertyChanged("working");
					NotifyPropertyChanged("controlEnabled");
					
				}
			}
		}
		public bool controlEnabled { get { return !working; } }

		//
		// Enabling and Disabling certain controls
		public bool joinButtonEnabled { get { return !isFC; } }
		public bool statusButtonEnabled {
			get { return isFC && (_status != OperationStatus.DISMISSING); } }
		public bool FCControlsEnabled { get { return isFC; } }
		public bool assignmentControlsEnabled {
			get { return isFC || freeMove; } }
		#endregion

		#region Instance Members
		private FleetUnit selectedShip = null;
		private ListBox activePositionList = null;
		private ListBoxItem draggedItem = null;

		#endregion

		/// <summary>
		/// Opening window and request the operation from the server
		/// identified by the UUID
		/// </summary>
		/// <param name="uuid"></param>
		public OperationDetails(int userId, string uuid) {
			this.DataContext = this;
			InitializeComponent();

			opUUID = uuid;
			this.userId = userId;
			working = true;

			fleet.assignedPositionAdded += (pos) => {
				OpParticipant user = GetParticipant(pos.filledById);
				if(user != null) {
					pos.filledByPointer = user;
					user.position = pos;
				}
			};

			MessageRouter.Instance.SetOpsPushTarget(this);

			AddProcessor(typeof(ANWI.Messaging.Ops.FullOperationSnapshot),
				ProcessOpSnapshot);
			AddProcessor(typeof(ANWI.Messaging.Ops.UpdateRoster),
				ProcessUpdateRoster);
			AddProcessor(typeof(ANWI.Messaging.Ops.UpdateStatus),
				ProcessUpdateStatus);
			AddProcessor(typeof(ANWI.Messaging.Ops.UpdateUnitsShips),
				ProcessUpdateOOBShips);
			AddProcessor(typeof(ANWI.Messaging.Ops.UpdateUnitsWings),
				ProcessUpdateOOBWings);
			AddProcessor(typeof(ANWI.Messaging.Ops.UpdateUnitsBoats),
				ProcessUpdateOOBBoats);
			AddProcessor(typeof(ANWI.Messaging.Ops.UpdateAssignments),
				ProcessUpdateAssignments);
			AddProcessor(typeof(ANWI.Messaging.Ops.UpdatePositions),
				ProcessUpdatePositions);
			AddProcessor(typeof(ANWI.Messaging.Ops.UpdateShip),
				ProcessUpdateShip);
			AddProcessor(typeof(ANWI.Messaging.Ops.UpdateWing),
				ProcessUpdateWing);
			AddProcessor(typeof(ANWI.Messaging.Ops.UpdateSettings),
				ProcessUpdateSettings);

			RebuildC2();

			MessageRouter.Instance.SendOps(
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.GetOperation,
					new ANWI.Messaging.ReqExp.IdString(0, uuid)),
				this
				);
		}

		#region Helpers
		
		private void AddParticipants(List<OpParticipant> add) {
			foreach(OpParticipant p in add) {

				// Only add them if they don't already exist in the roster
				if (GetParticipant(p.profile.id) == null) {
					if (p.profile.id == userId) {
						thisUser = p;
						isFC = thisUser.isFC;
						NotifyPropertyChanged(string.Empty);
					}
					this.Dispatcher.Invoke(() => { roster.Add(p); });
				}
			}

			NotifyPropertyChanged("currentUserNumber");
		}

		private void RemoveParticipants(List<int> rem) {
			foreach (int id in rem) {
				OpParticipant user = GetParticipant(id);
				if (user != null) {
					if(user.profile.id == userId) {
						thisUser = null;
						isFC = false;
						NotifyPropertyChanged(string.Empty);
					}

					this.Dispatcher.Invoke(() => { roster.Remove(user); });
				}
			}

			NotifyPropertyChanged("currentUserNumber");
		}

		private OpParticipant GetParticipant(int id) {
			foreach(OpParticipant p in _roster) {
				if (p.profile.id == id)
					return p;
			}
			return null;
		}

		private void ChangeAssignment(OpPosition pos, int userID) {
			// Only the FC can overwrite any position at any time
			// Users can move themselves only when freemove is on and the
			// position is empty
			if (isFC || (
					freeMove &&
					thisUser != null &&
					(userID == thisUser.profile.id || (
						userID == -1 && pos.filledById == thisUser.profile.id
					)) &&
					pos.filledByPointer == null
				)) {

				MessageRouter.Instance.SendOps(new ANWI.Messaging.Ops.AssignUser() {
					opUUID = opUUID,
					positionUUID = pos.uuid,
					userId = userID
				},
				null);
			}
		}

		private void 
		AddPositionsToC2(List<User> users, List<OpPosition> positions) {
			foreach (OpPosition pos in positions) {
				if (pos.filledByPointer != null) {
					users.Add(new User(pos.filledByPointer) {
						thisUser = pos.filledByPointer.profile.id == userId
					});
				}
			}
		}

		private void RebuildC2() {
			if (C2Unified) {
				Channel chan = new Channel() {
					name = "Operation",
					expanded = true
				};

				// Go through each unit and add members filling positions
				foreach(FleetUnit unit in fleet.Fleet) {
					if(unit is Ship) {
						Ship ship = unit as Ship;
						AddPositionsToC2(chan.users, ship.positions);
					} else if(unit is Wing) {
						Wing wing = unit as Wing;
						foreach(Boat boat in wing.members) {
							AddPositionsToC2(chan.users, boat.positions);
						}
				}
					}

				C2Channels = new List<Channel>() { chan };
			} else {
				C2Channels = new List<Channel>();

				foreach(FleetUnit unit in fleet.Fleet) {
					if(unit is Ship) {
						Ship ship = unit as Ship;

						Channel chan = new Channel() {
							name = $"{ship.v.name} [{ship.v.fullHullNumber}]",
							unitUUID = ship.uuid,
							expanded = true
						};

						AddPositionsToC2(chan.users, ship.positions);

						C2Channels.Add(chan);
					} else if(unit is Wing) {
						Wing wing = unit as Wing;

						Channel chan = new Channel() {
							name = $"{wing.name} [{wing.callsign}]",
							unitUUID = wing.uuid,
							expanded = true
						};

						foreach(Boat boat in wing.members) {
							AddPositionsToC2(chan.users, boat.positions);
						}

						C2Channels.Add(chan);
					}
				}

				NotifyPropertyChanged("C2Channels");
			}
		}
		#endregion

		#region Window Event Handlers
		private void MailboxWindow_MouseDown(object sender, MouseButtonEventArgs e) {
			Grid_Root.Focus();
		}

		private void Button_ChangeStatus_Click(object sender, RoutedEventArgs e) {
			MessageRouter.Instance.SendOps(
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.AdvanceOpLifecycle,
						new ANWI.Messaging.ReqExp.IdString(0, opUUID)),
				null
				);
		}

		private void Button_JoinOp_Click(object sender, RoutedEventArgs e) {
			if(thisUserIsJoined) {
				// Leave the op
				MessageRouter.Instance.SendOps(new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.LeaveOperation,
					new ANWI.Messaging.ReqExp.IdString(userId, opUUID)
					),
					null);
			} else {
				// Join the op
				MessageRouter.Instance.SendOps(new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.JoinOperation,
					new ANWI.Messaging.ReqExp.IdString(userId, opUUID)
					),
					null);
			}
		}

		private void 
		Checkbox_FreeMove_Click(object sender, RoutedEventArgs e) {
			CheckBox check = sender as CheckBox;
			
			// Prevent the checkbox from unchecking before the response
			// comes back
			check.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateTarget();

			MessageRouter.Instance.SendOps(new ANWI.Messaging.Ops.SetFreeMove() {
				opUUID = opUUID,
				freeMove = !freeMove
			},
			null);
		}

		private void
		List_Positions_DoubleClick(object sender, RoutedEventArgs e) {
			OpPosition pos = (sender as ListBoxItem).DataContext as OpPosition;
			ChangeAssignment(pos, thisUser.profile.id);
		}

		private void
		List_WingCrew_DoubleClick(object sender, RoutedEventArgs e) {
			OpPosition pos = (sender as ContentControl).DataContext as OpPosition;
			ChangeAssignment(pos, thisUser.profile.id);
		}

		private void Radio_C2Type_Click(object sender, RoutedEventArgs e) {
			Radio_C2Unified.GetBindingExpression(
				RadioButton.IsCheckedProperty).UpdateTarget();
			Radio_C2Hierarchy.GetBindingExpression(
				RadioButton.IsCheckedProperty).UpdateTarget();

			MessageRouter.Instance.SendOps(new ANWI.Messaging.Ops.SetC2Type() {
				opUUID = opUUID,
				unified = !C2Unified
			},
			null);
		}

		private void
		List_Fleet_SelectionChanged(object sender,
			SelectionChangedEventArgs e) {

			if (e.AddedItems.Count > 0) {
				// Newly selected element
				FleetUnit elem = e.AddedItems[0] as FleetUnit;

				// If this element is set as the active outer list item then
				// this change was caused by List_Positions_SelectionChanged.
				// If that is the case there is nothing more to do
				// If this call was generated by an actual list selection,
				// these would not be equal
				if (elem != null && elem != selectedShip) {
					// This was generated by a click in the OOB listbox
					// Clear the selection of the positions list
					if (activePositionList != null) {
						activePositionList.SelectedItem = null;
						activePositionList = null;
					}

					selectedShip = elem;
				}
			}
		}

		private void List_Positions_SelectionChanged(object sender,
			SelectionChangedEventArgs e) {

			if (e.AddedItems.Count > 0) {
				// This event was sent by a the Positions listbox
				ListBox positions = sender as ListBox;

				// From that listbox we can get the root of the datatemplate
				// by going up one level
				Grid namedShip = positions.Parent as Grid;

				// This datatemplate has a FleetCompElement datacontext, which can 
				// be used to set the currently selected item in the main OOB list
				List_Fleet.SelectedItem = namedShip.DataContext;
				selectedShip
					= namedShip.DataContext as FleetUnit;

				// Clear the selection from the previous listbox so it doesn't
				// get stuck
				if (activePositionList != null && activePositionList != positions) {
					activePositionList.SelectedItem = null;
				}
				activePositionList = positions;
			}
		}

		private void List_Roster_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			if (draggedItem != null)
				return;

			UIElement element = List_Roster.InputHitTest(
				e.GetPosition(List_Roster)) as UIElement;

			while(element != null) {
				if(element is ListBoxItem) {
					draggedItem = element as ListBoxItem;
					break;
				}

				element = VisualTreeHelper.GetParent(element) as UIElement;
			}
		}

		private void MailboxWindow_MouseMove(object sender, MouseEventArgs e) {
			if (draggedItem == null)
				return;

			if(e.LeftButton == MouseButtonState.Released) {
				draggedItem = null;
				return;
			}

			//DataObject obj 
				//= new DataObject(DataFormats.Text, draggedItem.ToString());
			DragDrop.DoDragDrop(draggedItem, draggedItem, DragDropEffects.All);
		}

		private void PositionGrid_Drop(object sender, DragEventArgs e) {
			Grid grid = sender as Grid;
			OpPosition pos = grid.DataContext as OpPosition;
			OpParticipant user = (draggedItem.DataContext as OpParticipant);
			ChangeAssignment(pos, user.profile.id);
		}

		private void List_Roster_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			/*if(e.AddedItems.Count > 0) {
				OpParticipant p = e.AddedItems[0] as OpParticipant;
				if(p.position != null) {
				}
				FindPosition(null);
			}*/
		}

		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);

			MessageRouter.Instance.SendOps(
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.CloseOperation,
						new ANWI.Messaging.ReqExp.IdString(0, opUUID)),
					null
					);

			InvokeOnClose();
		}
		#endregion

		#region Message Processors

		private void ProcessOpSnapshot(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.FullOperationSnapshot snap
				= p as ANWI.Messaging.Ops.FullOperationSnapshot;

			operationName = snap.op.name;
			type = snap.op.type;
			status = snap.op.status;
			freeMove = snap.op.freeMove;
			C2Unified = snap.op.C2Unified;

			AddParticipants(snap.op.roster);

			if (snap.op.fleet != null) {
				foreach (FleetUnit f in snap.op.fleet) {
					fleet.AddUnit(f);
				}
			}

			working = false;

			RebuildC2();

			NotifyPropertyChanged(string.Empty);
		}

		private void ProcessUpdateRoster(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.UpdateRoster up 
				= p as ANWI.Messaging.Ops.UpdateRoster;

			if(up.addedUsers != null) {
				AddParticipants(up.addedUsers);
			}

			if(up.removedUsers != null) {
				RemoveParticipants(up.removedUsers);
			}
		}

		private void ProcessUpdateStatus(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.UpdateStatus us 
				= p as ANWI.Messaging.Ops.UpdateStatus;
			status = us.status;
		}

		private void ProcessUpdateOOBShips(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.UpdateUnitsShips us
				= p as ANWI.Messaging.Ops.UpdateUnitsShips;

			if (us.addedShips != null) {
				foreach (Ship s in us.addedShips) {
					fleet.AddUnit(s);
				}
			}

			if (us.removedShips != null) {
				foreach (string s in us.removedShips) {
					fleet.DeleteUnit(s);
				}
			}

			RebuildC2();

			NotifyPropertyChanged("fleetComp");
		}

		private void ProcessUpdateOOBWings(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.UpdateUnitsWings uw
				= p as ANWI.Messaging.Ops.UpdateUnitsWings;

			if (uw.addedWings != null) {
				foreach (Wing w in uw.addedWings) {
					fleet.AddUnit(w);
				}
			}

			if (uw.removedWings != null) {
				foreach (string w in uw.removedWings) {
					fleet.DeleteUnit(w);
				}
			}

			RebuildC2();

			NotifyPropertyChanged("fleetComp");
		}

		private void ProcessUpdateOOBBoats(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.UpdateUnitsBoats ub
				= p as ANWI.Messaging.Ops.UpdateUnitsBoats;

			if(ub.added != null) {
				foreach (Boat add in ub.added)
					fleet.AddUnit(add);
			}

			if(ub.removed != null) {
				foreach (string rem in ub.removed)
					fleet.DeleteUnit(rem);
			}

			RebuildC2();

			NotifyPropertyChanged(string.Empty);
		}

		private void ProcessUpdateAssignments(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.UpdateAssignments au
				= p as ANWI.Messaging.Ops.UpdateAssignments;

			if(au.added != null) {
				foreach(Tuple<int, string> add in au.added) {
					OpParticipant member = GetParticipant(add.Item1);
					if(member != null)
						fleet.AssignPosition(add.Item2, member);
				}
			}

			if(au.removedByUser != null) {
				foreach(int rem in au.removedByUser) {
					OpParticipant member = GetParticipant(rem);
					if (member != null && member.position != null)
						fleet.ClearPosition(member.position.uuid);
				}
			}

			if(au.removedByUUID != null) {
				foreach(string rem in au.removedByUUID) {
					fleet.ClearPosition(rem);
				}
			}

			RebuildC2();

			NotifyPropertyChanged(string.Empty);
		}

		private void ProcessUpdatePositions(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.UpdatePositions up
				= p as ANWI.Messaging.Ops.UpdatePositions;

			if(up.added != null) {
				foreach(OpPosition pos in up.added) {
					fleet.AddPosition(pos);
				}
			}

			if(up.changed != null) {
				foreach(OpPosition pos in up.changed) {
					OpPosition old = fleet.GetPosition(pos.uuid);
					if(old != null) {
						old.critical = pos.critical;
					}
				}
			}

			if(up.removed != null) {
				foreach(string rem in up.removed) {
					fleet.DeletePosition(rem);
				}
			}

			RebuildC2();

			NotifyPropertyChanged(string.Empty);
		}

		private void ProcessUpdateShip(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.UpdateShip ups
				= p as ANWI.Messaging.Ops.UpdateShip;

			switch(ups.type) {
				case ANWI.Messaging.Ops.UpdateShip.Type.SetFlagship:
					fleet.SetFlagship(ups.shipUUID);
					break;
			}

			NotifyPropertyChanged(string.Empty);
		}

		private void ProcessUpdateWing(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.UpdateWing upw
				= p as ANWI.Messaging.Ops.UpdateWing;

			switch(upw.type) {
				case ANWI.Messaging.Ops.UpdateWing.Type.SetName:
					fleet.SetWingName(upw.wingUUID, upw.str);
					break;

				case ANWI.Messaging.Ops.UpdateWing.Type.SetCallsign:
					fleet.SetWingCallsign(upw.wingUUID, upw.str);
					break;

				case ANWI.Messaging.Ops.UpdateWing.Type.ChangeWingCommander:
					fleet.SetWingCommander(upw.boatUUID);
					break;

				case ANWI.Messaging.Ops.UpdateWing.Type.ChangeRole:
					fleet.ChangeWingRole(upw.wingUUID, (Wing.Role)upw.integer);
					break;
			}

			NotifyPropertyChanged(string.Empty);
		}

		private void ProcessUpdateSettings(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.UpdateSettings ups
				= p as ANWI.Messaging.Ops.UpdateSettings;

			freeMove = ups.freeMove;
			C2Unified = ups.C2Unified;

			RebuildC2();

			NotifyPropertyChanged(string.Empty);
		}
		#endregion

		/// <summary>
		/// Notifies the UI when a bound property changes
		/// </summary>
		/// <param name="name"></param>
		public void NotifyPropertyChanged(string name) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
