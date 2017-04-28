using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANWI;

namespace Client {
	namespace VesselRegHelpers {
		public class VesselRecord {
			public bool isEditable { get; set; }
		}

		public class CategoryDivider : VesselRecord {
			public string text { get; set; }
		}

		public class NamedVessel : VesselRecord {
			public LiteVessel v;

			public int id { get { return v.id; } }
			public string statusString { get { return v.status.ToString(); } }
			public string owner { get { return v.owner; } }
			public string name { get { return v.name; } }
			public VesselStatus status { get { return v.status; } }
			public bool isLTI { get { return v.isLTI; } }
			public int hullNumber { get { return v.hullNumber; } }
			public Hull hull { get { return v.hull; } }

			//public string DetailName { get { return $"{hull.symbol}-{hullNumber}: {name}"; } }
			//public string DetailType { get { return $"{hull.type} class {hull.role}"; } }
		}

		public class UnnamedVessel : VesselRecord {
			public struct Owner {
				public string name;
				public bool LTI;
			}

			public Vessel v;
			public List<Owner> owners;

			public string manufacturer { get { return v.hull.manufacturer; } }
			public string type { get { return v.hull.name; } }
			public int count { get { return owners.Count; } }
		}
	}
}
