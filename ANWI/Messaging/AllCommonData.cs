using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class AllCommonData : IMessagePayload {
		public List<Rank> ranks = null;
		public List<Rate> rates = null;

		public AllCommonData() {
			ranks = null;
			rates = null;
		}

		public AllCommonData(List<Rank> rnk, List<Rate> rts) {
			ranks = rnk;
			rates = rts;
		}

		public override string ToString() {
			return "Type: AllCommonData";
		}
	}
}
