using System;
using System.Data.SQLite;

namespace ANWI.Database.Model
{
	/// <summary>
	/// Represents a row of the Assignment table.
	/// </summary>

	public class Assignment
	{
		#region Model

		public int id;
		public int user;
		public int ship;
		public int role;
		public int from;
		public int until;

		private Assignment(int id, int user, int ship, int role, int from, int until)
		{
			this.id = id;
			this.user = user;
			this.ship = ship;
			this.role = role;
			this.from = from;
			this.until = until;
		}

		#endregion

		#region Class-Members

		public static Assignment Factory()
		{
			Assignment result = new Assignment(
				id: -1,
				user: -1,
				ship: -1,
				role: -1,
				from: 0,
				until: 0
			);
			return result;
		}

		public static Assignment Factory(int id, int user, int ship, int role, int from, int until)
		{

			Assignment result = new Assignment(
				id: id,
				user: user,
				ship: ship,
				role: role,
				from: from,
				until: until
			);
			return result;
		}

		public static Assignment Factory(SQLiteDataReader reader)
		{
			Assignment result = new Assignment(
				id:    Convert.ToInt32(reader["id"]),
				user:  Convert.ToInt32(reader["user"]),
				ship:  Convert.ToInt32(reader["ship"]),
				role:  Convert.ToInt32(reader["role"]),
				from:  Convert.ToInt32(reader["start"]),
				until: 0//Convert.ToInt32(reader["until"]),
			);
			return result;
		}

		public static bool Create(ref Assignment output, int user, int ship, int role, int from, int until)
		{
			int result = DBI.DoAction($"insert into Assignment (user, ship, role, from, until) values({user}, {ship}, {role}, {from}, {until});");
			if (result == 1)
			{
				return Assignment.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		public static bool FetchById(ref Assignment output, int id)
		{
			SQLiteDataReader reader = DBI.DoQuery($"select * from Assignment where id = {id} limit 1;");
			if (reader.Read())
			{
				output = Assignment.Factory(reader);
				return true;
			}
			return false;
		}

		public static bool FetchCurrentAssignment(ref Assignment output, int userId) {
			SQLiteDataReader reader = DBI.DoQuery($"select id, user, ship, role, start, COALESCE(until, 0) from Assignment where user = {userId} and until is null limit 1;");
			if(reader.Read()) {
				output = Assignment.Factory(reader);
				return true;
			}
			return false;
		}

		public static bool Store(Assignment input)
		{
			int result = DBI.DoAction($"update Assignment set user = {input.user}, ship = {input.ship}, role = {input.role}, from = {input.from}, until = {input.until} where id = {input.id};");
			if (result == 1)
				return true;
			return false;
		}

		#endregion
	}
}