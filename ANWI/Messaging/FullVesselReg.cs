using System.Collections.Generic;

namespace ANWI.Messaging {

	/// <summary>
	/// Server -> Client
	/// List of all ships
	/// </summary>
	public class FullVesselReg : IMessagePayload {
		public List<LiteVessel> vessels = null;

		public FullVesselReg() {
			vessels = null;
		}

		public FullVesselReg(List<LiteVessel> v) {
			vessels = v;
		}

		public override string ToString() {
			return "Type: FullVesselReg. Count: " + vessels.Count;
		}
	}
}
