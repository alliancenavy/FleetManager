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

namespace Client {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private readonly WebSocket socket;
		private AuthenticatedAccount account = null;
		private ObservableCollection<VesselRecord> fleetList = new ObservableCollection<VesselRecord>();
		private ObservableCollection<Operation> operationList = new ObservableCollection<Operation>();

		#region Observable Members
		public Profile wpfProfile { get { return account.profile; } }
		public ObservableCollection<VesselRecord> wpfFleetList { get { return fleetList; } }
		public ObservableCollection<Operation> wpfOpList { get { return operationList; } }
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
			{
				// Assigned ship
				Profile p = account.profile;
				if (p.assignedShip == null) {
					Text_CurrentAssignment.Text = "No Current Assignment";
				} else {
					Text_CurrentAssignment.Text = p.assignedShip.name + " (" + p.assignedShip.hull.type
						+ " class " + p.assignedShip.hull.role + ")";
				}

				// TODO: Time in service
			}

			// Open connection to the main service
			socket = new WebSocket("ws://localhost:9000/main");
			socket.OnMessage += OnMessage;
			socket.OnError += SocketError;
			socket.Connect();

			FetchFleet();
			FetchOps();

		}
		#endregion

		#region Window Event Handlers
		private void QuitButton_Click(object sender, RoutedEventArgs e) {
			Application.Current.Shutdown();
		}

		private void Button_RefreshFleets_Click(object sender, RoutedEventArgs e) {
			FetchFleet();
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
		#endregion

		#region Sockets and Messaging
		private void OnMessage(object sender, MessageEventArgs e) {
			ANWI.Messaging.Message msg = ANWI.Messaging.Message.Receive(e.RawData);

			switch(msg.address.dest) {
				case ANWI.Messaging.Message.Routing.Target.Main:
					ProcessMessage(msg);
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

		/// <summary>
		/// Processes messages meant to be delivered to the main window
		/// </summary>
		/// <param name="m">Incomming message</param>
		private void ProcessMessage(ANWI.Messaging.Message m) {
			if(m.payload is ANWI.Messaging.FullVesselReg) {
				LoadFleet(m.payload as ANWI.Messaging.FullVesselReg);
			} else if(m.payload is ANWI.Messaging.FullOperationsList) {
				LoadOps(m.payload as ANWI.Messaging.FullOperationsList);
			}
		}

		/// <summary>
		/// Sends a request to the server for the full fleet registry
		/// </summary>
		private void FetchFleet() {
			// Clear the old list
			this.Dispatcher.Invoke(() => {
				fleetList.Clear();
				Spinner_Fleet.Visibility = Visibility.Visible;
			});

			// Send a request to the server
			ANWI.Messaging.Message.Send(
				socket, 
				new ANWI.Messaging.Message.Routing(ANWI.Messaging.Message.Routing.Target.Main, 0),
				new ANWI.Messaging.Request(ANWI.Messaging.Request.Type.GetFleet));
		}

		/// <summary>
		/// Response handler for the full fleet request.
		/// Populates the global fleet list and also the profile ship list for ships owned
		/// by the current user.
		/// </summary>
		/// <param name="fvr"></param>
		private void LoadFleet(ANWI.Messaging.FullVesselReg fvr) {
			this.Dispatcher.Invoke(() => {
				fleetList.Clear();
				Spinner_Fleet.Visibility = Visibility.Hidden;
			});

			// Sort the records by their ordering
			fvr.vessels.Sort((a, b) => {
				if (a.hull.ordering < b.hull.ordering)
					return -1;
				else if (a.hull.ordering == b.hull.ordering)
					return 0;
				else return 1;
			});

			// Insert each vessel into the list
			foreach(Vessel vessel in fvr.vessels) {
				// For now 100 will be the boundary between named and unnamed vessels
				if(vessel.hull.ordering < 100) {
					NamedVessel vr = new NamedVessel();
					vr.v = vessel;
					this.Dispatcher.Invoke(() => { fleetList.Add(vr); });
				} else {
					// TODO
				}
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
				new ANWI.Messaging.Message.Routing(ANWI.Messaging.Message.Routing.Target.Main, 0),
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
		#endregion
	}
}
