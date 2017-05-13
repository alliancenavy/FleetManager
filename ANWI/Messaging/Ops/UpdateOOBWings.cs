using System.Collections.Generic;
using ANWI.FleetComp;

namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Informs clients when the wings in the OOB have changed
	/// </summary>
	public class UpdateOOBWings : IMessagePayload {
		public List<Wing> addedWings = null;
		public List<string> removedWings = null;

		public UpdateOOBWings() {
		}

		public UpdateOOBWings(List<Wing> added, List<string> removed) {
			addedWings = added;
			removedWings = removed;
		}

		public override string ToString() {
			return $"Type: UpdateOOBWings. Added: {addedWings.Count} " +
				$"Removed: {removedWings.Count}";
		}
	}
}
