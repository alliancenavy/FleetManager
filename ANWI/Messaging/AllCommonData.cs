using System.Collections.Generic;

namespace ANWI.Messaging {

	/// <summary>
	/// Server -> Client
	/// Returns all of the static data such as ranks, rates, etc
	/// </summary>
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
