using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class RegisterResponse : IMessagePayload {
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
