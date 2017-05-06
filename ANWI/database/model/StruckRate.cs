using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ANWI.Database.Model {
	/// <summary>
	/// Represents a row of the StruckRate table.
	/// </summary>

	public class StruckRate {
		#region Model

		public int id;
		public int user;
		public int rate;
		public int rank;
		public long earnedDate;
		public long expirationDate;

		private StruckRate(int id, int user, int rate, int rank, long earned, 
			long expires) {
			this.id = id;
			this.user = user;
			this.rate = rate;
			this.rank = rank;
			this.earnedDate = earned;
			this.expirationDate = expires;
		}

		#endregion

		#region Class-Members

		public static StruckRate Factory() {
			StruckRate result = new StruckRate(
				id: -1,
				user: -1,
				rate: -1,
				rank: 0,
				earned: -1,
				expires: -1
			);
			return result;
		}

		public static StruckRate Factory(int id, int user, int rate, int rank, 
			long earned, bool hasExp, long expires) {
			StruckRate result = new StruckRate(
				id: id,
				user: user,
				rate: rate,
				rank: rank,
				earned: earned,
				expires: expires
			);
			return result;
		}

		public static StruckRate Factory(SQLiteDataReader reader) {
			StruckRate result = new StruckRate(
				id: Convert.ToInt32(reader["id"]),
				user: Convert.ToInt32(reader["user"]),
				rate: Convert.ToInt32(reader["rate"]),
				rank: Convert.ToInt32(reader["rank"]),
				earned: Convert.ToInt64(reader["earned"]),
				expires: Convert.ToInt64(reader["expires"])
			);
			return result;
		}

		/// <summary>
		/// Builds a query which determines the expiration date of a rating 
		/// when creating and updating.
		/// 3rd Class rates do not expire.  2nd and 1st do.
		/// </summary>
		/// <param name="rate">The rate being struck</param>
		/// <param name="rank">The rank level</param>
		/// <returns></returns>
		private static string getExpirationQuery(int rate, int rank) {
			if (rank == 3) {
				return "null";
			} else {
				return $"(SELECT strftime('%s', 'now') + " + 
					$"(SELECT rank{rank}duration FROM Rate WHERE id = {rate}))";
			}
		}

		/// <summary>
		/// Creates a new rate.  Automatically sets the earned date to today.
		/// Calculates the expiration date if necessary.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="user"></param>
		/// <param name="rate"></param>
		/// <param name="rank"></param>
		/// <returns></returns>
		public static bool Create(ref StruckRate output, int user, int rate, 
			int rank) {
			string expQuery = getExpirationQuery(rate, rank);
			int result = DBI.DoAction(
				$@"INSERT INTO StruckRate (id, user, rate, 
				rank, earned, expires) 
				VALUES ((SELECT max(id) FROM StruckRate) + 1, {user}, {rate}, 
				{rank}, strftime('%s', 'now'), {expQuery});");
			if (result == 1) {
				return StruckRate.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		/// <summary>
		/// Gets a rating by ID
		/// </summary>
		/// <param name="output"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool FetchById(ref StruckRate output, int id) {
			SQLiteDataReader reader = DBI.DoQuery(
				$@"SELECT id, user, rate, rank, earned, 
				COALESCE(expires, -1) AS expires 
				FROM StruckRate 
				WHERE id = {id} LIMIT 1;");
			if (reader.Read()) {
				output = StruckRate.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets a user's primary rate based on their id and rate id
		/// </summary>
		/// <param name="output"></param>
		/// <param name="uid"></param>
		/// <param name="rid"></param>
		/// <returns></returns>
		public static bool FetchByUserRate(ref StruckRate output, int uid, 
			int rid) {
			SQLiteDataReader reader = DBI.DoQuery(
				$@"SELECT id, user, rate, rank, earned, 
				COALESCE(expires, -1) AS expires 
				FROM StruckRate 
				WHERE user = {uid} AND id = {rid};");
			if (reader.Read()) {
				output = StruckRate.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets all of the rates a user has
		/// </summary>
		/// <param name="output"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public static bool FetchByUserId(ref List<StruckRate> output, 
			int user) {
			SQLiteDataReader reader = DBI.DoQuery(
				$@"SELECT id, user, rate, rank, earned, 
				COALESCE(expires, -1) AS expires 
				FROM StruckRate 
				WHERE user = {user};");
			if (reader.Read()) {
				output = new List<StruckRate>();
				do {
					output.Add(StruckRate.Factory(reader));
				}
				while (reader.Read());
				return true;
			}
			return false;
		}

		/// <summary>
		/// Fetches a rating by ID
		/// </summary>
		/// <param name="output"></param>
		/// <param name="rate"></param>
		/// <returns></returns>
		public static bool FetchByRateId(ref List<StruckRate> output, 
			int rate) {
			SQLiteDataReader reader = DBI.DoQuery(
				$@"SELECT id, user, rate, rank, earned, 
				COALESCE(expires, -1) AS expires 
				FROM StruckRate 
				WHERE id = {rate};");
			if (reader.Read()) {
				output = new List<StruckRate>();
				do {
					output.Add(StruckRate.Factory(reader));
				}
				while (reader.Read());
				return true;
			}
			return false;
		}

		/// <summary>
		/// Updated a rate strike.  Automatically moves the earned time up to 
		/// today and moves the expiration date accordingly.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool Store(StruckRate input) {
			string expQuery = getExpirationQuery(input.rank, input.rank);
			int result = DBI.DoAction(
				$@"UPDATE StruckRate SET user = {input.user}, 
				rate = {input.rate}, rank = {input.rank}, 
				earned = strftime('%s', 'now'), expires = {expQuery} 
				WHERE id = {input.id};");
			if (result == 1)
				return true;
			return false;
		}

		/// <summary>
		/// Deletes a struck rate for a user
		/// </summary>
		/// <param name="StruckID"></param>
		/// <returns></returns>
		public static bool DeleteById(int StruckID) {
			int result = DBI.DoAction(
				$"DELETE FROM StruckRate WHERE id = {StruckID}");
			if (result == 1)
				return true;
			return false;
		}

		#endregion
	}
}