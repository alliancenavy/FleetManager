using System.Collections.Generic;

namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Informs clients when the roster changes
	/// </summary>
	public class UpdateRoster : IMessagePayload {
		public List<OpParticipant> addedUsers = null;
		public List<int> removedUsers = null;

		public UpdateRoster() {
		}

		public UpdateRoster(List<OpParticipant> added, List<int> removed) {
			addedUsers = added;
			removedUsers = removed;
		}

		public override string ToString() {
			return $"Type: UpdateRoster. Added: {addedUsers.Count} " +
				$"Removed: {removedUsers.Count}";
		}
	}
}
