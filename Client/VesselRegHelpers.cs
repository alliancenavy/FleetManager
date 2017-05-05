using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANWI;

namespace Client {
	namespace VesselRegHelpers {

		/// <summary>
		/// Base class for the vessel registry list
		/// </summary>
		public class VesselRecord {
			public bool isEditable { get; set; }
		}

		public class CategoryDivider : VesselRecord {
			public string text { get; set; }
		}

		/// <summary>
		/// A named, large vessel in the fleet registry
		/// </summary>
		public class NamedVessel : VesselRecord {
			public LiteVessel v;

			public int id { get { return v.id; } }
			public string statusString {
				get { return v.status.ToFriendlyString(); }
			}
			public string owner { get { return v.owner; } }
			public string name { get { return v.name; } }
			public VesselStatus status { get { return v.status; } }
			public bool isLTI { get { return v.isLTI; } }
			public int hullNumber { get { return v.hullNumber; } }
			public Hull hull { get { return v.hull; } }
			
			public string fullHullNumber { get { return v.fullHullNumber; } }
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
