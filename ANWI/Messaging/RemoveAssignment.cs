﻿namespace ANWI.Messaging {
	
	/// <summary>
	/// Client -> Server
	/// Terminates an assignment for a user
	/// </summary>
	public class EndAssignment : IMessagePayload {
		public int userId;
		public int assignmentId;

		public EndAssignment() {
			userId = 0;
			assignmentId = 0;
		}

		public EndAssignment(int user, int assignment) {
			userId = user;
			assignmentId = assignment;
		}

		public override string ToString() {
			return $"Type: EndAssignment. User: {userId} Assignment: {assignmentId}";
		}
	}
}
