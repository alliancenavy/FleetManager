using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;

namespace ANWI {
	public class Vessel {
		public enum VesselStatus {
			ACTIVE = 0,
			DESTROYED = 1,
			DESTROYED_WAITING_REPLACEMENT = 2,
			DRYDOCKED = 3

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

		public static Vessel FromDatamodel(ANWI.Database.Model.UserShip dmship) {
			Vessel v = new Vessel();

			v.id = dmship.id;
			v.owner = dmship.User.name;
			v.hull = Hull.FromDatamodel(dmship.Hull);
			v.name = dmship.name;
			v.isLTI = Convert.ToBoolean(dmship.insurance);
			v.hullNumber = dmship.number;
			v.status = (VesselStatus)dmship.status;

			return v;
		}
	}
}
