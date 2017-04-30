using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class NewAssignment : IMessagePayload {
		public int userId;
		public int shipId;
		public int roleId;

		public NewAssignment() {
			userId = 0;
			shipId = 0;
			roleId = 0;
		}

		public NewAssignment(int user, int ship, int role) {
			userId = user;
			shipId = ship;
			roleId = role;
		}

		public override string ToString() {
			return $"Type: NewAssignment. User: {userId} Ship: {shipId} Role: {roleId}";
		}
	}
}
