using System.Collections.Generic;
using ANWI.FleetComp;

namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Informs clients when the ships in the OOB have changed
	/// </summary>
	public class UpdateUnitsShips : IMessagePayload {
		public List<Ship> addedShips = null;
		public List<string> removedShips = null;

		public UpdateUnitsShips() {
		}

		public UpdateUnitsShips(List<Ship> added, List<string> removed) {
			addedShips = added;
			removedShips = removed;
		}

		public override string ToString() {
			return $"Type: UpdateUnitsShips. Added: {addedShips.Count} " +
				$"Removed: {removedShips.Count}";
		}
	}
}
