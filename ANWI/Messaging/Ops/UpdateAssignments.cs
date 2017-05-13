using System.Collections.Generic;
using ANWI.FleetComp;
using System;

namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Changes assignments on a member in the roster
	/// </summary>
	public class UpdateAssignments : IMessagePayload {
		public class AssignTo {
			public int userId;
			public string shipUUID;
			public string posUUID;
		}

		public List<AssignTo> added;
		public List<int> removed;

		public UpdateAssignments() {
		}

		public UpdateAssignments(List<AssignTo> added, List<int> removed) {
			this.added = added;
			this.removed = removed;
		}

		public override string ToString() {
			return $"Type: UpdateAssignments.";
		}
	}
}
