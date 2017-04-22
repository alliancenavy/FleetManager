using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class DeleteRate : IMessagePayload {
		public int userId;
		public int rateId;

		public DeleteRate() {
			userId = 0;
			rateId = 0;
		}

		public DeleteRate(int uid, int rid) {
			userId = uid;
			rateId = rid;
		}

		public override string ToString() {
			return $"Type: DeleteRate. Strike ID {rateId}";
		}
	}
}
