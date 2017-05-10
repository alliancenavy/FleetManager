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
using Client.FleetComp;
using System.ComponentModel;

namespace Client {
	/// <summary>
	/// Interaction logic for OperationDetails.xaml
	/// </summary>
	public partial class OperationDetails : MailboxWindow, INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;

		#region Double-Bound WPF Variables
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


		#endregion

		#region Single-Bound WPF Variables
		public bool isFC { get; set; }

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
		private ObservableCollection<FleetComp.FleetCompElement> _fleetComp
			= new ObservableCollection<FleetComp.FleetCompElement>();
		public ObservableCollection<FleetComp.FleetCompElement> fleetComp {
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
		#endregion

		#region Instance Members
		private FleetComp.FleetCompElement selectedShip = null;
		private ListBox activePositionList = null;
		private ListBoxItem draggedItem = null;

		private int lastWingNumber = 1;

		public bool working { get; set; } = true;
		public bool controlEnabled { get { return !working; } }
		#endregion

		/// <summary>
		/// Opening the window as the FC
		/// </summary>
		public OperationDetails() {
			this.DataContext = this;
			InitializeComponent();
			

		}

		/// <summary>
		/// Opening window as a member
		/// </summary>
		/// <param name="uuid"></param>
		public OperationDetails(string uuid) {

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

		private void Assign(OpParticipant member, OpPosition job) {
			UnAssign(member);

			member.position = job;
			job.filledById = member.profile.id;
			job.filledByPointer = member;
		}

		private void UnAssign(OpParticipant member) {
			if(member.position != null) {
				member.position.filledByPointer = null;
				member.position.filledById = -1;
				member.position = null;
			}
		}

		private void AddNewShip(LiteVessel vessel) {
			fleetComp.Add(new FleetComp.NamedShip() {
				v = vessel,
				isFlagship = false,
				positions = new List<OpPosition>()
			});
		}

		private void AddNewWing() {
			fleetComp.Add(new FleetComp.Wing() {
				name = $"New Wing {lastWingNumber}",
				callsign = "No Callsign",
				primaryRole = FleetComp.Wing.Role.CAP,
				members = new List<FleetComp.WingMember>()
			});
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
			status = status.Next();
		}

		private void Button_RosterRemove_Click(object sender, RoutedEventArgs e) {
			Button b = sender as Button;
			OpParticipant p = b.DataContext as OpParticipant;
			UnAssign(p);
		}

		private void Button_JoinOp_Click(object sender, RoutedEventArgs e) {

		}

		private void
		List_Positions_DoubleClick(object sender, RoutedEventArgs e) {
			Assign(thisUser, (sender as ListBoxItem).DataContext as OpPosition);
		}

		private void
		List_WingCrew_DoubleClick(object sender, RoutedEventArgs e) {
			Assign(thisUser, 
				(sender as ContentControl).DataContext as OpPosition);
		}

		private void 
		Button_FighterCrewRemove_Click(object sender, RoutedEventArgs e) {

		}

		private void
		List_Fleet_SelectionChanged(object sender,
			SelectionChangedEventArgs e) {

			if (e.AddedItems.Count > 0) {
				// Newly selected element
				FleetComp.FleetCompElement elem
					= e.AddedItems[0] as FleetComp.FleetCompElement;

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
					= namedShip.DataContext as FleetComp.FleetCompElement;

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

			// If there's already someone in this slot unassign them
			if (pos.filledByPointer != null)
				UnAssign(pos.filledByPointer);

			Assign(draggedItem.DataContext as OpParticipant, pos);
		}

		private void List_Roster_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			/*if(e.AddedItems.Count > 0) {
				OpParticipant p = e.AddedItems[0] as OpParticipant;
				if(p.position != null) {
				}
				FindPosition(null);
			}*/
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
