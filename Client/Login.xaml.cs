using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WebSocketSharp;
using Newtonsoft.Json;
using ANWI;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Client {

	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login : MailboxWindow {

		// Subscribe to receive the user login result
		public event Action<AuthenticatedAccount> returnuser;

		// Version of the client
		private Version version 
			= System.Reflection.Assembly.GetExecutingAssembly().
				GetName().Version;
		public string versionString { get { return $"version {version}"; } }

		// Name of file to store remember-me credentials
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

			this.AddProcessor(typeof(ANWI.Messaging.LoginResponse),
				Login_Response);
			this.AddProcessor(typeof(ANWI.Messaging.RegisterResponse),
				Register_Response);

			
			// Load credentials
			if (Appdata.CheckFileExists(credentialsFile)) {
				try {
					JObject root
						= Appdata.OpenJSONFileEncrypted(credentialsFile);
					

					Textbox_Username.Text = (string)root["username"];
					Textbox_Password.Password = (string)root["password"];
					Checkbox_RememberMe.IsChecked = true;
				} catch (Exception e) {
					// Delete the credentials file so we don't fail again
					Appdata.DeleteFile(credentialsFile);
				}
			}
		}
		
		/// <summary>
		/// Called when there is a socket error.  Assumed some kind of 
		/// login failure.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SocketError(object sender, 
			WebSocketSharp.ErrorEventArgs e) {
			Login_EndWorkingFailed("Network Error");
			Register_EndWorkingFailed("Network Error");
		}
		#endregion

		#region Login
		/// <summary>
		/// Starts the login process after the user clicks the button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Login_Click(object sender, RoutedEventArgs e) {
			Login_StartWorking();

			string uname = Textbox_Username.Text;
			string pass = Textbox_Password.Password;
			bool remember = Checkbox_RememberMe.IsChecked.Value;
			Task t = new Task(() => {
				SendLogin(uname, pass, remember);
			});
			t.Start();
		}

		/// <summary>
		/// Sends the login message to the server
		/// Also records credentials to file if checkbox is clicked
		/// </summary>
		/// <param name="uname"></param>
		/// <param name="pass"></param>
		/// <param name="remember"></param>
		private void SendLogin(string uname, string pass, bool remember) {
			// Make sure fields have text
			if (uname != "" && pass != "") {
				MessageRouter.Instance.ConnectAuth(this);

				// Send message to login server
				MessageRouter.Instance.SendAuth(
					new ANWI.Messaging.LoginRequest(version, uname, pass)
					);

				// Save credentials
				if(remember) {
					try {
						JObject root = new JObject(
							new JProperty("username", uname),
							new JProperty("password", pass));

						Appdata.WriteFileEncrypted(
							credentialsFile, root.ToString());
					} catch(Exception e) {
						// Fail silently, just prevent crashes
					}
				}
			}
		}

		/// <summary>
		/// If the login was successful this closes the window and returns
		/// the logged in user.
		/// If login failed it keeps the window opened
		/// </summary>
		/// <param name="resp"></param>
		private void Login_Response(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.LoginResponse resp 
				= p as ANWI.Messaging.LoginResponse;

			if (resp.code == ANWI.Messaging.LoginResponse.Code.OK) {
				Login_EndWorkingSucceeded();
				MessageRouter.Instance.DisconnectAuth();
				returnuser(resp.account);
				this.Dispatcher.Invoke(Close);
			} else {
				Login_EndWorkingFailed(Login_GetFailedText(resp.code));
			}
		}

		/// <summary>
		/// Hides the failure text and shows the spinning gear
		/// </summary>
		private void Login_StartWorking() {
			this.Dispatcher.Invoke(() => {
				Text_Failed.Visibility = Visibility.Hidden;
				Button_Login.Visibility = Visibility.Hidden;
				Spinner_Login.Visibility = Visibility.Visible;
				Text_Failed.Visibility = Visibility.Hidden;
			});
		}

		/// <summary>
		/// Hides the working spinner and shows the button again
		/// </summary>
		private void Login_EndWorkingSucceeded() {
			this.Dispatcher.Invoke(() => {
				Button_Login.Visibility = Visibility.Visible;
				Spinner_Login.Visibility = Visibility.Hidden;
				Text_Failed.Visibility = Visibility.Hidden;
			});
		}

		/// <summary>
		/// Hides the spinner and shows the button and error text
		/// </summary>
		/// <param name="reason"></param>
		private void Login_EndWorkingFailed(string reason) {
			this.Dispatcher.Invoke(() => {
				Button_Login.Visibility = Visibility.Visible;
				Spinner_Login.Visibility = Visibility.Hidden;
				Text_Failed.Text = reason;
				Text_Failed.Visibility = Visibility.Visible;
			});
		}

		/// <summary>
		/// Writes a failure message based on the result code.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		private string Login_GetFailedText(
			ANWI.Messaging.LoginResponse.Code c) {
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

		/// <summary>
		/// Capture enter key so user doesn't need to click the button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Textbox_Login_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Return) {
				Login_StartWorking();

				string uname = Textbox_Username.Text;
				string pass = Textbox_Password.Password;
				bool remember = Checkbox_RememberMe.IsChecked.Value;
				Task t = new Task(() => {
					SendLogin(uname, pass, remember);
				});
				t.Start();
			}
		}

		/// <summary>
		/// Deletes the credentials file if unchecking.
		/// Does not save the credentials on checking, wait for login attempt
		/// to do that.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Checkbox_RememberMe_Click(object sender, 
			RoutedEventArgs e) {
			if (Checkbox_RememberMe.IsChecked.Value == false) {
				// Delete file
				File.Delete(credentialsFile);
			}
		}
		#endregion

		#region Registration
		/// <summary>
		/// Send registration message
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Register_Click(object sender, RoutedEventArgs e) {
			SendRegister();
		}

		/// <summary>
		/// Sends a registration message to the server
		/// </summary>
		private void SendRegister() {
			Register_StartWorking();

			// Check that all fields are filled in
			if(Textbox_RegisterEmail.Text == "" || 
				Textbox_RegisterNickname.Text == "" ||
				Textbox_RegisterPassword.Password == "" || 
				Textbox_RegisterPassword2.Password == "") {
				return;
			}

			// Check that passwords match
			if (Textbox_RegisterPassword.Password 
				!= Textbox_RegisterPassword2.Password) {
				Register_EndWorkingFailed("Passwords do not match");
				return;
			}

			MessageRouter.Instance.ConnectAuth(this);

			// Send message to server
			MessageRouter.Instance.SendAuth(
				new ANWI.Messaging.RegisterRequest(
					version,
					Textbox_RegisterEmail.Text,
					Textbox_RegisterNickname.Text,
					Textbox_RegisterPassword.Password)
				);
		}

		/// <summary>
		/// Fills in appropriate text based on code
		/// </summary>
		/// <param name="resp"></param>
		private void Register_Response(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.RegisterResponse resp
				= p as ANWI.Messaging.RegisterResponse;

			if (resp.code == ANWI.Messaging.RegisterResponse.Code.OK) {
				Register_EndWorkingSucceeded();
			} else {
				Register_EndWorkingFailed(Register_GetFailedText(resp.code));
			}
		}

		/// <summary>
		/// Turns error code into useful error message.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		private string Register_GetFailedText(
			ANWI.Messaging.RegisterResponse.Code code) {
			switch(code) {
				case ANWI.Messaging.RegisterResponse.Code.FAILED_ALREADY_EXISTS:
					return "Registration Failed: Email Already In Use";

				case ANWI.Messaging.RegisterResponse.Code.FAILED_SERVER_ERROR:
					return "Registration Failed: Server Error";

				default:
					return "Registration Failed: Unknown Error";
			}
		}

		/// <summary>
		/// Show spinner and hides button
		/// </summary>
		private void Register_StartWorking() {
			this.Dispatcher.Invoke(() => {
				Button_Register.Visibility = Visibility.Hidden;
				Spinner_Register.Visibility = Visibility.Visible;
				Text_RegisterResult.Visibility = Visibility.Hidden;
			});
		}

		/// <summary>
		/// Hides spinner and shows success message
		/// </summary>
		private void Register_EndWorkingSucceeded() {
			this.Dispatcher.Invoke(() => {
				Button_Register.Visibility = Visibility.Visible;
				Spinner_Register.Visibility = Visibility.Hidden;
				Text_RegisterResult.Visibility = Visibility.Visible;
				Text_RegisterResult.Foreground = Brushes.Green;
				Text_RegisterResult.Text
					= "Account Registered: Please confirm your email " +
					"before logging in.";
			});
		}

		/// <summary>
		/// Hides spinner and shows error message
		/// </summary>
		/// <param name="reason"></param>
		private void Register_EndWorkingFailed(string reason) {
			this.Dispatcher.Invoke(() => {
				Button_Register.Visibility = Visibility.Visible;
				Spinner_Register.Visibility = Visibility.Hidden;
				Text_RegisterResult.Text = reason;
				Text_RegisterResult.Visibility = Visibility.Visible;
				Text_RegisterResult.Foreground = Brushes.Red;
			});
		}

		/// <summary>
		/// Capture enter key so user doesn't need to click the register button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Textbox_Register_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Return)
				SendRegister();
		}
		#endregion
	}
}
