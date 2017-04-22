using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class AddRate : IMessagePayload {
		public int userId;
		public int rateId;
		public int rank;

		public AddRate() {
			userId = 0;
			rateId = 0;
			rank = 0;
		}

		public AddRate(int uid, int rid, int rnk) {
			userId = uid;
			rateId = rid;
			rank = rnk;
		}

		public override string ToString() {
			return $"Type: AddRate. User {userId}, Rate {rateId}, Rank {rank}";
		}
	}
}
