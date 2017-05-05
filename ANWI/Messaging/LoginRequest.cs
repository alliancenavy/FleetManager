using System;

namespace ANWI.Messaging {

	/// <summary>
	/// Client -> Server
	/// Login credentials for a user
	/// </summary>
	public class LoginRequest : IMessagePayload {
		public Version clientVer;
		public string username;
		public string password;

		public LoginRequest() {
			clientVer = null;
			username = "";
			password = "";
		}

		public LoginRequest(Version v, string uname, string pwd) {
			clientVer = v;
			username = uname;
			password = pwd;
		}

		public override string ToString() {
			return "Type: LoginRequest.";
		}
	}
}
