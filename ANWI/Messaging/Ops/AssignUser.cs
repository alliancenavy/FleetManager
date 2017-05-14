namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Client -> Server
	/// Assigns or unassigns a user from a position
	/// </summary>
	public class AssignUser : IMessagePayload {

		// Empty string positionUUID means unassign the given user
		// -1 as userId means unassign the given position
		public string opUUID;
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
