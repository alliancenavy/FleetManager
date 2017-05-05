namespace ANWI.Messaging {
	
	/// <summary>
	/// Client -> Server
	/// Terminates an assignment for a user
	/// </summary>
	public class EndAssignment : IMessagePayload {
		public int userId;
		public int assignmentId;
		public int shipId;

		public EndAssignment() {
			userId = 0;
			assignmentId = 0;
			shipId = 0;
		}

		public EndAssignment(int user, int assignment, int ship) {
			userId = user;
			assignmentId = assignment;
			shipId = ship;
		}

		public override string ToString() {
			return $"Type: EndAssignment. User: {userId} Assignment: {assignmentId}";
		}
	}
}
