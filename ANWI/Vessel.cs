using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI {
	public class Vessel {
		public enum VesselStatus {
			ACTIVE,
			DESTROYED,
			DESTROYED_WAITING_REPLACEMENT,
			DRYDOCKED

		}

		public int id;
		public string owner { get; set; }
		public Hull hull;
		public string name { get; set; }
		public bool isLTI { get; set; }
		public int hullNumber { get; set; }
		public VesselStatus status { get; set; }

		public Hull wpfHull { get { return hull; } }

		public string statusString {
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
			}
		}
	}
}
