using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class FullVessel : IMessagePayload {
		public Vessel vessel;

		public FullVessel() {
			vessel = null;
		}

		public FullVessel(Vessel v) {
			vessel = v;
		}

		public override string ToString() {
			return "Type: FullVessel";
		}
	}
}
