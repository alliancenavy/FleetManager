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
			if(ctxt.CookieCollection["authtoken"] != null)
				token = ctxt.CookieCollection["authtoken"].Value;

			if(ctxt.CookieCollection["auth0id"] != null)
				profile = LiteProfile.FetchByAuth0(
					ctxt.CookieCollection["auth0id"].Value);

			socket = ctxt.WebSocket;
		}

		/// <summary>
		/// FOR TESTING PURPOSES ONLY
		/// Spoofing connected users for adding to ops
		/// </summary>
		/// <param name="token"></param>
		/// <param name="userID"></param>
		public ConnectedUser(string token, int userID) {
			this.token = token;
			this.profile = LiteProfile.FetchById(userID);
			socket = null;
		}

		public void SendMessageTo(ANWI.Messaging.Message m) {
			ANWI.Messaging.Message.Send(socket, m);
		}
	}
}
