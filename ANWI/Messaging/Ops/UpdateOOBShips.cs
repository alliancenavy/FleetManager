using System.Collections.Generic;
using ANWI.FleetComp;

namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Informs clients when the ships in the OOB have changed
	/// </summary>
	public class UpdateOOBShips : IMessagePayload {
		public List<NamedShip> addedShips = null;
		public List<string> removedShips = null;

		public UpdateOOBShips() {
		}

		public UpdateOOBShips(List<NamedShip> added, List<string> removed) {
			addedShips = added;
			removedShips = removed;
		}

		public override string ToString() {
			return $"Type: UpdateOOBShips. Added: {addedShips.Count} " +
				$"Removed: {removedShips.Count}";
		}
	}
}
