using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class FullVesselDetails : IMessagePayload {
		public VesselDetails details;

		public FullVesselDetails() {
			details = null;
		}

		public FullVesselDetails(VesselDetails v) {
			details = v;
		}

		public override string ToString() {
			return "Type: FullVesselDetails";
		}
	}
}
