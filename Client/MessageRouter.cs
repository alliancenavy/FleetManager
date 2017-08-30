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
		public enum Service {
			Update,
			Auth,
			Main,
			Ops
		}

		public event Action<Service> onOpen;
		public event Action<Service, CloseEventArgs> onClose;
		public event Action<Service, ErrorEventArgs> onError;

		#region Instance Variables
		private AuthenticatedAccount account = null;

		private static MessageRouter instance = null;

		private Dictionary<Service, WebSocket> sockets
			= new Dictionary<Service, WebSocket>();
		private Dictionary<object, Service> socketLookup
			= new Dictionary<object, Service>();

		private int sequence = 0;
		private Dictionary<int, IMailbox> pendingResponses
			= new Dictionary<int, IMailbox>();
		private Dictionary<string, IMailbox> pushSubscriptions
			= new Dictionary<string, IMailbox>();
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
			WebSocket updateSocket 
				= new WebSocket($"{CommonData.serverAddress}/update");
			updateSocket.OnMessage += OnMessage;
			updateSocket.OnOpen += OnOpen;
			updateSocket.OnClose += OnClose;
			updateSocket.OnError += OnError;
			updateSocket.SetCookie(
						new WebSocketSharp.Net.Cookie(
							"name", Environment.MachineName)
						);
			sockets.Add(Service.Update, updateSocket);
			socketLookup.Add(updateSocket, Service.Update);

			WebSocket authSocket 
				= new WebSocket($"{CommonData.serverAddress}/auth");
			authSocket.OnMessage += OnMessage;
			authSocket.OnOpen += OnOpen;
			authSocket.OnClose += OnClose;
			authSocket.OnError += OnError;
			authSocket.SetCookie(
						new WebSocketSharp.Net.Cookie(
							"name", Environment.MachineName)
						);
			sockets.Add(Service.Auth, authSocket);
			socketLookup.Add(authSocket, Service.Auth);

			WebSocket mainSocket 
				= new WebSocket($"{CommonData.serverAddress}/main");
			mainSocket.OnMessage += OnMessage;
			mainSocket.OnOpen += OnOpen;
			mainSocket.OnClose += OnClose;
			mainSocket.OnError += OnError;
			sockets.Add(Service.Main, mainSocket);
			socketLookup.Add(mainSocket, Service.Main);

			WebSocket opsSocket 
				= new WebSocket($"{CommonData.serverAddress}/ops");
			opsSocket.OnMessage += OnMessage;
			opsSocket.OnOpen += OnOpen;
			opsSocket.OnClose += OnClose;
			opsSocket.OnError += OnError;
			sockets.Add(Service.Ops, opsSocket);
			socketLookup.Add(opsSocket, Service.Ops);
		}
		#endregion

		#region Interface
		public void SetAccount(AuthenticatedAccount acct) {
			account = acct;

			// Set the appropriate cookies
			sockets[Service.Main].SetCookie(
						new WebSocketSharp.Net.Cookie(
							"name", account.profile.nickname));
			sockets[Service.Main].SetCookie(
				new WebSocketSharp.Net.Cookie(
					"authtoken", account.authToken));
			sockets[Service.Main].SetCookie(
				new WebSocketSharp.Net.Cookie(
					"auth0id", account.auth0_id));

			sockets[Service.Ops].SetCookie(
						new WebSocketSharp.Net.Cookie(
							"name", account.profile.nickname));
			sockets[Service.Ops].SetCookie(
				new WebSocketSharp.Net.Cookie(
					"authtoken", account.authToken));
			sockets[Service.Ops].SetCookie(
				new WebSocketSharp.Net.Cookie(
					"auth0id", account.auth0_id));
		}

		public void Connect(Service serv) {
			WebSocket socket = sockets[serv];
			socket.Connect();
		}

		public void Disconnect(Service serv) {
			WebSocket socket = sockets[serv];
			socket.Close();
		}

		public bool IsConnected(Service svc) {
			WebSocket sock = sockets[svc];
			return sock.IsAlive;
		}

		public void Send(Service serv, IMessagePayload payload, 
			IMailbox returnTo) {

			if (!IsConnected(serv))
				Connect(serv);

			int seq = GetSequence();
			Message.Send(sockets[serv], seq, payload);

			if(returnTo != null) {
				pendingResponses.Add(seq, returnTo);
			}
		}

		public void SubscribeSource(string source, IMailbox subscriber) {
			if(pushSubscriptions.ContainsKey(source)) {
				pushSubscriptions[source] = subscriber;
			} else {
				pushSubscriptions.Add(source, subscriber);
			}
		}

		public void UnsubscribeSource(string source) {
			if (pushSubscriptions.ContainsKey(source))
				pushSubscriptions.Remove(source);
		}
		#endregion

		#region Socket Functions
		private void OnMessage(object sender, MessageEventArgs e) {
			Message msg = Message.Receive(e.RawData);

			if (msg.sequence >= 0) {
				// Normal message
				IMailbox returnTo = null;
				if (pendingResponses.TryGetValue(msg.sequence, out returnTo)) {
					pendingResponses.Remove(msg.sequence);
					returnTo.DeliverMessage(msg);
				}
			} else {
				IMailbox sub = null;
				if (pushSubscriptions.TryGetValue(msg.source, out sub)) {
					sub.DeliverMessage(msg);
				}
			}
		}

		private void OnOpen(object sender, EventArgs e) {
			onOpen?.Invoke(socketLookup[sender]);
		}

		private void OnClose(object sender, CloseEventArgs e) {
			onClose?.Invoke(socketLookup[sender], e);
		}

		private void OnError(object sender, ErrorEventArgs e) {
			onError?.Invoke(socketLookup[sender], e);
		}
		#endregion

		private int GetSequence() {
			return sequence++;
		}
	}
}
