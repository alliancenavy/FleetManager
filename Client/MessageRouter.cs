using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using ANWI;
using ANWI.Messaging;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;

namespace Client {
	public class MessageRouter {
		public event Action<CloseEventArgs> onMainClose;
		public event Action<ErrorEventArgs> onError;

		#region Instance Variables
		private static MessageRouter instance = null;

		private IMailbox splashScreen = null;
		private WebSocket updateSocket = null;

		private IMailbox loginWindow = null;
		private WebSocket authSocket = null;

		private WebSocket mainSocket = null;

		private IMailbox opsPushTarget = null;
		private WebSocket opsSocket = null;

		private int sequence = 0;
		private Dictionary<int, IMailbox> pendingResponses
			= new Dictionary<int, IMailbox>();
		#endregion

		#region Singleton
		public static MessageRouter Instance {
			get {
				if (instance == null) {
					instance = new MessageRouter();
				}

				return instance;
			}
		}
		#endregion

		#region Constructors
		private MessageRouter() {
			updateSocket = new WebSocket($"{CommonData.serverAddress}/update");
			updateSocket.OnMessage += OnUpdateMessage;
			updateSocket.OnError += OnError;

			authSocket = new WebSocket($"{CommonData.serverAddress}/auth");
			authSocket.OnMessage += OnAuthMessage;
			authSocket.OnError += OnAuthError;

			mainSocket = new WebSocket($"{CommonData.serverAddress}/main");
			mainSocket.OnMessage += OnMainMessage;
			mainSocket.OnClose += OnMainClose;
			mainSocket.OnError += OnError;

			opsSocket = new WebSocket($"{CommonData.serverAddress}/ops");
			opsSocket.OnMessage += OnOpsMessage;
			opsSocket.OnError += OnError;
		}
		#endregion

		#region Interface
		public void ConnectUpdate(Splash spl) {
			splashScreen = spl;
			updateSocket.SetCookie(
				new WebSocketSharp.Net.Cookie("name", Environment.MachineName)
				);
			updateSocket.Connect();

			if (!updateSocket.IsAlive)
				throw new ArgumentException("Could not connect to server");
		}

		public void DisconnectUpdate() {
			updateSocket.Close();
		}

		public void ConnectAuth(Login lWin) {
			loginWindow = lWin;
			authSocket.SetCookie(
				new WebSocketSharp.Net.Cookie("name", Environment.MachineName)
				);
			authSocket.Connect();
		}

		public void DisconnectAuth() {
			authSocket.Close();
		}

		public void ConnectMain(ANWI.AuthenticatedAccount account) {
			mainSocket.SetCookie(
				new WebSocketSharp.Net.Cookie("name", account.profile.nickname)
				);
			mainSocket.SetCookie(
				new WebSocketSharp.Net.Cookie("authtoken", account.authToken)
				);
			mainSocket.SetCookie(
				new WebSocketSharp.Net.Cookie("auth0id", account.auth0_id)
				);
			mainSocket.Connect();
		}

		public void ConnectOps(ANWI.AuthenticatedAccount account) {
			opsSocket.SetCookie(
				new WebSocketSharp.Net.Cookie("name", account.profile.nickname)
				);
			opsSocket.SetCookie(
				new WebSocketSharp.Net.Cookie("authtoken", account.authToken)
				);
			opsSocket.SetCookie(
				new WebSocketSharp.Net.Cookie("auth0id", account.auth0_id)
				);
			opsSocket.Connect();
		}

		public void DisconnectOps() {
			opsSocket.Close();
		}

		public void SendUpdate(IMessagePayload payload) {
			Message.Send(updateSocket, 0, payload);
		}

		public void SendAuth(IMessagePayload payload) {
			Message.Send(authSocket, 0, payload);
		}

		public void SendMain(IMessagePayload payload, IMailbox returnTo) {
			Message.Send(mainSocket, sequence, payload);
			if(returnTo != null) {
				pendingResponses.Add(sequence, returnTo);
			}

			sequence++;
		}

		public void SendOps(IMessagePayload payload, IMailbox returnTo) {
			Message.Send(opsSocket, sequence, payload);
			if (returnTo != null) {
				pendingResponses.Add(sequence, returnTo);
			}

			sequence++;
		}

		public void SetOpsPushTarget(IMailbox target) {
			opsPushTarget = target;
		}

		#endregion

		#region Socket Functions
		private void OnUpdateMessage(object sender, MessageEventArgs e) {
			if(splashScreen != null) {
				splashScreen.DeliverMessage(Message.Receive(e.RawData));
			}
		}

		private void OnAuthMessage(object sender, MessageEventArgs e) {
			if(loginWindow != null) {
				loginWindow.DeliverMessage(Message.Receive(e.RawData));
			}
		}

		private void OnAuthError(object sender, ErrorEventArgs e) {
			// TODO
		}

		private void OnMainMessage(object sender, MessageEventArgs e) {
			Message msg = Message.Receive(e.RawData);

			IMailbox returnTo = pendingResponses[msg.sequence];
			if(returnTo != null) {
				pendingResponses.Remove(msg.sequence);
				returnTo.DeliverMessage(msg);
			}
		}

		private void OnMainClose(object sender, CloseEventArgs c) {
			onMainClose?.Invoke(c);
		}

		private void OnOpsMessage(object sender, MessageEventArgs e) {
			Message msg = Message.Receive(e.RawData);

			if (msg.sequence >= 0) {
				IMailbox returnTo = pendingResponses[msg.sequence];
				if (returnTo != null) {
					pendingResponses.Remove(msg.sequence);
					returnTo.DeliverMessage(msg);
				}
			} else {
				if (opsPushTarget != null)
					opsPushTarget.DeliverMessage(msg);
			}
		}

		private void OnError(object sender, ErrorEventArgs e) {
			onError?.Invoke(e);
		}
		#endregion

		private void IncrementSequence() {
			sequence++;
			if (sequence >= 1000000)
				sequence = 0;
		}
	}
}
