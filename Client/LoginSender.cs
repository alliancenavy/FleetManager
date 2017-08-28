using ANWI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client {

	/// <summary>
	/// Sends the set credentials to the login service
	/// </summary>
	class LoginSender {

		private bool credSet = false;
		private string username = "";
		private string password = "";

		public LoginSender() {

		}

		public void SetCredentials(string username, string password) {
			this.username = username;
			this.password = password;
			credSet = true;
		}

		public AuthenticatedAccount Submit() {
			if (!credSet)
				return null;

			return null;
		}
	}
}
