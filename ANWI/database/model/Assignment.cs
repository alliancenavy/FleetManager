using System;
using System.Data.SQLite;
using System.Collections.Generic;

namespace ANWI.Database.Model {

	/// <summary>
	/// Represents a row of the Assignment table.
	/// </summary>
	public class Assignment {
		#region Model

		public int id;
		public int user;
		public int ship;
		public int role;
		public long from;
		public long until;

		private Assignment(int id, int user, int ship, int role, long from, 
			long until) {
			this.id = id;
			this.user = user;
			this.ship = ship;
			this.role = role;
			this.from = from;
			this.until = until;
		}

		#endregion

		#region Class-Members

		public static Assignment Factory() {
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

		public static Assignment Factory(int id, int user, int ship, int role, 
			long from, long until) {

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

		public static Assignment Factory(SQLiteDataReader reader) {
			Assignment result = new Assignment(
				id: Convert.ToInt32(reader["id"]),
				user: Convert.ToInt32(reader["user"]),
				ship: Convert.ToInt32(reader["ship"]),
				role: Convert.ToInt32(reader["role"]),
				from: Convert.ToInt64(reader["start"]),
				until: Convert.ToInt64(reader["until"])
			);
			return result;
		}

		/// <summary>
		/// Creates a new assignment with the given start and end dates
		/// </summary>
		/// <param name="output"></param>
		/// <param name="user"></param>
		/// <param name="ship"></param>
		/// <param name="role"></param>
		/// <param name="from"></param>
		/// <param name="until"></param>
		/// <returns></returns>
		public static bool Create(ref Assignment output, int user, int ship, 
			int role, long from, long until) {
			int result = DBI.DoPreparedAction(
				@"INSERT INTO Assignment (user, ship, role, start, until) 
				VALUES (@user, @ship, @role, @from, @until);",
				new Tuple<string, object>("@user", user), 
				new Tuple<string, object>("@ship", ship), 
				new Tuple<string, object>("@role", role), 
				new Tuple<string, object>("@from", from), 
				new Tuple<string, object>("@until", until));
			if (result == 1) {
				return Assignment.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		/// <summary>
		/// Creates a new assignment with a starting time of today's date 
		/// and a null ending date
		/// </summary>
		/// <param name="output"></param>
		/// <param name="user"></param>
		/// <param name="ship"></param>
		/// <param name="role"></param>
		/// <returns></returns>
		public static bool Create(ref Assignment output, int user, int ship, 
			int role) {
			int result = DBI.DoPreparedAction(
				@"INSERT INTO Assignment (user, ship, role, start) 
				VALUES (@user, @ship, @role, strftime('%s', 'now'));",
				new Tuple<string, object>("@user", user), 
				new Tuple<string, object>("@ship", ship), 
				new Tuple<string, object>("@role", role));
			if (result == 1) {
				return Assignment.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		/// <summary>
		/// Gets an assignment by its ID
		/// </summary>
		/// <param name="output"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool FetchById(ref Assignment output, int id) {
			SQLiteDataReader reader = DBI.DoPreparedQuery(
				@"SELECT id, user, ship, role, start, 
				COALESCE(until, -1) AS until 
				FROM Assignment 
				WHERE id = @id LIMIT 1;",
				new Tuple<string, object>("@id", id));
			if (reader != null && reader.Read()) {
				output = Assignment.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the current assignment of the user
		/// </summary>
		/// <param name="output"></param>
		/// <param name="userId">The assigned user</param>
		/// <returns></returns>
		public static bool FetchCurrentAssignment(ref Assignment output, 
			int userId) {
			SQLiteDataReader reader = DBI.DoPreparedQuery(
				@"SELECT id, user, ship, role, start, 
				COALESCE(until, -1) AS until 
				FROM Assignment 
				WHERE user = @user AND until IS NULL LIMIT 1;",
				new Tuple<string, object>("@user", userId));
			if (reader != null && reader.Read()) {
				output = Assignment.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Fetches the full assignment history of a user ordered from most 
		/// recent to oldest
		/// </summary>
		/// <param name="output"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static bool FetchAssignmentHistory(ref List<Assignment> output, 
			int userId) {
			output = new List<Assignment>();

			SQLiteDataReader reader = DBI.DoPreparedQuery(
				@"SELECT id, user, ship, role, start, 
				COALESCE(until, -1) AS until 
				FROM Assignment 
				WHERE user = @user ORDER BY start DESC;",
				new Tuple<string, object>("@user", userId));
			while (reader != null && reader.Read()) {
				Assignment a = Assignment.Factory(reader);
				output.Add(a);
			}

			return true;
		}

		/// <summary>
		/// Updates an assignment row in the table
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool Store(Assignment input) {
			int result = DBI.DoPreparedAction(
				@"UPDATE Assignment 
				SET user = @user, ship = @ship, 
				role = @role, start = @start, until = @until
				WHERE id = @id;",
				new Tuple<string, object>("@user", input.user), 
				new Tuple<string, object>("@ship", input.ship), 
				new Tuple<string, object>("@role", input.role), 
				new Tuple<string, object>("@from", input.from),
				new Tuple<string, object>("@until", input.until),
				new Tuple<string, object>("@id", input.id));
			if (result == 1)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Updates a given assignment with an end date of today's date.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="assignmentId"></param>
		/// <returns></returns>
		public static bool EndAssignment(int userId, int assignmentId) {
			int result = DBI.DoPreparedAction(
				@"UPDATE Assignment 
				SET until = strftime('%s', 'now') 
				WHERE user = @user AND id = @assignment;",
				new Tuple<string, object>("@user", userId),
				new Tuple<string, object>("@assignment", assignmentId));
			if (result == 1)
				return true;
			else
				return false;
		}

		#endregion
	}
}