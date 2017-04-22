using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class SetPrimaryRate : IMessagePayload {
		public int userId;
		public int rateId;

		public SetPrimaryRate() {
			userId = 0;
			rateId = 0;
		}

		public SetPrimaryRate(int uid, int rid) {
			userId = uid;
			rateId = rid;
		}

		public override string ToString() {
			return $"Type: SetPrimaryRate. User: {userId} Rate: {rateId}";
		}
	}
}
