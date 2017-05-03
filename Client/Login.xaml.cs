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
using Newtonsoft.Json.Linq;

namespace Client {

	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login : Window {

		private WebSocket ws;

		public event Action<AuthenticatedAccount> returnuser;

		private Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
		public string versionString { get { return $"version {version}"; } }

		private static readonly string credentialsFile = ".login.json";

		#region General
		/// <summary>
		/// Form constructor.
		/// Grabs handles to all the important controls and builds
		/// the auth websocket
		/// </summary>
		public Login() {
			this.DataContext = this;
			InitializeComponent();
			Text_Failed.Visibility = Visibility.Hidden;
			Spinner_Login.Visibility = Visibility.Hidden;
			Text_RegisterResult.Visibility = Visibility.Hidden;
			Spinner_Register.Visibility = Visibility.Hidden;

			// Set websocket callbacks
			ws = new WebSocket("ws://107.173.28.114:9000/auth");
			ws.OnMessage += OnMessage;
			ws.OnError += SocketError;

			// Load credentials
			if (File.Exists(credentialsFile)) {
				try {
					File.Decrypt(credentialsFile);
					StreamReader stream = File.OpenText(credentialsFile);
					JsonTextReader reader = new JsonTextReader(stream);
					JObject root = (JObject)JToken.ReadFrom(reader);

					Textbox_Username.Text = (string)root["username"];
					Textbox_Password.Password = (string)root["password"];
					Checkbox_RememberMe.IsChecked = true;
					stream.Close();
					File.Encrypt(credentialsFile);
				} catch (Exception e) {
					// Delete the credentials file so we don't fail again
					File.Delete(credentialsFile);
				}
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
				Login_Response(msg.payload as ANWI.Messaging.LoginResponse);
			} else if (msg.payload is ANWI.Messaging.RegisterResponse) {
				Register_Response(msg.payload as ANWI.Messaging.RegisterResponse);
			} else {
				Login_EndWorkingFailed("Login Failed: Invalid Message Format");
			}
		}

		/// <summary>
		/// Called when there is a socket error.  Assumed some kind of login failure.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SocketError(object sender, WebSocketSharp.ErrorEventArgs e) {
			Login_EndWorkingFailed("Login Failed: Network Error");
		}
		#endregion

		#region Login
		/// <summary>
		/// Starts the login process after the user clicks the button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Login_Click(object sender, RoutedEventArgs e) {
			SendLogin();
		}

		private void SendLogin() {
			if (Textbox_Username.Text != "" && Textbox_Password.Password != "") {
				ws.Connect();

				// Clear previous errors
				Text_Failed.Visibility = Visibility.Hidden;

				Login_StartWorking();

				ANWI.Messaging.Message.Send(
					ws,
					ANWI.Messaging.Message.Routing.NoReturn,
					new ANWI.Messaging.LoginRequest(
						version, Textbox_Username.Text, Textbox_Password.Password));

				// Save credentials
				if(Checkbox_RememberMe.IsChecked.Value) {
					try {
						JObject root = new JObject(
							new JProperty("username", Textbox_Username.Text),
							new JProperty("password", Textbox_Password.Password));

						File.WriteAllText(credentialsFile, root.ToString());
						File.Encrypt(credentialsFile);
					} catch(Exception e) {
						// Fail silently, just prevent crashes
					}
				}
			}
		}

		private void Login_Response(ANWI.Messaging.LoginResponse resp) {
			if (resp.code == ANWI.Messaging.LoginResponse.Code.OK) {
				Login_EndWorkingSucceeded();
				ws.Close();
				returnuser(resp.account);
				this.Dispatcher.Invoke(Close);
			} else {
				Login_EndWorkingFailed(Login_GetFailedText(resp.code));
			}
		}

		private void Login_StartWorking() {
			this.Dispatcher.Invoke(() => {
				Button_Login.Visibility = Visibility.Hidden;
				Spinner_Login.Visibility = Visibility.Visible;
				Text_Failed.Visibility = Visibility.Hidden;
			});
		}

		private void Login_EndWorkingSucceeded() {
			this.Dispatcher.Invoke(() => {
				Button_Login.Visibility = Visibility.Visible;
				Spinner_Login.Visibility = Visibility.Hidden;
				Text_Failed.Visibility = Visibility.Hidden;
			});
		}

		private void Login_EndWorkingFailed(string reason) {
			this.Dispatcher.Invoke(() => {
				Button_Login.Visibility = Visibility.Visible;
				Spinner_Login.Visibility = Visibility.Hidden;
				Text_Failed.Text = reason;
				Text_Failed.Visibility = Visibility.Visible;
			});
		}

		private string Login_GetFailedText(ANWI.Messaging.LoginResponse.Code c) {
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

		private void Textbox_Login_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Return) {
				SendLogin();
			}
		}
		#endregion

		private void Button_Register_Click(object sender, RoutedEventArgs e) {
			SendRegister();
		}

		private void SendRegister() {
			Register_StartWorking();

			if(Textbox_RegisterEmail.Text == "" || Textbox_RegisterNickname.Text == "" ||
				Textbox_RegisterPassword.Password == "" || Textbox_RegisterPassword2.Password == "") {
				return;
			}

			// Check that passwords match
			if (Textbox_RegisterPassword.Password != Textbox_RegisterPassword2.Password) {
				Register_EndWorkingFailed("Passwords do not match");
				return;
			}

			ws.Connect();

			ANWI.Messaging.Message.Send(
				ws,
				ANWI.Messaging.Message.Routing.NoReturn,
				new ANWI.Messaging.RegisterRequest(
					version,
					Textbox_RegisterEmail.Text,
					Textbox_RegisterNickname.Text,
					Textbox_RegisterPassword.Password));
		}

		private void Register_Response(ANWI.Messaging.RegisterResponse resp) {
			if (resp.code == ANWI.Messaging.RegisterResponse.Code.OK) {
				Register_EndWorkingSucceeded();
			} else {
				Register_EndWorkingFailed(Register_GetFailedText(resp.code));
			}
		}

		private string Register_GetFailedText(ANWI.Messaging.RegisterResponse.Code code) {
			switch(code) {
				case ANWI.Messaging.RegisterResponse.Code.FAILED_ALREADY_EXISTS:
					return "Registration Failed: Email Already In Use";

				case ANWI.Messaging.RegisterResponse.Code.FAILED_SERVER_ERROR:
					return "Registration Failed: Server Error";

				default:
					return "Registration Failed: Unknown Error";
			}
		}

		private void Register_StartWorking() {
			this.Dispatcher.Invoke(() => {
				Button_Register.Visibility = Visibility.Hidden;
				Spinner_Register.Visibility = Visibility.Visible;
				Text_RegisterResult.Visibility = Visibility.Hidden;
			});
		}

		private void Register_EndWorkingSucceeded() {
			this.Dispatcher.Invoke(() => {
				Button_Register.Visibility = Visibility.Visible;
				Spinner_Register.Visibility = Visibility.Hidden;
				Text_RegisterResult.Visibility = Visibility.Visible;
				Text_RegisterResult.Foreground = Brushes.Green;
				Text_RegisterResult.Text = "Account Registered: Please confirm your email before logging in.";
			});
		}

		private void Register_EndWorkingFailed(string reason) {
			this.Dispatcher.Invoke(() => {
				Button_Register.Visibility = Visibility.Visible;
				Spinner_Register.Visibility = Visibility.Hidden;
				Text_RegisterResult.Text = reason;
				Text_RegisterResult.Visibility = Visibility.Visible;
				Text_RegisterResult.Foreground = Brushes.Red;
			});
		}

		private void Textbox_Register_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Return)
				SendRegister();
		}

		private void Checkbox_RememberMe_Click(object sender, RoutedEventArgs e) {
			if(Checkbox_RememberMe.IsChecked.Value == false) {
				// Delete file
				File.Delete(credentialsFile);
			}
		}
	}
}
