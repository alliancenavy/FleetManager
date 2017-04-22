using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class ConfirmProfileUpdated :IMessagePayload {
		public int userId;

		public ConfirmProfileUpdated() {
			userId = 0;
		}

		public ConfirmProfileUpdated(int id) {
			userId = id;
		}

		public override string ToString() {
			return $"Type: ConfirmProfileUpdated.  Id: {userId}";
		}
	}
}
