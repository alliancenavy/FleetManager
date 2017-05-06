using ANWI;
using WebSocketSharp;
using WebSocketSharp.Net.WebSockets;

namespace FleetManager {

	/// <summary>
	/// Represents a user connected to the server
	/// </summary>
	public class ConnectedUser {

		public string token { get; private set; }
		public LiteProfile profile { get; private set; }
		public WebSocket socket { get; private set; }

		/// <summary>
		/// Constructs a new connected user from a connection context
		/// </summary>
		/// <param name="ctxt"></param>
		public ConnectedUser(WebSocketContext ctxt) {
			token = ctxt.CookieCollection["authtoken"].Value;
			profile = LiteProfile.FetchByAuth0(
				ctxt.CookieCollection["auth0id"].Value);
			socket = ctxt.WebSocket;
		}

		public void SendMessageTo(ANWI.Messaging.Message m) {
			ANWI.Messaging.Message.Send(socket, m);
		}
	}
}
