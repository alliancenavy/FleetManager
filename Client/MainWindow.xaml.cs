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

namespace Client {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private readonly WebSocket socket;
		private AuthenticatedAccount account = null;
		private ServiceRecord serviceRecord = null;
		private VesselReg vesselRegister = null;

		/// <summary>
		/// Form constructor
		/// Opens a modal login window first.  When control returns it checks if there's
		/// a valid user and populates their profile info.
		/// </summary>
		public MainWindow() {
			InitializeComponent();
			
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

			// Open connection to the main service
			socket = new WebSocket("ws://localhost:9000/main");
			socket.OnMessage += OnMessage;
			socket.OnError += SocketError;
			socket.Connect();
		}

		/// <summary>
		/// Opens the service record for the logged-in member
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ServiceRecButton_Click(object sender, RoutedEventArgs e) {
			if (account != null && serviceRecord == null) {
				serviceRecord = new ServiceRecord(account.profile);
				serviceRecord.Closed += (s,args) => { serviceRecord = null; };
				serviceRecord.Show();
			}
		}

		/// <summary>
		/// Opens the vessel registry window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VesselRegButton_Click(object sender, RoutedEventArgs e) {
			if(vesselRegister == null) {
				vesselRegister = new VesselReg(socket);
				vesselRegister.Closed += (s, args) => { vesselRegister = null; };
				vesselRegister.Show();
			}
		}

		private void QuitButton_Click(object sender, RoutedEventArgs e) {
			Application.Current.Shutdown();
		}

		private void OnMessage(object sender, MessageEventArgs e) {
			ANWI.Messaging.Message msg = null;
			using (MemoryStream stream = new MemoryStream(e.RawData)) {
				msg = MessagePackSerializer.Get<ANWI.Messaging.Message>().Unpack(stream);
			}

			switch(msg.address.dest) {
				case ANWI.Messaging.Message.Routing.Target.ServiceRecord:
					serviceRecord.DeliverMessage(msg);
					break;

				case ANWI.Messaging.Message.Routing.Target.VesselReg:
					vesselRegister.DeliverMessage(msg);
					break;

				default:
					break;
			}
		}

		private void SocketError(object sender, WebSocketSharp.ErrorEventArgs e) {
			// TODO
		}
	}
}
