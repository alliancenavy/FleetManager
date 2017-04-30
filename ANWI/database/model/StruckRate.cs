using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ANWI.Database.Model
{
	/// <summary>
	/// Represents a row of the StruckRate table.
	/// </summary>

	public class StruckRate
	{
		#region Model

		public int id;
		public int user;
		public int rate;
		public int rank;
		public long earnedDate;
		public long expirationDate;
		
		private StruckRate(int id, int user, int rate, int rank, long earned, long expires)
		{
			this.id = id;
			this.user = user;
			this.rate = rate;
			this.rank = rank;
			this.earnedDate = earned;
			this.expirationDate = expires;
		}

		#endregion

		#region Class-Members

		public static StruckRate Factory()
		{
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

		public static StruckRate Factory(int id, int user, int rate, int rank, long earned, bool hasExp, long expires)
		{
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

		public static StruckRate Factory(SQLiteDataReader reader)
		{
			StruckRate result = new StruckRate(
				id:   Convert.ToInt32(reader["id"]),
				user: Convert.ToInt32(reader["user"]),
				rate: Convert.ToInt32(reader["rate"]),
				rank: Convert.ToInt32(reader["rank"]),
				earned: Convert.ToInt64(reader["earned"]),
				expires: Convert.ToInt64(reader["expires"])
			);
			return result;
		}

		private static string getExpirationQuery(int rate, int rank) {
			if(rank == 3) {
				return "null";
			} else {
				return $"(SELECT strftime('%s', 'now') + (SELECT rank{rank}duration FROM Rate WHERE id = {rate}))";
			}
		}

		public static bool Create(ref StruckRate output, int user, int rate, int rank)
		{
			string expQuery = getExpirationQuery(rate, rank);
			int result = DBI.DoAction($"insert into StruckRate (id, user, rate, rank, earned, expires) values((select max(id) from StruckRate) + 1, {user}, {rate}, {rank}, strftime('%s', 'now'), {expQuery});");
			if (result == 1)
			{
				return StruckRate.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		public static bool FetchById(ref StruckRate output, int id) {
			SQLiteDataReader reader = DBI.DoQuery($"select id, user, rate, rank, earned, COALESCE(expires, -1) as expires from StruckRate where id = {id} limit 1;");
			if (reader.Read()) {
				output = StruckRate.Factory(reader);
				return true;
			}
			return false;
		}

		public static bool FetchByUserRate(ref StruckRate output, int uid, int rid) {
			SQLiteDataReader reader = DBI.DoQuery($"select id, user, rate, rank, earned, COALESCE(expires, -1) as expires from StruckRate where user = {uid} and rate = {rid};");
			if (reader.Read()) {
				output = StruckRate.Factory(reader);
				return true;
			}
			return false;
		}

		public static bool FetchByUserId(ref List<StruckRate> output, int user)
		{
			SQLiteDataReader reader = DBI.DoQuery($"select id, user, rate, rank, earned, COALESCE(expires, -1) as expires from StruckRate where user = {user};");
			if ( reader.Read() )
			{
				output = new List<StruckRate>();
				do
				{
					output.Add(StruckRate.Factory(reader));
				}
				while ( reader.Read() );
				return true;
			}
			return false;
		}

		public static bool FetchByRateId(ref List<StruckRate> output, int rate)
		{
			SQLiteDataReader reader = DBI.DoQuery($"select id, user, rate, rank, earned, COALESCE(expires, -1) as expires from StruckRate where id = {rate};");
			if ( reader.Read() )
			{
				output = new List<StruckRate>();
				do
				{
					output.Add(StruckRate.Factory(reader));
				}
				while ( reader.Read() );
				return true;
			}
			return false;
		}

		public static bool Store(StruckRate input)
		{
			string expQuery = getExpirationQuery(input.rank, input.rank);
			int result = DBI.DoAction($"update StruckRate set user = {input.user}, rate = {input.rate}, rank = {input.rank}, earned = strftime('%s', 'now'), expires = {expQuery} where id = {input.id};");
			if (result == 1)
				return true;
			return false;
		}

		public static bool DeleteById(int StruckID) {
			int result = DBI.DoAction($"delete from StruckRate where id = {StruckID}");
			if (result == 1)
				return true;
			return false;
		}

		#endregion
	}
}