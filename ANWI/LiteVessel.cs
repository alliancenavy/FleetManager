using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;
using ANWI.Database;

namespace ANWI {
	public class LiteVessel {
		#region Instance Members
		public int id;
		public string owner { get; set; }
		public string name { get; set; }
		public bool isLTI { get; set; }
		public int hullNumber { get; set; }
		public VesselStatus status { get; set; }

		private int _hullId;
		private Hull _hull = null;
		public Hull hull {
			get {
				if(DBI.IsOpen() && _hull == null) {
					_hull = Hull.FetchById(_hullId);
				}
				return _hull;
			}
			set { _hull = value; }
		}
		#endregion

		#region Constructors
		public LiteVessel() {
			id = 0;
			owner = "";
			name = "";
			isLTI = false;
			hullNumber = 0;
			status = VesselStatus.ACTIVE;
		}

		private LiteVessel(Datamodel.UserShip s) {
			id = s.id;
			name = s.name;
			isLTI = Convert.ToBoolean(s.insurance);
			hullNumber = s.number;
			status = (VesselStatus)s.status;

			Datamodel.User u = null;
			if (!Datamodel.User.FetchById(ref u, s.user))
				throw new ArgumentException("Ship does not have valid owner ID");
			owner = u.name;

			_hullId = s.hull;
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
