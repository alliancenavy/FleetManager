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
		public int minUserNumber { get; set; }
		public int totalSlots { get; set; }
		#endregion

		#region Instance Members
		private FleetComp.FleetCompElement selectedShip = null;
		private ListBox activePositionList = null;
		#endregion

		/// <summary>
		/// Opening the window as the FC
		/// </summary>
		public OperationDetails() {
			this.DataContext = this;
			InitializeComponent();

			status = OperationStatus.CONFIGURING;

			roster.Add(new OpParticipant() {
				isFC = true,
				profile = new LiteProfile() {
					nickname = "Mazer Ludd",
					rank = new Rank() {
						name = "Captain",
						abbrev = "CAPT",
						ordering = 5
					},
					primaryRate = new Rate() {
						name = "Skipper",
						abbrev = "SK",
						rank = 2
					}
				}
			});

			roster.Add(new OpParticipant() {
				isFC = false,
				profile = new LiteProfile() {
					nickname = "Spaceman Timmy",
					rank = new Rank() {
						name = "Spacer",
						abbrev = "S",
						ordering = 1
					},
					primaryRate = new Rate() {
						name = "Quartermaster",
						abbrev = "QM",
						rank = 2
					}
				}
			});

			fleetComp.Add(new FleetComp.NamedShip() {
				v = new LiteVessel() {
					name = "ANS Legend of Dave",
					hullNumber = 10,
					hull = new Hull() {
						name = "Idris-P",
						symbol = "FF",
						role = "Frigate"
					}
				},
				isFlagship = true
			});

			fleetComp.Add(new FleetComp.NamedShip() {
				v = new LiteVessel() {
					name = "ANS Everlasting Snowmew",
					hullNumber = 1,
					hull = new Hull() {
						name = "Polaris",
						symbol = "K",
						role = "Corvette"
					}
				},
				isFlagship = false
			});

			/*RosterEntry re = new RosterEntry();
			re.name = "Mazer Ludd";
			re.rank = new Rank();
			re.rank.abbrev = "CAPT";
			re.rank.ordering = 5;
			re.primaryRate = new Rate();
			re.primaryRate.abbrev = "SK";
			re.primaryRate.rank = 2;
			roster.Add(re);*/
		}

		/// <summary>
		/// Opening window as a member
		/// </summary>
		/// <param name="uuid"></param>
		public OperationDetails(string uuid) {

		}

		#region Window Event Handlers
		private void MailboxWindow_MouseDown(object sender, MouseButtonEventArgs e) {
			Grid_Root.Focus();
		}

		private void Button_ChangeStatus_Click(object sender, RoutedEventArgs e) {
			status = status.Next();
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

		private void Button_Remove_Click(object sender, RoutedEventArgs e) {

		}

		private void Button_JoinOp_Click(object sender, RoutedEventArgs e) {

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
	}
}
