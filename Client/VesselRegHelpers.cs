using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANWI;

namespace Client {
	namespace VesselRegHelpers {
		public class VesselRecord {
		}

		public class CategoryDivider : VesselRecord {
			public string text { get; set; }
		}

		public class NamedVessel : VesselRecord {
			public Vessel v;

			public string statusString { get { return v.statusString; } }
			public string owner { get { return v.owner; } }
			public string name { get { return v.name; } }
			public Vessel.VesselStatus status { get { return v.status; } }
			public bool isLTI { get { return v.isLTI; } }
			public int hullNumber { get { return v.hullNumber; } }
			public Hull hull { get { return v.wpfHull; } }
		}

		public class UnnamedVessel : VesselRecord {
			public struct Owner {
				public string name;
				public bool LTI;
			}

			public Vessel v;
			public List<Owner> owners;

			public string manufacturer { get { return v.hull.manufacturer; } }
			public string type { get { return v.hull.type; } }
			public string subtype { get { return v.hull.subtype; } }
			public int count { get { return owners.Count; } }
		}
	}
}
