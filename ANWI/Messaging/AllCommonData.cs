using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class AllCommonData : IMessagePayload {
		public List<Rank> ranks = null;
		public List<Rate> rates = null;
		public List<AssignmentRole> assignmentRoles = null;
		public List<Hull> largeHulls = null;
		public List<Hull> smallHulls = null;

		public AllCommonData() {
			ranks = null;
			rates = null;
			assignmentRoles = null;
			largeHulls = null;
		}

		public override string ToString() {
			return "Type: AllCommonData";
		}
	}
}
