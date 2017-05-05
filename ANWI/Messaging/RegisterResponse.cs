namespace ANWI.Messaging {

	/// <summary>
	/// Server -> Client
	/// Response to a registration request
	/// </summary>
	public class RegisterResponse : IMessagePayload {
		/// <summary>
		/// Describes the reason for a registration failure
		/// </summary>
		public enum Code {
			OK,
			FAILED_ALREADY_EXISTS,
			FAILED_SERVER_ERROR,
			FAILED_OTHER
		}

		public Code code;

		public RegisterResponse() {
			code = Code.OK;
		}

		public RegisterResponse(Code c) {
			code = c;
		}

		public override string ToString() {
			return $"Type: RegisterResponse. Code: {code}";
		}
	}
}
