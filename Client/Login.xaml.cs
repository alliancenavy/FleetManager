using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WebSocketSharp;
using FontAwesome;
using Newtonsoft.Json;
using ANWI;
using MsgPack;
using MsgPack.Serialization;
using System.IO;
using System.Diagnostics;

namespace Client {

	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login : Window {

		private WebSocket ws;

		public event Action<AuthenticatedAccount> returnuser;

		private Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
		public string versionString { get { return $"version {version}"; } }

		/// <summary>
		/// Form constructor.
		/// Grabs handles to all the important controls and builds
		/// the auth websocket
		/// </summary>
		public Login() {
			this.DataContext = this;
			InitializeComponent();
			Text_Failed.Visibility = Visibility.Hidden;
			Spinner.Visibility = Visibility.Hidden;

			// Set websocket callbacks
			ws = new WebSocket("ws://localhost:9000/auth");
			ws.OnMessage += OnMessage;
			ws.OnError += SocketError;
		}

		/// <summary>
		/// Starts the login process after the user clicks the button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Login_Click(object sender, RoutedEventArgs e) {
			// Clear previous errors
			Text_Failed.Visibility = Visibility.Hidden;

			StartWorking();

			ANWI.Credentials cred = new ANWI.Credentials();
			cred.username = Textbox_Username.Text;
			cred.password = Textbox_Password.Password;

			cred.clientVersion = version;

			// Attempt to log into the server
			LogIn(cred);
		}

		/// <summary>
		/// Used to move the login process off the UI thread.
		/// Connects to the auth websocket and sends the login credentials
		/// </summary>
		/// <param name="cred"></param>
		private async void LogIn(ANWI.Credentials cred) {
			ws.Connect();

			using (MemoryStream stream = new MemoryStream()) {
				MessagePackSerializer.Get<Credentials>().Pack(stream, cred);
				ws.Send(stream.ToArray());
			}
		}

		/// <summary>
		/// Called when a message is received from the websocket.
		/// If this login failed it tells the user and ends
		/// A successful login will return the user information to the main window
		/// and close the login window.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMessage(object sender, MessageEventArgs e) {
			ANWI.Messaging.Message msg = ANWI.Messaging.Message.Receive(e.RawData);
			if (msg.payload is ANWI.Messaging.LoginResponse) {
				ANWI.Messaging.LoginResponse resp = msg.payload as ANWI.Messaging.LoginResponse;

				if(resp.code == ANWI.Messaging.LoginResponse.Code.SUCCEEDED) {
					this.Dispatcher.Invoke(() => {
						EndWorkingSucceeded();
						ws.Close();
						returnuser(resp.account);
						this.Close();
					});
				} else {
					this.Dispatcher.Invoke(() => {
						EndWorkingFailed(GetFailedText(resp.code));
					});
				}
			} else {
				this.Dispatcher.Invoke(() => { EndWorkingFailed("Login Failed: Invalid Message Format"); });
			}
		}

		/// <summary>
		/// Called when there is a socket error.  Assumed some kind of login failure.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SocketError(object sender, WebSocketSharp.ErrorEventArgs e) {
			this.Dispatcher.Invoke(() => { EndWorkingFailed("Login Failed: Network Error"); });
		}

		private void StartWorking() {
			Button_Login.Visibility = Visibility.Hidden;
			Spinner.Visibility = Visibility.Visible;
			Text_Failed.Visibility = Visibility.Hidden;
		}

		private void EndWorkingSucceeded() {
			Button_Login.Visibility = Visibility.Visible;
			Spinner.Visibility = Visibility.Hidden;
			Text_Failed.Visibility = Visibility.Hidden;
		}

		private void EndWorkingFailed(string reason) {
			Button_Login.Visibility = Visibility.Visible;
			Spinner.Visibility = Visibility.Hidden;
			Text_Failed.Text = reason;
			Text_Failed.Visibility = Visibility.Visible;
		}

		private string GetFailedText(ANWI.Messaging.LoginResponse.Code c) {
			switch(c) {
				case ANWI.Messaging.LoginResponse.Code.FAILED_CREDENTIALS:
					return "Login Failed: Invalid Credentials";
				case ANWI.Messaging.LoginResponse.Code.FAILED_VERSION:
					return "Login Failed: Incompatible Version";
				case ANWI.Messaging.LoginResponse.Code.FAILED_SERVER_ERROR:
					return "Login Failed: Server Error";
				case ANWI.Messaging.LoginResponse.Code.FAILED_OTHER:
					return "Login Failed: Unknown Error";
				default:
					return "Login Failed";
			}
		}
	}
}
