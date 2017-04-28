using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;
using MsgPack.Serialization;
using ANWI.Database;

namespace ANWI {
	public class LiteProfile {
		#region Instance Variables
		public int id { get; set; }
		public string nickname { get; set; }

		private int _rankId;
		private Rank _rank = null;
		public Rank rank {
			get {
				if(_rank == null) {
					_rank = Rank.FetchById(_rankId);
				}
				return _rank;
			}
			set { _rank = value; }
		}

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

		[MessagePackIgnore]
		public bool isMe { get; set; } = false;
		#endregion

		#region WPF Helpers
		public string fullName { get { return rank.abbrev + " " + nickname; } }
		#endregion

		#region Constructors
		public LiteProfile() {
			id = 0;
			nickname = "";
			_rankId = 0;
			_primaryRateId = 0;
		}

		private LiteProfile(Datamodel.User u) {
			id = u.id;
			nickname = u.name;
			_rankId = u.rank;
			_primaryRateId = u.rate;
		}

		public static LiteProfile FetchById(int id) {
			Datamodel.User u = null;
			if(Datamodel.User.FetchById(ref u, id)) {
				return new LiteProfile(u);
			} else {
				return null;
			}
		}

		public static List<LiteProfile> FetchAll() {
			List<Datamodel.User> dbUsers = null;
			Datamodel.User.FetchAll(ref dbUsers);

			return dbUsers.ConvertAll<LiteProfile>((a) => { return new LiteProfile(a); });
		}
		#endregion
	}
}
