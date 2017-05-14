using System.Collections.Generic;
using ANWI.FleetComp;
using System;

namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Changes assignments on a member in the roster
	/// </summary>
	public class UpdateAssignments : IMessagePayload {
		public List<Tuple<int, string>> added;
		public List<int> removedByUser;
		public List<string> removedByUUID;

		public UpdateAssignments() {
		}

		public UpdateAssignments(List<Tuple<int, string>> added, 
			List<int> removedUser, List<string> removedUUID) {
			this.added = added;
			this.removedByUser = removedUser;
			this.removedByUUID = removedUUID;
		}

		public override string ToString() {
			return $"Type: UpdateAssignments.";
		}
	}
}
