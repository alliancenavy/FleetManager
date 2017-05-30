using System.Collections.Generic;
using Datamodel = ANWI.Database.Model;
using MsgPack.Serialization;
using ANWI.Database;

namespace ANWI {

	/// <summary>
	/// A lighter-weight version of a user's profile.
	/// Does not contain the big lists of rates, assignments, etc
	/// </summary>
	public class LiteProfile {
		#region Instance Variables
		public int id { get; set; }
		public string auth0;
		public string nickname { get; set; }

		// The user's rank
		private int _rankId;
		private Rank _rank = null;
		public Rank rank {
			get {
				if(DBI.IsOpen() && _rank == null) {
					_rank = Rank.FetchById(_rankId);
				}
				return _rank;
			}
			set { _rank = value; }
		}

		// The user's primary rate
		private int _primaryRateId;
		private Rate _primaryRate = null;
		public Rate primaryRate {
			get {
				if(DBI.IsOpen() && _primaryRate == null) {
					_primaryRate = Rate.FetchUsersRate(id, _primaryRateId);
				}
				return _primaryRate;
			}
			set { _primaryRate = value; }
		}

		// The user's current assignment
		private Assignment _assignment = null;
		public Assignment assignment {
			get {
				if(DBI.IsOpen() && _assignment == null) {
					_assignment = Assignment.FetchCurrentAssignment(id);
				}
				return _assignment;
			}
			set { _assignment = value; }
		}

		// Helper variable for the client to record if this profile is the
		// user running the program.
		[MessagePackIgnore]
		public bool isMe { get; set; } = false;
		#endregion

		#region WPF Helpers
		public string fullName { get { return rank.abbrev + " " + nickname; } }
		#endregion

		#region Constructors
		public LiteProfile() {
			id = 0;
			auth0 = "";
			nickname = "";
			_rankId = 0;
			_primaryRateId = 0;
		}

		private LiteProfile(Datamodel.User u) {
			id = u.id;
			auth0 = u.auth0;
			nickname = u.name;
			_rankId = u.rank;
			_primaryRateId = u.rate;
		}

		/// <summary>
		/// Gets the profile by ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static LiteProfile FetchById(int id) {
			Datamodel.User u = null;
			if(Datamodel.User.FetchById(ref u, id)) {
				return new LiteProfile(u);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets a profile by the Auth0 ID
		/// </summary>
		/// <param name="auth0"></param>
		/// <returns></returns>
		public static LiteProfile FetchByAuth0(string auth0) {
			Datamodel.User u = null;
			if (Datamodel.User.FetchByAuth0(ref u, auth0)) {
				return new LiteProfile(u);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets all the profiles on the roster
		/// </summary>
		/// <returns></returns>
		public static List<LiteProfile> FetchAll() {
			List<Datamodel.User> dbUsers = null;
			Datamodel.User.FetchAll(ref dbUsers);

			return dbUsers.ConvertAll<LiteProfile>((a) => {
				return new LiteProfile(a); });
		}

		/// <summary>
		/// Gets all the profiles assigned to a ship
		/// </summary>
		/// <param name="shipId"></param>
		/// <param name="company"></param>
		/// <returns></returns>
		public static List<LiteProfile> FetchByAssignment(int shipId, 
			bool company) {
			List<Datamodel.User> dbUsers = null;
			Datamodel.User.FetchAllByAssignment(ref dbUsers, shipId, company);

			return dbUsers.ConvertAll<LiteProfile>((a) => {
				return new LiteProfile(a); });
		}

		/// <summary>
		/// Gets all the profiles without an assignment
		/// </summary>
		/// <returns></returns>
		public static List<LiteProfile> FetchAllUnassigned() {
			List<Datamodel.User> dbUsers = null;
			Datamodel.User.FetchAllUnassigned(ref dbUsers);

			return dbUsers.ConvertAll<LiteProfile>((a) => {
				return new LiteProfile(a); });
		}
		#endregion

		public void Refresh() {
			LiteProfile p = LiteProfile.FetchById(id);

			nickname = p.nickname;
			_rankId = p._rankId;
			_primaryRateId = p._primaryRateId;

			_rank = null;
			_primaryRate = null;
			_assignment = null;
		}
	}
}
