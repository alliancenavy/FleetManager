using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
	public class Vessel {

		#region Instance Variables
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
				if(_hull == null) {
					_hull = Hull.FetchById(_hullId);
				}
				return _hull;
			}
			set { _hull = value; }
		}

		private List<LiteProfile> _shipsCompany = null;
		public List<LiteProfile> shipsCompany {
			get {
				if(_shipsCompany == null) {
					// TODO
				}
				return _shipsCompany;
			}
			set { _shipsCompany = value; }
		}

		private List<LiteProfile> _shipsEmbarked = null;
		public List<LiteProfile> shipsEmbarked {
			get {
				if(_shipsEmbarked == null) {
					// TODO
				}
				return _shipsEmbarked;
			}
			set { _shipsEmbarked = value; }
		}
		#endregion

		#region Constructors
		private Vessel() {
			id = 0;
			owner = "";
			name = "";
			isLTI = false;
			hullNumber = 0;
			status = VesselStatus.ACTIVE;
			_hullId = 0;
		}

		private Vessel(Datamodel.UserShip s) {
			id = s.id;
			name = s.name;
			isLTI = Convert.ToBoolean(s.insurance);
			hullNumber = s.number;
			status = (VesselStatus)s.status;

			Datamodel.User u = null;
			if (!Datamodel.User.FetchById(ref u, s.user))
				throw new ArgumentException("Ship does not have valid owner ID");
			owner = u.name;
		}

		public static Vessel FetchById(int id) {
			Datamodel.UserShip s = null;
			if(Datamodel.UserShip.FetchById(ref s, id)) {
				return new Vessel(s);
			} else {
				return null;
			}
		}

		public static Vessel FetchByName(string name) {
			Datamodel.UserShip s = null;
			if(Datamodel.UserShip.FetchByName(ref s, name)) {
				return new Vessel(s);
			} else {
				return null;
			}
		}
		#endregion
	}
}
