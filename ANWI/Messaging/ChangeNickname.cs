using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class ChangeNickname : IMessagePayload {
		public string auth0_id;
		public string newName;

		public ChangeNickname() {
			auth0_id = "";
			newName = "";
		}

		public ChangeNickname(string id, string nick) {
			auth0_id = id;
			newName = nick;
		}

		public override string ToString() {
			return $"Type: ChangeNickname. Id: {auth0_id}. New Name: {newName}";
		}
	}
}
