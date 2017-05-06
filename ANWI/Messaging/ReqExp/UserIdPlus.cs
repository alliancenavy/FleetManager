namespace ANWI.Messaging.ReqExp {

	/// <summary>
	/// Request expounding detail
	/// Many requests require a user ID plus the ID of another object
	/// </summary>
	public class TwoIDs : IRequestDetail {
		public int id1;
		public int id2;

		public TwoIDs() {
			id1 = 0;
			id2 = 0;
		}

		public TwoIDs(int uid, int oid) {
			id1 = uid;
			id2 = oid;
		}
	}
}
