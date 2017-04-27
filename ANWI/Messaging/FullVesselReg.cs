using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
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
