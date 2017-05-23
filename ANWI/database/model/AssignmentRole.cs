using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ANWI.Database.Model {
	/// <summary>
	/// Represents a row of the AssignmentRole table.
	/// </summary>
	public class AssignmentRole {
		#region Model

		public int id;
		public string name;
		public bool isCompany;

		private AssignmentRole(int id, string name, bool isCompany) {
			this.id = id;
			this.name = name;
			this.isCompany = isCompany;
		}

		#endregion

		#region Class-Members

		public static AssignmentRole Factory() {
			AssignmentRole result = new AssignmentRole(
				id: -1,
				name: "",
				isCompany: false
			);
			return result;
		}

		public static AssignmentRole Factory(int id, string name, 
			bool isCompany) {
			AssignmentRole result = new AssignmentRole(
				id: id,
				name: name,
				isCompany: isCompany
			);
			return result;
		}

		public static AssignmentRole Factory(SQLiteDataReader reader) {
			AssignmentRole result = new AssignmentRole(
				id: Convert.ToInt32(reader["id"]),
				name: (string)reader["name"],
				isCompany: Convert.ToBoolean(reader["isCompany"])
			);
			return result;
		}

		/// <summary>
		/// Creates a new assignment role
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name"></param>
		/// <param name="isCompany"></param>
		/// <returns></returns>
		public static bool Create(ref AssignmentRole output, string name, 
			bool isCompany) {
			int result = DBI.DoPreparedAction(
				@"INSERT INTO AssignmentRole (name, isCompany) 
				VALUES (@name, @company);",
				new Tuple<string, object>("@name", name),
				new Tuple<string, object>("@company", isCompany));
			if (result == 1) {
				return AssignmentRole.FetchById(ref output, 
					DBI.LastInsertRowId);
			}
			return false;
		}

		/// <summary>
		/// Fetches an assignment role by ID
		/// </summary>
		/// <param name="output"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool FetchById(ref AssignmentRole output, int id) {
			SQLiteDataReader reader = DBI.DoPreparedQuery(
				@"SELECT * FROM AssignmentRole 
				WHERE id = @id LIMIT 1;",
				new Tuple<string, object>("@id", id));
			if (reader != null && reader.Read()) {
				output = AssignmentRole.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Fetches a role by name
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool FetchByName(ref AssignmentRole output, string name) {
			SQLiteDataReader reader = DBI.DoPreparedQuery(
				@"SELECT * FROM AssignmentRole 
				WHERE name = @name LIMIT 1;",
				new Tuple<string, object>("@name", name));
			if (reader != null && reader.Read()) {
				output = AssignmentRole.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Fetches all possible assignment roles
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public static bool FetchAll(ref List<AssignmentRole> output) {
			output = new List<AssignmentRole>();

			SQLiteDataReader reader = DBI.DoQuery(
				"SELECT * FROM AssignmentRole;");
			while (reader != null && reader.Read()) {
				AssignmentRole ar = AssignmentRole.Factory(reader);
				output.Add(ar);
			}

			return true;
		}

		/// <summary>
		/// Updates a role in the table
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool Store(AssignmentRole input) {
			int result = DBI.DoPreparedAction(
				@"UPDATE AssignmentRole 
				SET name = @name, isCompany = @company 
				WHERE id = @id;",
				new Tuple<string, object>("@name", input.name), 
				new Tuple<string, object>("@company", input.isCompany), 
				new Tuple<string, object>("@id", input.id));
			if (result == 1)
				return true;
			return false;
		}

		#endregion
	}
}