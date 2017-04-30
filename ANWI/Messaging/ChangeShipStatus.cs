using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class ChangeShipStatus : IMessagePayload {
		public int shipId;
		public VesselStatus status;

		public ChangeShipStatus() {
			shipId = 0;
			status = VesselStatus.ACTIVE;
		}

		public ChangeShipStatus(int ship, VesselStatus stat) {
			shipId = ship;
			status = stat;
		}

		public override string ToString() {
			return $"Type: ChangeShipStatus. Ship: {shipId} Status: {status}";
		}
	}
}
