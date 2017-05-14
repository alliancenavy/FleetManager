using System.Collections.Generic;
using ANWI.FleetComp;

namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Informs clients when the boats in the OOB have changed
	/// </summary>
	public class UpdateUnitsBoats : IMessagePayload {
		public List<Boat> added = null;
		public List<string> removed = null;

		public UpdateUnitsBoats() {
		}

		public UpdateUnitsBoats(List<Boat> added, List<string> removed) {
			this.added = added;
			this.removed = removed;
		}

		public override string ToString() {
			return $"Type: UpdateUnitsBoats. Added: {added.Count} " +
				$"Removed: {removed.Count}";
		}
	}
}
