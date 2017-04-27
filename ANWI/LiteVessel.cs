using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
	public class LiteVessel {
		#region Instance Members
		public int id;
		public string owner { get; set; }
		public string name { get; set; }
		public bool isLTI { get; set; }
		public string hullNumber { get; set; }
		public string hullDesc { get; set; }
		public VesselStatus status { get; set; }
		#endregion

		#region Constructors
		private LiteVessel() {
			id = 0;
			owner = "";
			name = "";
			isLTI = false;
			hullNumber = "";
			hullDesc = "";
			status = VesselStatus.ACTIVE;
		}

		private LiteVessel(Datamodel.UserShip s) {
			id = s.id;
			name = s.name;
			isLTI = Convert.ToBoolean(s.insurance);
			status = (VesselStatus)s.status;

			Datamodel.User u = null;
			if (!Datamodel.User.FetchById(ref u, s.user))
				throw new ArgumentException("Ship does not have valid owner ID");
			owner = u.name;

			Datamodel.Hull h = null;
			if (!Datamodel.Hull.FetchById(ref h, s.hull))
				throw new ArgumentException("Ship does not have valid hull ID");
			hullNumber = $"{h.symbol}-{s.number}";
			hullDesc = $"{h.series} class {h.role}";
		}

		public static LiteVessel FetchById(int id) {
			Datamodel.UserShip s = null;
			if(Datamodel.UserShip.FetchById(ref s, id)) {
				return new LiteVessel(s);
			} else {
				return null;
			}
		}

		public static LiteVessel FetchByName(string name) {
			Datamodel.UserShip s = null;
			if(Datamodel.UserShip.FetchByName(ref s, name)) {
				return new LiteVessel(s);
			} else {
				return null;
			}
		}

		public static List<LiteVessel> FetchRegistry() {
			List<Datamodel.UserShip> s = null;
			if(Datamodel.UserShip.FetchRegistry(ref s)) {
				return s.ConvertAll<LiteVessel>((a) => { return new LiteVessel(a); });
			} else {
				return null;
			}
		}
		#endregion
	}
}
