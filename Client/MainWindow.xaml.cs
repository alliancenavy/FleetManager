using System;
using System.Collections.Generic;
using System.Windows;
using WebSocketSharp;
using ANWI;
using System.Collections.ObjectModel;
using System.Threading;
using System.ComponentModel;

namespace Client {
	/// <summary>
	/// Main application window
	/// Shows personnel jackets, full roster, and active operations
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;

		private readonly WebSocket socket;
		private AuthenticatedAccount account = null;
		private Profile currentProfile = null;
		private List<LiteProfile> originalRoster = null;
		private ObservableCollection<LiteProfile> rosterList 
			= new ObservableCollection<LiteProfile>();
		private ObservableCollection<LiteOperation> operationList 
			= new ObservableCollection<LiteOperation>();

		#region Other Windows
		private FleetRegistry fleetReg = null;
		private OperationDetails opDet = null;
		#endregion

		#region Observable Members
		public Profile wpfProfile {
			get { return currentProfile; }
			set {
				if (currentProfile != value) {
					currentProfile = value;
					NotifyPropertyChanged("wpfProfile");
					NotifyPropertyChanged("canSetPrimaryRate");
					NotifyPropertyChanged("canEditName");
				}
			}
		}
		public ObservableCollection<LiteProfile> wpfRosterList {
			get { return rosterList; }
		}
		public ObservableCollection<LiteOperation> wpfOpList {
			get { return operationList; }
		}

		public Privs userPrivileges { get { return account.profile.privs; } }
		public bool canSetPrimaryRate { get {
				return currentProfile != null && 
					account.profile.id == currentProfile.id;
			} }
		public bool canEditName {
			get { return currentProfile != null && 
					account.profile.id == currentProfile.id; }
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Form constructor
		/// Opens a modal login window first.  When control returns it checks 
		/// if there's a valid user it populates the rest of the window.
		/// </summary>
		public MainWindow() {
			
			// Last resort for any unhandled exceptions
			AppDomain.CurrentDomain.UnhandledException 
				+= new UnhandledExceptionEventHandler(UEHandler);

			//OperationDetails op = new OperationDetails();
			//op.ShowDialog();

			// Open a modal login window
			// When the window closes the authclient member will be either null
			// or have a structure.
			// If null, the login failed.
			Login logwin = new Client.Login();
			logwin.returnuser += value => account = value;
			logwin.ShowDialog();

			// Once the control returns check if we have a valid account
			if(account == null || account.profile == null) {
				// Just quit the app
				Application.Current.Shutdown();
				return;
			}

			// Once we have the basic data initialize the window
			this.DataContext = this;
			InitializeComponent();

			wpfProfile = account.profile;

			// Open connection to the main service
			socket = new WebSocket($"{CommonData.serverAddress}/main");
			socket.OnMessage += OnMessage;
			socket.OnError += SocketError;
			socket.SetCookie(
				new WebSocketSharp.Net.Cookie("name", account.profile.nickname)
				);
			socket.SetCookie(
				new WebSocketSharp.Net.Cookie("authtoken", account.authToken)
				);
			socket.SetCookie(
				new WebSocketSharp.Net.Cookie("auth0id", account.auth0_id)
				);
			socket.Connect();

			// Note: blocking
			FetchCommonData();

			FetchRoster();
			FetchOps();

		}
		#endregion

		#region Window Event Handlers
		/// <summary>
		/// Closes the application
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void QuitButton_Click(object sender, RoutedEventArgs e) {
			Application.Current.Shutdown();
		}

		/// <summary>
		/// Refreshes the roster listbox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_RefreshRoster_Click(object sender,
			RoutedEventArgs e) {
			FetchRoster();
		}

		/// <summary>
		/// Refreshes the operations listbox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_RefreshOps_Click(object sender, 
			RoutedEventArgs e) {
			FetchOps();
		}

		/// <summary>
		/// Starts a new operation
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_NewOp_Click(object sender, RoutedEventArgs e) {
			if (opDet == null) {
				opDet = new OperationDetails();
				opDet.Show();
			}
		}

		/// <summary>
		/// Opens an operation's details window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_OpenOp_Click(object sender, RoutedEventArgs e) {
			if(List_Ops.SelectedIndex >= 0) {
				// TODO
			}
		}

		/// <summary>
		/// Opens the AddRate window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_AddRate_Click(object sender, RoutedEventArgs e) {
			AddRate ar = new AddRate();
			ar.returnNewRate += (rateId, rank) => {
				AddRate(currentProfile.id, rateId, rank);
			};
			ar.ShowDialog();
		}

		/// <summary>
		/// Removes a rate from the currently selected user
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_DeleteRate_Click(object sender, RoutedEventArgs e) {
			Rate selected = (List_Rates.SelectedItem as Rate);
			if (selected != null) {
				Confirm c = new Confirm(
					$"Are you sure you want to delete {selected.fullName} from"+
					$" {currentProfile.rank.abbrev} {currentProfile.nickname}?"
					);
				c.yesAction += () => {
					DeleteRate(currentProfile.id, selected.struckId);
				};
				c.ShowDialog();
			}
		}

		/// <summary>
		/// Sets the primary rate for the currently selected user
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_SetPrimaryRate_Click(object sender, 
			RoutedEventArgs e) {
			Rate selected = (List_Rates.SelectedItem as Rate);
			if (selected != null) {  
				Confirm c = new Confirm(
					$"Are you sure you want to change your rate from " +
					$"{currentProfile.primaryRate.fullName} to " +
					$"{selected.fullName}?"
					);
				c.yesAction += () => {
					SetPrimaryRate(currentProfile.id, selected.struckId);
				};
				c.ShowDialog();
			}
		}

		/// <summary>
		/// Opens the ChangeRank window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_ChangeRank_Click(object sender, RoutedEventArgs e) {
			ChangeRank cr = new ChangeRank();
			cr.ReturnNewRank += (rank) => {
				ChangeRank(currentProfile.id, rank);
			};
			cr.ShowDialog();
		}

		/// <summary>
		/// Requests the full profile of the selected user
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_ViewJacket_Click(object sender, RoutedEventArgs e) {
			if(List_Roster.SelectedItem != null) {
				LiteProfile p = List_Roster.SelectedItem as LiteProfile;
				ANWI.Messaging.Message.Send(
					socket,
					ANWI.Messaging.Message.Routing.Main,
					new ANWI.Messaging.Request(
						ANWI.Messaging.Request.Type.GetProfile, p.id));
			}
		}

		/// <summary>
		/// Opens the FleetRegistry window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_OpenFleetReg_Click(object sender, 
			RoutedEventArgs e) {
			if (fleetReg == null) {
				fleetReg = new FleetRegistry(socket, account.profile);
				fleetReg.OnClose += (t) => { fleetReg = null; };
				fleetReg.Show();
			}
		}

		/// <summary>
		/// Changes the sort method of the roster listbox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Radio_RosterSort_Click(object sender, RoutedEventArgs e) {
			rosterList.Clear();

			if(Radio_Roster_Name.IsChecked.Value) {
				originalRoster.Sort((a, b) => {
					return a.nickname.CompareTo(b.nickname);
				});
			} else if(Radio_Roster_Rank.IsChecked.Value) {
				originalRoster.Sort((a, b) => {
					if (a.rank.ordering > b.rank.ordering)
						return -1;
					else if (a.rank.ordering == b.rank.ordering)
						return 0;
					else return 0;
				});
			}

			foreach(LiteProfile p in originalRoster) {
				rosterList.Add(p);
			}
		}

		/// <summary>
		/// Edit user nickname button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_EditName_Click(object sender, RoutedEventArgs e) {
			SimpleTextPrompt prompt = new SimpleTextPrompt("Edit Name", 
				currentProfile.nickname);
			prompt.ReturnText += (name) => {
				ChangeNickname(currentProfile.id, name);
			};
			prompt.ShowDialog();
		}

		/// <summary>
		/// Causes the application to quit when the main window closes
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);

			Application.Current.Shutdown();
		}

		#endregion

		#region Sockets and Messaging
		/// <summary>
		/// Handles routing for received messages
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMessage(object sender, MessageEventArgs e) {
			ANWI.Messaging.Message msg 
				= ANWI.Messaging.Message.Receive(e.RawData);

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
			} else if (m.payload is ANWI.Messaging.ConfirmUpdate) {
				ANWI.Messaging.ConfirmUpdate cpu 
					= m.payload as ANWI.Messaging.ConfirmUpdate;
				FetchProfile(cpu.updatedId);
			} else if (m.payload is ANWI.Messaging.FullProfile) {
				ANWI.Messaging.FullProfile fp
					= m.payload as ANWI.Messaging.FullProfile;
				wpfProfile = fp.profile;
			}
		}

		/// <summary>
		/// Handles socket errors
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SocketError(object sender, 
			WebSocketSharp.ErrorEventArgs e) {
			// TODO
		}
		#endregion

		#region Request Senders
		/// <summary>
		/// Gets commonly used data like the list of ranks and rates from 
		/// the server for storing globally.
		/// Waits in a loop until the response is received so the program can't 
		/// move forward without the data.
		/// </summary>
		private void FetchCommonData() {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.GetCommonData));

			while (!CommonData.loaded) {
				Thread.Sleep(10);
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
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.GetRoster));
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
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.GetOperations));
		}

		/// <summary>
		/// Sends a request for a user's full profile
		/// </summary>
		/// <param name="userId"></param>
		private void FetchProfile(int userId) {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.GetProfile, userId));
		}
		#endregion

		#region Response Processors

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

			originalRoster = fr.members;

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
		/// Loads the list of operations
		/// </summary>
		/// <param name="fol"></param>
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
			foreach(LiteOperation op in fol.ops) {
				this.Dispatcher.Invoke(() => { operationList.Add(op); });
			}
		}

		#endregion

		#region Callbacks
		/// <summary>
		/// Adds a rate to the given user
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="rateId"></param>
		/// <param name="rank"></param>
		private void AddRate(int userId, int rateId, int rank) {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.AddRate(userId, rateId, rank));
		}

		/// <summary>
		/// Removes a rate from the given user
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="rateId"></param>
		private void DeleteRate(int userId, int rateId) {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.DeleteRate,
					new ANWI.Messaging.ReqExp.TwoIDs(userId, rateId)));
		}

		/// <summary>
		/// Sets the primary rate of a given user
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="rateId"></param>
		private void SetPrimaryRate(int userId, int rateId) {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.SetPrimaryRate,
					new ANWI.Messaging.ReqExp.TwoIDs(userId, rateId)));
		}

		/// <summary>
		/// Changes the rank of a given user
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="rankId"></param>
		private void ChangeRank(int userId, int rankId) {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.ChangeRank,
					new ANWI.Messaging.ReqExp.TwoIDs(userId, rankId)));
		}

		/// <summary>
		/// Changes the name of a given user
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="name"></param>
		private void ChangeNickname(int userId, string name) {
			ANWI.Messaging.Message.Send(
				socket,
				ANWI.Messaging.Message.Routing.Main,
				new ANWI.Messaging.Request(
					ANWI.Messaging.Request.Type.ChangeName,
					new ANWI.Messaging.ReqExp.IdString(userId, name)));
		}
		#endregion

		/// <summary>
		/// Notifies the UI when a bound property changes
		/// </summary>
		/// <param name="name"></param>
		public void NotifyPropertyChanged(string name) {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		/// <summary>
		/// Writes crash dumps in the event of a fatal exception
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void UEHandler(object sender, 
			UnhandledExceptionEventArgs e) {
			if (e.IsTerminating) {
				ANWI.Utility.DumpWriter.MiniDumpToFile("crashdump.dmp");
				Application.Current.Shutdown();
			}
		}
	}
}
