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

namespace Client {

	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login : Window {
		
		private readonly TextBox username;
		private readonly PasswordBox password;
		private readonly Button button;
		private readonly FontAwesome.WPF.ImageAwesome spinner;
		private readonly TextBlock failed;

		private WebSocket ws;

		public event Action<AuthenticatedAccount> returnuser;

		/// <summary>
		/// Form constructor.
		/// Grabs handles to all the important controls and builds
		/// the auth websocket
		/// </summary>
		public Login() {
			InitializeComponent();

			// Fetch all of the components
			username = (TextBox)this.FindName("UnameBox");
			password = (PasswordBox)this.FindName("PswdBox");
			button = (Button)this.FindName("LoginButton");
			spinner = (FontAwesome.WPF.ImageAwesome)this.FindName("Spinner");
			failed = (TextBlock)this.FindName("failedtext");

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
		private void LoginButton_Click(object sender, RoutedEventArgs e) {
			// Clear previous errors
			failed.Visibility = Visibility.Hidden;

			StartWorking();

			ANWI.Credentials cred = new ANWI.Credentials();
			cred.username = username.Text;
			cred.password = password.Password;

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
			AuthenticatedAccount account = null;
			using (MemoryStream stream = new MemoryStream(e.RawData)) {
				account = MessagePackSerializer.Get<AuthenticatedAccount>().Unpack(stream);
			}

			// If the login failed the authToken will be an empty string
			if(account.authToken == "") {
				this.Dispatcher.Invoke(EndWorkingFailed);
			} else {
				this.Dispatcher.Invoke(() => {
					// Hand the account off to the main window and close this one
					EndWorkingSucceeded();
					ws.Close();
					returnuser(account);
					this.Close();
				});
			}
		}

		/// <summary>
		/// Called when there is a socket error.  Assumed some kind of login failure.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SocketError(object sender, WebSocketSharp.ErrorEventArgs e) {
			this.Dispatcher.Invoke(EndWorkingFailed);
		}

		private void StartWorking() {
			button.Visibility = Visibility.Hidden;
			spinner.Visibility = Visibility.Visible;
			failed.Visibility = Visibility.Hidden;
		}

		private void EndWorkingSucceeded() {
			button.Visibility = Visibility.Visible;
			spinner.Visibility = Visibility.Hidden;
			failed.Visibility = Visibility.Hidden;
		}

		private void EndWorkingFailed() {
			button.Visibility = Visibility.Visible;
			spinner.Visibility = Visibility.Hidden;
			failed.Visibility = Visibility.Visible;
		}
	}
}
