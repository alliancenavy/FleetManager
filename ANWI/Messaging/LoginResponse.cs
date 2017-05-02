using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class LoginResponse : IMessagePayload {
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
