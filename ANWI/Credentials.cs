using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI {
	/// <summary>
	/// Login credentials for an account
	/// </summary>
    public struct Credentials {
		public Version clientVersion;
		public string username;
		public string password;
	}
}
