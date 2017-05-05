namespace ANWI.Messaging {

	/// <summary>
	/// Server -> Client
	/// Response to a login request.
	/// </summary>
	public class LoginResponse : IMessagePayload {
		/// <summary>
		/// Describes why the login attempt failed
		/// </summary>
		public enum Code {
			OK,
			FAILED_CREDENTIALS,
			FAILED_VERSION,
			FAILED_SERVER_ERROR,
			FAILED_OTHER
		}

		public Code code;
		public AuthenticatedAccount account;

		public LoginResponse() {
			code = Code.FAILED_OTHER;
			account = null;
		}

		public LoginResponse(Code c, AuthenticatedAccount acc) {
			code = c;
			account = acc;
		}

		public override string ToString() {
			return $"Type: LoginResponse. Code: {code}";
		}
	}
}
