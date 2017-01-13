using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI {
	/// <summary>
	/// An account which has been successfully authenticated by the server
	/// Contains the nickname of the account, an idToken so the server can recognize
	/// it, and a profile containing the user's information.
	/// </summary>
	public class AuthenticatedAccount {
		public string nickname;
		public string idToken;
		public Profile profile;
	}
}
