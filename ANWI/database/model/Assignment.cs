using System;
using System.Data.SQLite;
using System.Collections;
using System.Collections.Generic;

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
		public long from;
		public long until;

		private Assignment(int id, int user, int ship, int role, long from, long until)
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
				from: -1,
				until: -1
			);
			return result;
		}

		public static Assignment Factory(int id, int user, int ship, int role, long from, long until)
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
				from:  Convert.ToInt64(reader["start"]),
				until: Convert.ToInt64(reader["until"])
			);
			return result;
		}

		public static bool Create(ref Assignment output, int user, int ship, int role, long from, long until)
		{
			int result = DBI.DoAction($"insert into Assignment (user, ship, role, start, until) values({user}, {ship}, {role}, {from}, {until});");
			if (result == 1)
			{
				return Assignment.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		public static bool Create(ref Assignment output, int user, int ship, int role) {
			int result = DBI.DoAction($"insert into Assignment (user, ship, role, start) values({user}, {ship}, {role}, strftime('%s', 'now'));");
			if (result == 1) {
				return Assignment.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		public static bool FetchById(ref Assignment output, int id)
		{
			SQLiteDataReader reader = DBI.DoQuery($"select id, user, ship, role, start, COALESCE(until, -1) as until from Assignment where id = {id} limit 1;");
			if (reader.Read())
			{
				output = Assignment.Factory(reader);
				return true;
			}
			return false;
		}

		public static bool FetchCurrentAssignment(ref Assignment output, int userId) {
			SQLiteDataReader reader = DBI.DoQuery($"select id, user, ship, role, start, COALESCE(until, -1) as until from Assignment where user = {userId} and until is null limit 1;");
			if(reader.Read()) {
				output = Assignment.Factory(reader);
				return true;
			}
			return false;
		}

		public static bool FetchAssignmentHistory(ref List<Assignment> output, int userId) {
			output = new List<Assignment>();

			SQLiteDataReader reader = DBI.DoQuery($"select id, user, ship, role, start, COALESCE(until, -1) as until from Assignment where user = {userId} order by start desc;");
			while(reader.Read()) {
				Assignment a = Assignment.Factory(reader);
				output.Add(a);
			}

			return true;
		}

		public static bool Store(Assignment input)
		{
			int result = DBI.DoAction($"update Assignment set user = {input.user}, ship = {input.ship}, role = {input.role}, start = {input.from}, until = {input.until} where id = {input.id};");
			if (result == 1)
				return true;
			else
				return false;
		}

		public static bool EndAssignment(int userId, int assignmentId) {
			int result = DBI.DoAction($"update Assignment set until = strftime('%s', 'now') where user = {userId} and id = {assignmentId};");
			if (result == 1)
				return true;
			else
				return false;
		}

		#endregion
	}
}