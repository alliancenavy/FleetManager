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

		private UserPrivs(int user, bool canPromote, bool canCertify) {
			this.user = user;
			this.canPromote = canPromote;
			this.canCertify = canCertify;
		}

		#endregion

		#region Class-Members

		public static UserPrivs Factory() {
			UserPrivs result = new UserPrivs(
				user: -1,
				canPromote: false,
				canCertify: false
				);
			return result;
		}

		public static UserPrivs Factory(int user, bool canPromote, bool canCertify) {
			UserPrivs result = new UserPrivs(
				user: user,
				canPromote: canPromote,
				canCertify: canCertify
				);
			return result;
		}

		public static UserPrivs Factory(SQLiteDataReader reader) {
			UserPrivs result = new UserPrivs(
				user: Convert.ToInt32(reader["user"]),
				canPromote: Convert.ToBoolean(reader["canPromote"]),
				canCertify: Convert.ToBoolean(reader["canCertify"])
				);
			return result;
		}

		public static bool Create(ref UserPrivs output, int user, bool canPromote, bool canCertify) {
			int result = DBI.DoAction($"insert into UserPrivs (user, canPromote, canCertify) values ({user}, {canPromote}, {canCertify});");
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
