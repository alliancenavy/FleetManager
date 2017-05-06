namespace ANWI.Messaging.ReqExp {

	/// <summary>
	/// Request expounding detail
	/// Many requests require a user ID plus the ID of another object
	/// </summary>
	public class UserIdPlus : IRequestDetail {
		public int userId;
		public int otherId;

		public UserIdPlus() {
			userId = 0;
			otherId = 0;
		}

		public UserIdPlus(int uid, int oid) {
			userId = uid;
			otherId = oid;
		}
	}
}
