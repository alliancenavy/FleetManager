using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace ANWI.Database.Model {
	public class UserPrivs {
		#region Model

		public int user;
		public bool canPromote;
		public bool canCertify;
		public bool canAssign;
		public bool canStartOps;
		public bool isFleetAdmin;

		private UserPrivs(int user, bool canPromote, bool canCertify, bool canAssign, bool canStartOps, bool isFleetAdmin) {
			this.user = user;
			this.canPromote = canPromote;
			this.canCertify = canCertify;
			this.canAssign = canAssign;
			this.canStartOps = canStartOps;
			this.isFleetAdmin = isFleetAdmin;
		}

		#endregion

		#region Class-Members

		public static UserPrivs Factory() {
			UserPrivs result = new UserPrivs(
				user: -1,
				canPromote: false,
				canCertify: false,
				canAssign: false,
				canStartOps: false,
				isFleetAdmin: false
				);
			return result;
		}

		public static UserPrivs Factory(int user, bool canPromote, bool canCertify, bool canAssign, bool canStartOps, bool isFleetAdmin) {
			UserPrivs result = new UserPrivs(
				user: user,
				canPromote: canPromote,
				canCertify: canCertify,
				canAssign: canAssign,
				canStartOps: canStartOps,
				isFleetAdmin: isFleetAdmin
				);
			return result;
		}

		public static UserPrivs Factory(SQLiteDataReader reader) {
			UserPrivs result = new UserPrivs(
				user: Convert.ToInt32(reader["user"]),
				canPromote: Convert.ToBoolean(reader["canPromote"]),
				canCertify: Convert.ToBoolean(reader["canCertify"]),
				canAssign: Convert.ToBoolean(reader["canAssign"]),
				canStartOps: Convert.ToBoolean(reader["canStartOps"]),
				isFleetAdmin: Convert.ToBoolean(reader["isFleetAdmin"])
				);
			return result;
		}

		public static bool Create(ref UserPrivs output, int user, bool canPromote, bool canCertify, bool canAssign, bool canStartOps, bool isFleetAdmin) {
			int result = DBI.DoAction($"insert into UserPrivs (user, canPromote, canCertify, canAssign, canStartOps, isFleetAdmin) values ({user}, {canPromote}, {canCertify}, {canAssign}, {canStartOps}, {isFleetAdmin});");
			if(result == 1) {
				return UserPrivs.FetchByUser(ref output, user);
			}
			return false;
		}

		public static bool FetchByUser(ref UserPrivs output, int user) {
			SQLiteDataReader reader = DBI.DoQuery($"select * from UserPrivs where user={user};");
			if(reader.Read()) {
				output = UserPrivs.Factory(reader);
				return true;
			}
			return false;
		}

		#endregion
	}
}
