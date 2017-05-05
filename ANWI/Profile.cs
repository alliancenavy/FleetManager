using System.Collections.Generic;
using Datamodel = ANWI.Database.Model;
using ANWI.Database;

namespace ANWI {

	/// <summary>
	/// A user's profile.
	/// Incudings things like name, rank, rates, etc
	/// </summary>
	public class Profile {

		#region Instance Variables
		public int id;
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

		// List of all of the user's rates
		private List<Rate> _rates = null;
		public List<Rate> rates {
			get {
				if(DBI.IsOpen() && _rates == null) {
					_rates = Rate.FetchUserRates(id);
				}
				return _rates;
			}
			set { _rates = value; }
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

		// The full assignment history for this user
		private List<Assignment> _assignmentHistory = null;
		public List<Assignment> assignmentHistory {
			get {
				if(DBI.IsOpen() && _assignmentHistory == null) {
					_assignmentHistory = Assignment.FetchAssignmentHistory(id);
				}
				return _assignmentHistory;
			}
			set { _assignmentHistory = value; }
		}

		// This user's privileges
		private Privs _privs = null;
		public Privs privs {
			get {
				if(DBI.IsOpen() && _privs == null) {
					_privs = Privs.FetchByUser(id);
				}
				return _privs;
			}
			set { _privs = value; }
		}
		#endregion

		#region Constructors
		public Profile() {
			id = 0;
			nickname = "";
		}

		private Profile(Datamodel.User user) {
			id = user.id;
			nickname = user.name;
			_rankId = user.rank;
			_primaryRateId = user.rate;
		}

		/// <summary>
		/// Gets a user by ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static Profile FetchById(int id) {
			Datamodel.User u = null;
			if(Datamodel.User.FetchById(ref u, id)) {
				return new Profile(u);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets a user by their Auth0 identifier
		/// </summary>
		/// <param name="auth0"></param>
		/// <returns></returns>
		public static Profile FetchByAuth0(string auth0) {
			Datamodel.User u = null;
			if(Datamodel.User.FetchByAuth0(ref u, auth0)) {
				return new Profile(u);
			} else {
				return null;
			}
		}


		#endregion
	}
}
