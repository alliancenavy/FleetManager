using System.Collections.Generic;
using ANWI.FleetComp;

namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Informs clients when the wings in the OOB have changed
	/// </summary>
	public class UpdateUnitsWings : IMessagePayload {
		public List<Wing> addedWings = null;
		public List<string> removedWings = null;

		public UpdateUnitsWings() {
		}

		public UpdateUnitsWings(List<Wing> added, List<string> removed) {
			addedWings = added;
			removedWings = removed;
		}

		public override string ToString() {
			return $"Type: UpdateUnitsWings. Added: {addedWings.Count} " +
				$"Removed: {removedWings.Count}";
		}
	}
}
