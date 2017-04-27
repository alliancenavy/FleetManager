using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI {
	public enum VesselStatus {
		ACTIVE = 0,
		DESTROYED = 1,
		DESTROYED_WAITING_REPLACEMENT = 2,
		DRYDOCKED = 3


			/*public string statusString {
		get {
			switch (status) {
				case VesselStatus.ACTIVE:
					return "Active";
				case VesselStatus.DESTROYED:
					return "Destroyed";
				case VesselStatus.DESTROYED_WAITING_REPLACEMENT:
					return "Destroyed (Awaiting Replacement)";
				case VesselStatus.DRYDOCKED:
					return "Drydocked";
				default:
					return "Unknown";
			}
		}*/
	}
}
