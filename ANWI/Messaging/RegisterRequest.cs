using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class RegisterRequest : IMessagePayload {
		public Version clientVer;
		public string email;
		public string username;
		public string password;

		public RegisterRequest() {
			clientVer = null;
			email = "";
			username = "";
			password = "";
		}

		public RegisterRequest(Version v, string em, string uname, string pwd) {
			clientVer = v;
			email = em;
			username = uname;
			password = pwd;
		}

		public override string ToString() {
			return "Type: RegisterRequest.";
		}
	}
}
