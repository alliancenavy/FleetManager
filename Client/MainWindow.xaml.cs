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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebSocketSharp;
using ANWI;
using MsgPack.Serialization;
using System.IO;
using System.Collections.ObjectModel;
using Client.VesselRegHelpers;
using System.Threading;
using System.ComponentModel;

namespace Client {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged {

		private readonly WebSocket socket;
		private AuthenticatedAccount account = null;
		private Profile currentProfile = null;
		private ObservableCollection<LiteProfile> rosterList = new ObservableCollection<LiteProfile>();
		private ObservableCollection<Operation> operationList = new ObservableCollection<Operation>();

		public event PropertyChangedEventHandler PropertyChanged;

		#region Other Windows
		private FleetRegistry fleetReg = null;
		#endregion

		#region Observable Members
		public Profile wpfProfile {
			get { return currentProfile; }
			set {
				if (currentProfile != value) {
					currentProfile = value;
					NotifyPropertyChanged("wpfProfile");
				}
			}
		}
		public ObservableCollection<LiteProfile> wpfRosterList { get { return rosterList; } }
		public ObservableCollection<Operation> wpfOpList { get { return operationList; } }

		public Privs userPrivileges { get { return account.profile.privs; } }
		#endregion

		#region Initialization
		/// <summary>
		/// Form constructor
		/// Opens a modal login window first.  When control returns it checks if there's
		/// a valid user and populates their profile info.
		/// </summary>
		public MainWindow() {
			// Open a modal login window
			// When the window closes the authclient member will be either null
			// or have a structure.
			// If null, the login failed.
			Login logwin = new Client.Login();
			logwin.returnuser += value => account = value;
			logwin.ShowDialog();

			// Once the control returns check if we have a valid account
			if(account == null) {
				// Just quit the app
				Application.Current.Shutdown();
			}

			// Once we have the basic data initialize the window
			this.DataContext = this;
			InitializeComponent();

			//
			// Populate profile sidebar with non-bound information
	//		PropertyChanged += (sender, e) => {
	//			if (e.PropertyName == "wpfProfile") {
	//				// Assigned ship
	//				Profile p = account.profile;
	//				if (p.assignment == null) {
	//					this.Dispatcher.Invoke(() => { Text_CurrentAssignment.Text = "No Current Assignment"; });
	//				} else {
	//					this.Dispatcher.Invoke(() => {
	//						Text_CurrentAssignment.Text = p.assignedShip.name + " (" + p.assignedShip.hull.type
 //+ " class " + p.assignedShip.hull.role + ")";
	//					});
	//				}

	//				// TODO: Time in service
	//			}
	//		};

			wpfProfile = account.profile;

			// Open connection to the main service
			socket = new WebSocket("ws://localhost:9000/main");
			socket.OnMessage += OnMessage;
			socket.OnError += SocketError;
			socket.Connect();

			// Note: blocking
			FetchCommonData();

			FetchRoster();
			FetchOps();

		}
		#endregion

		#region Window Event Handlers
		private void QuitButton_Click(object sender, RoutedEventArgs e) {
			Application.Current.Shutdown();
		}

		private void Button_RefreshRoster_Click(object sender, RoutedEventArgs e) {
			FetchRoster();
		}

		private void Button_RefreshOps_Click(object sender, RoutedEventArgs e) {
			FetchOps();
		}

		private void Button_NewOp_Click(object sender, RoutedEventArgs e) {

		}

		private void Button_OpenOp_Click(object sender, RoutedEventArgs e) {
			if(List_Ops.SelectedIndex >= 0) {
				// TODO
			}
		}

		private void Button_AddRate_Click(object sender, RoutedEventArgs e) {
			AddRate ar = new AddRate();
			ar.returnNewRate += (rateId, rank) => { AddRate(currentProfile.id, rateId, rank); };
			ar.ShowDialog();
		}

		private void Button_DeleteRate_Click(object sender, RoutedEventArgs e) {
			Rate selected = (List_Rates.SelectedItem as Rate);
			Confirm c = new Confirm($"Are you sure you want to delete {selected.FullName} from {currentProfile.rank.abbrev} {currentProfile.nickname}?");
			c.yesAction += () => { DeleteRate(currentProfile.id, selected.id); };
			c.ShowDialog();
		}

		private void Button_SetPrimaryRate_Click(object sender, RoutedEventArgs e) {
			if (List_Rates.SelectedItem != null) {
				Rate selected = (List_Rates.SelectedItem as Rate);
				Confirm c = new Confirm($"Are you sure you want to change your rate from {currentProfile.primaryRate.FullName} to {selected.FullName}?");
				c.yesAction += () => { SetPrimaryRate(currentProfile.id, selected.id); };
				c.ShowDialog();
			}
		}

		private void Button_ChangeRank_Click(object sender, RoutedEventArgs e) {
			ChangeRank cr = new ChangeRank();
			cr.ReturnNewRank += (rank) => { ChangeRank(currentProfile.id, rank); };
			cr.ShowDialog();
		}

		private void Button_ViewJacket_Click(object sender, RoutedEventArgs e) {
			if(List_Roster.SelectedItem != null) {
				LiteProfile p = List_Roster.SelectedItem as LiteProfile;
				ANWI.Messaging.Message.Send(
					socket,
					ANWI.Messaging.Message.Routing.Main,
					new ANWI.Messaging.Request(ANWI.Messaging.Request.Type.GetProfile, p.id));
			}
		}

		private void Button_OpenFleetReg_Click(object sender, RoutedEventArgs e) {
			if (fleetReg == null) {
				fleetReg = new FleetRegistry(socket);
				fleetReg.Show();
			}
		}

		#endregion

		#region Sockets and Messaging
		private void OnMessage(object sender, MessageEventArgs e) {
			ANWI.Messaging.Message msg = ANWI.Messaging.Message.Receive(e.RawData);

			switch(msg.address.dest) {
				case ANWI.Messaging.Message.Routing.Target.Main:
					ProcessMessage(msg);
					break;

				case ANWI.Messaging.Message.Routing.Target.FleetReg:
					if (fleetReg != null)
						fleetReg.DeliverMessage(msg);
					break;

				default:
					break;
			}
		}

		private void SocketError(object sender, WebSocketSharp.ErrorEventArgs e) {
			// TODO
		}
		#endregion

		#region Helpers

		private void AddRate(int userId, int rateId, int rank) {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.AddRate(userId, rateId, rank));
		}

		private void DeleteRate(int userId, int rateId) {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.DeleteRate(userId, rateId));
		}

		private void SetPrimaryRate(int userId, int rateId) {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.SetPrimaryRate(userId, rateId));
		}

		private void ChangeRank(int userId, int rankId) {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.ChangeRank(userId, rankId));
		}

		/// <summary>
		/// Gets commonly used data like the list of ranks and rates from the server
		/// for storing globally.
		/// Waits in a loop until the response is received so the program can't move
		/// forward without the data.
		/// </summary>
		private void FetchCommonData() {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.Request(ANWI.Messaging.Request.Type.GetCommonData));

			while(!CommonData.loaded) {
				Thread.Sleep(10);
			}
		}

		/// <summary>
		/// Processes messages meant to be delivered to the main window
		/// </summary>
		/// <param name="m">Incomming message</param>
		private void ProcessMessage(ANWI.Messaging.Message m) {
			if (m.payload is ANWI.Messaging.FullRoster) {
				LoadRoster(m.payload as ANWI.Messaging.FullRoster);
			} else if (m.payload is ANWI.Messaging.FullOperationsList) {
				LoadOps(m.payload as ANWI.Messaging.FullOperationsList);
			} else if (m.payload is ANWI.Messaging.AllCommonData) {
				CommonData.LoadAll(m.payload as ANWI.Messaging.AllCommonData);
			} else if (m.payload is ANWI.Messaging.ConfirmProfileUpdated) {
				ANWI.Messaging.ConfirmProfileUpdated cpu = m.payload as ANWI.Messaging.ConfirmProfileUpdated;
				FetchProfile(cpu.userId);
			} else if (m.payload is ANWI.Messaging.FullProfile) {
				ANWI.Messaging.FullProfile fp = m.payload as ANWI.Messaging.FullProfile;
				wpfProfile = fp.profile;
			}
		}

		/// <summary>
		/// Sends a request to the server for the full member roster
		/// </summary>
		private void FetchRoster() {
			// Clear the old list
			this.Dispatcher.Invoke(() => {
				rosterList.Clear();
				Spinner_Roster.Visibility = Visibility.Visible;
			});

			// Send a request to the server
			ANWI.Messaging.Message.Send(
				socket, 
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.Request(ANWI.Messaging.Request.Type.GetRoster));
		}

		/// <summary>
		/// Response handler for the full roster request.
		/// Populates the global roster list
		/// </summary>
		/// <param name="fr"></param>
		private void LoadRoster(ANWI.Messaging.FullRoster fr) {
			this.Dispatcher.Invoke(() => {
				rosterList.Clear();
				Spinner_Roster.Visibility = Visibility.Hidden;
			});

			// TODO: Sort by radio button instead of rank
			fr.members.Sort((a, b) => {
				if (a.rank.ordering > b.rank.ordering)
					return -1;
				else if (a.rank.ordering == b.rank.ordering)
					return 0;
				else return 0;
			});

			// Load all the records in
			foreach (LiteProfile p in fr.members) {
				this.Dispatcher.Invoke(() => {
					if (p.id == account.profile.id)
						p.isMe = true;
					rosterList.Add(p);
				});
			}
		}

		/// <summary>
		/// Sends a request to the server for the list of active operations
		/// </summary>
		private void FetchOps() {
			// Clear the old list
			this.Dispatcher.Invoke(() => {
				operationList.Clear();
				Spinner_Ops.Visibility = Visibility.Visible;
			});

			// Send a request to the server
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.Request(ANWI.Messaging.Request.Type.GetOperations));
		}

		private void LoadOps(ANWI.Messaging.FullOperationsList fol) {
			this.Dispatcher.Invoke(() => {
				operationList.Clear();
				Spinner_Ops.Visibility = Visibility.Hidden;
			});

			// Sort records by status
			fol.ops.Sort((a, b) => {
				return a.status.CompareTo(b.status);
			});

			// Insert each into the list
			foreach(Operation op in fol.ops) {
				this.Dispatcher.Invoke(() => { operationList.Add(op); });
			}
		}

		private void FetchProfile(int userId) {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.Request(ANWI.Messaging.Request.Type.GetProfile, userId));
		}

		#endregion

		public void NotifyPropertyChanged(string name) {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

	}
}
