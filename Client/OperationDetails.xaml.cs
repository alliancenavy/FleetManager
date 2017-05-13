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
using System.ComponentModel;

namespace Client {
	/// <summary>
	/// Interaction logic for OperationDetails.xaml
	/// </summary>
	public partial class OperationDetails : MailboxWindow, INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;

		private string opUUID;
		private int userId;

		#region Double-Bound WPF Variables

		private bool _freeMove = false;
		public bool freeMove {
			get { return _freeMove; }
			set {
				if(_freeMove != value) {
					_freeMove = value;
					NotifyPropertyChanged("freeMove");
				}
			}
		}

		#endregion

		#region Single-Bound WPF Variables
		public bool isFC { get; set; }

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
		public bool statusButtonEnabled {
			get { return _status != OperationStatus.DISMISSING; }
		}

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
		// Fleet Composition Table
		private ObservableCollection<FleetCompElement> _fleetComp
			= new ObservableCollection<FleetCompElement>();
		public ObservableCollection<FleetCompElement> fleetComp {
			get { return _fleetComp; }
			set {
				if(_fleetComp != value) {
					_fleetComp = value;
					NotifyPropertyChanged("fleetComp");
				}
			}
		}

		//
		// Participant Numbers
		public int currentUserNumber { get { return _roster.Count; } }
		public int totalCriticalSlots { get; set; }
		public int totalSlots { get; set; }

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
		#endregion

		#region Instance Members
		private FleetCompElement selectedShip = null;
		private ListBox activePositionList = null;
		private ListBoxItem draggedItem = null;

		private int lastWingNumber = 1;

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

			MessageRouter.Instance.SetOpsPushTarget(this);

			AddProcessor(typeof(ANWI.Messaging.Ops.FullOperationSnapshot),
				ProcessOpSnapshot);
			AddProcessor(typeof(ANWI.Messaging.Ops.UpdateStatus),
				ProcessUpdateStatus);
			AddProcessor(typeof(ANWI.Messaging.Ops.UpdateOOBShips),
				ProcessUpdateOOBShips);
			AddProcessor(typeof(ANWI.Messaging.Ops.UpdateOOBWings),
				ProcessUpdateOOBWings);
			AddProcessor(typeof(ANWI.Messaging.Ops.UpdateAssignments),
				ProcessUpdateAssignments);

			MessageRouter.Instance.SendOps(
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.GetOperation,
					new ANWI.Messaging.ReqExp.IdString(0, uuid)),
				this
				);
		}

		#region Helpers

		private void RecountSlots() {
			int total = 0;
			int critical = 0;

			foreach(FleetCompElement element in _fleetComp) {
				if(element is NamedShip) {
					NamedShip ship = element as NamedShip;
					total += ship.positions.Count;
					critical += CountCriticalPositions(ship.positions);
				} else if(element is Wing) {
					Wing wing = element as Wing;
					foreach(WingMember member in wing.members) {
						total += member.positions.Count;
						critical += CountCriticalPositions(member.positions);
					}
				}
			}

			totalCriticalSlots = critical;
			totalSlots = total;

			NotifyPropertyChanged("totalCriticalSlots");
			NotifyPropertyChanged("totalSlots");
		}

		private int CountCriticalPositions(List<OpPosition> positions) {
			int critical = 0;
			foreach (OpPosition p in positions) {
				if (p.critical)
					critical += 1;
			}
			return critical;
		}

		private void AddOOBElement(FleetCompElement ship) {
			foreach(FleetCompElement elem in fleetComp) {
				if (elem.uuid == ship.uuid)
					return;
			}

			this.Dispatcher.Invoke(() => { fleetComp.Add(ship); });
		}

		private void DeleteOOBElement(string uuid) {
			foreach (FleetCompElement elem in fleetComp) {
				if(elem.uuid == uuid) {
					this.Dispatcher.Invoke(() => { fleetComp.Remove(elem); });
					break;
				}
			}
		}

		private void AddParticipants(List<OpParticipant> add) {
			foreach(OpParticipant p in add) {
				// First make sure they aren't already in the list
				bool alreadyExists = false;
				foreach(OpParticipant existing in roster) {
					if(p.profile.id == existing.profile.id) {
						alreadyExists = true;
						break;
					}
				}

				if(!alreadyExists) {
					if (p.profile.id == userId)
						thisUser = p;
					this.Dispatcher.Invoke(() => { roster.Add(p); });
				}
			}
		}


		/*private ListBoxItem FindPosition(OpPosition pos) {
			foreach(FleetCompElement element in List_Fleet.Items) {
				if(element is NamedShip) {
					ListBoxItem elemContainer = List_Fleet.ItemContainerGenerator.ContainerFromItem(element) as ListBoxItem;
					elemContainer
				} else if(element is Wing) {

				}
			}

			return null;
		}*/
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

		private void Button_RosterRemove_Click(object sender, RoutedEventArgs e) {
			Button b = sender as Button;
			OpParticipant p = b.DataContext as OpParticipant;
			OpPosition.UnassignPosition(p.position);
		}

		private void Button_JoinOp_Click(object sender, RoutedEventArgs e) {

		}

		private void
		List_Positions_DoubleClick(object sender, RoutedEventArgs e) {
			OpPosition.AssignPosition( 
				(sender as ListBoxItem).DataContext as OpPosition,
				thisUser);
		}

		private void
		List_WingCrew_DoubleClick(object sender, RoutedEventArgs e) {
			OpPosition.AssignPosition( 
				(sender as ContentControl).DataContext as OpPosition,
				thisUser);
		}

		private void 
		Button_FighterCrewRemove_Click(object sender, RoutedEventArgs e) {

		}

		private void
		List_Fleet_SelectionChanged(object sender,
			SelectionChangedEventArgs e) {

			if (e.AddedItems.Count > 0) {
				// Newly selected element
				FleetCompElement elem = e.AddedItems[0] as FleetCompElement;

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
					= namedShip.DataContext as FleetCompElement;

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

			MessageRouter.Instance.SendOps(
				new ANWI.Messaging.Ops.AssignUser() {
					opUUID = opUUID,
					elemUUID = pos.elemUUID,
					positionUUID = pos.uuid,
					userId = (draggedItem.DataContext as OpParticipant).profile.id
				}, 
				null);
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

			AddParticipants(snap.op.roster);

			working = false;
		}

		private void ProcessUpdateStatus(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.UpdateStatus us 
				= p as ANWI.Messaging.Ops.UpdateStatus;
			status = us.status;
		}

		private void ProcessUpdateOOBShips(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.UpdateOOBShips us
				= p as ANWI.Messaging.Ops.UpdateOOBShips;

			if (us.addedShips != null) {
				foreach (NamedShip s in us.addedShips) {
					AddOOBElement(s);
				}
			}

			if (us.removedShips != null) {
				foreach (string s in us.removedShips) {
					DeleteOOBElement(s);
				}
			}
		}

		private void ProcessUpdateOOBWings(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.UpdateOOBWings uw
				= p as ANWI.Messaging.Ops.UpdateOOBWings;

			if (uw.addedWings != null) {
				foreach (Wing w in uw.addedWings) {
					AddOOBElement(w);
				}
			}

			if (uw.removedWings != null) {
				foreach (string w in uw.removedWings) {
					DeleteOOBElement(w);
				}
			}
		}

		private void ProcessUpdateAssignments(ANWI.Messaging.IMessagePayload p) {
			
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
