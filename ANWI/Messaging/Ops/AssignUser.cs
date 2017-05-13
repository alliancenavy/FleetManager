namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Client -> Server
	/// Assigns or unassigns a user from a position
	/// </summary>
	public class AssignUser : IMessagePayload {

		public string opUUID;
		public string elemUUID;
		public string wingmemberUUID;
		public string positionUUID;
		public int userId;

		public AssignUser() {
		}

		public AssignUser(string uuid) {
			opUUID = uuid;
		}

		public override string ToString() {
			return $"Type: AssignUser.";
		}
	}
}
