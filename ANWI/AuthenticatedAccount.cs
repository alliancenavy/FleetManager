namespace ANWI {
	/// <summary>
	/// An account which has been successfully authenticated by the server
	/// Contains the nickname of the account, an idToken so the server can recognize
	/// it, and a profile containing the user's information.
	/// </summary>
	public class AuthenticatedAccount {
		public string nickname;
		public string auth0_id;
		public string authToken;
		public Profile profile;
	}
}
