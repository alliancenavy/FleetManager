using System.Collections.Generic;

namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Adds and removes positions in the fleet
	/// </summary>
	public class UpdatePositions : IMessagePayload {
		public List<OpPosition> added;
		public List<OpPosition> changed;
		public List<string> removed;

		public UpdatePositions() {
		}

		public UpdatePositions(List<OpPosition> add, List<OpPosition> change,
			List<string> remove) {
			added = add;
			changed = change;
			removed = remove;
		}

		public override string ToString() {
			return $"Type: UpdatePositions.";
		}
	}
}
