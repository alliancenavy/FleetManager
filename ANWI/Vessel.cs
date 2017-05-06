using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;
using Datamodel = ANWI.Database.Model;
using ANWI.Database;

namespace ANWI {
	
	/// <summary>
	/// A full description of a large, named ship
	/// </summary>
	public class Vessel {

		#region Instance Variables
		public int id;
		public int ownerId { get; set; }
		public string ownerName { get; set; }
		public string name { get; set; }
		public bool isLTI { get; set; }
		public int hullNumber { get; set; }
		public VesselStatus status { get; set; }
		public DateTime statusDate;

		// The hull this ship uses
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

		// List of all users assigned to this ship as company
		private List<LiteProfile> _shipsCompany = null;
		public List<LiteProfile> shipsCompany {
			get {
				if(DBI.IsOpen() && _shipsCompany == null) {
					_shipsCompany = LiteProfile.FetchByAssignment(id, true);
				}
				return _shipsCompany;
			}
			set { _shipsCompany = value; }
		}

		// List of all users embarked on this ship
		private List<LiteProfile> _embarkedPersonnel = null;
		public List<LiteProfile> embarkedPersonnel {
			get {
				if(DBI.IsOpen() && _embarkedPersonnel == null) {
					_embarkedPersonnel 
						= LiteProfile.FetchByAssignment(id, false);
				}
				return _embarkedPersonnel;
			}
			set { _embarkedPersonnel = value; }
		}

		// List of all equipment embarked on this ship
		private List<ShipEquipment> _embarkedEquipment = null;
		public List<ShipEquipment> embarkedEquipment {
			get {
				if(DBI.IsOpen() && _embarkedEquipment == null) {
					_embarkedEquipment = ShipEquipment.FetchAllByShip(id);
				}
				return _embarkedEquipment;
			}
			set {
				_embarkedEquipment = value;
				totalEquipmentEmbarked = 0;
				foreach(ShipEquipment e in _embarkedEquipment) {
					totalEquipmentEmbarked += e.count;
				}
			}
		}
		[MessagePackIgnore]
		public int totalEquipmentEmbarked { get; private set; }
		#endregion

		#region WPF Helpers
		public string fullHullNumber { get {
				return $"{hull.symbol}-{hullNumber}"; } }
		public string detailName { get {
				return $"{fullHullNumber}: {name}"; } }
		public string detailType { get {
				return $"{hull.name} class {hull.role}"; } }
		public string statusString { get {
				return status.ToFriendlyString(); } }
		public string statusStringWithDate {
			get {
				return $"{status.ToFriendlyString()} as of {statusDateString}";
			} }
		public string statusDateString { get {
				return statusDate.ToString("dd MMM yyyy"); } }
		#endregion

		#region Constructors
		public Vessel() {
			id = 0;
			ownerId = 0;
			ownerName = "";
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
			statusDate = 
				DateTimeOffset.FromUnixTimeSeconds(s.statusDate).DateTime;
			_hullId = s.hull;

			Datamodel.User u = null;
			if (!Datamodel.User.FetchById(ref u, s.user))
				throw new ArgumentException(
					"Ship does not have valid owner ID");
			ownerId = u.id;
			ownerName = u.name;
		}

		/// <summary>
		/// Gets a vessel by ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static Vessel FetchById(int id) {
			Datamodel.UserShip s = null;
			if(Datamodel.UserShip.FetchById(ref s, id)) {
				return new Vessel(s);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets a vessel by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
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
