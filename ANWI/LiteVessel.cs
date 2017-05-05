using System;
using System.Collections.Generic;
using Datamodel = ANWI.Database.Model;
using ANWI.Database;

namespace ANWI {

	/// <summary>
	/// A lighter-weight version of a Vessel
	/// Does not include things like currently assigned personnel
	/// </summary>
	public class LiteVessel {
		#region Instance Members
		public int id;
		public string owner { get; set; }
		public string name { get; set; }
		public bool isLTI { get; set; }
		public int hullNumber { get; set; }
		public VesselStatus status { get; set; }

		// The hull this ship is using
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

		#region WPF Helpers
		public string fullHullNumber { get { return $"{hull.symbol}-{hullNumber}"; } }
		public string statusString { get { return status.ToFriendlyString(); } }
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

		/// <summary>
		/// Gets a vessel by ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static LiteVessel FetchById(int id) {
			Datamodel.UserShip s = null;
			if(Datamodel.UserShip.FetchById(ref s, id)) {
				return new LiteVessel(s);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets a vessel by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static LiteVessel FetchByName(string name) {
			Datamodel.UserShip s = null;
			if(Datamodel.UserShip.FetchByName(ref s, name)) {
				return new LiteVessel(s);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets the full list of registered vessels
		/// </summary>
		/// <returns></returns>
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
