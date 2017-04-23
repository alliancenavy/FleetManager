using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class ChangeRank : IMessagePayload {
		public int userId;
		public int rankId;

		public ChangeRank() {
			userId = 0;
			rankId = 0;
		}

		public ChangeRank(int uid, int rid) {
			userId = uid;
			rankId = rid;
		}

		public override string ToString() {
			return $"Type: ChangeRank. User: {userId} Rank: {rankId}";
		}
	}
}
