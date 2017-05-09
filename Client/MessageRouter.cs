﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using ANWI;
using ANWI.Messaging;

namespace Client {
	public class MessageRouter {
		#region Instance Variables
		private static MessageRouter instance = null;

		private IMailbox loginWindow = null;
		private WebSocket authSocket = null;
		private WebSocket mainSocket = null;

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
			authSocket = new WebSocket($"{CommonData.serverAddress}/auth");
			authSocket.OnMessage += OnAuthMessage;
			authSocket.OnError += OnAuthError;

			mainSocket = new WebSocket($"{CommonData.serverAddress}/main");
			mainSocket.OnMessage += OnMainMessage;
			mainSocket.OnError += OnMainError; 
		}
		#endregion

		#region Interface
		public void ConnectAuth() {
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

		public void DisconnectMain() {
			mainSocket.Close();
		}

		public void SendAuth(IMessagePayload payload, IMailbox returnTo) {
			loginWindow = returnTo;
			Message.Send(authSocket, 0, payload);
		}

		public void SendMain(IMessagePayload payload, IMailbox returnTo) {
			Message.Send(mainSocket, sequence, payload);
			if(returnTo != null) {
				pendingResponses.Add(sequence, returnTo);
			}

			sequence++;
		}
		#endregion

		#region Socket Functions
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

		private void OnMainError(object sender, ErrorEventArgs e) {
			// TODO
		}
		#endregion
	}
}
