using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ANWI.Database.Model {
	/// <summary>
	/// Represents a row of the User table.
	/// </summary>

	public class User {
		#region Model

		public int id;
		public string name;
		public string auth0;
		public int rank;
		public int rate;

		private User(int id, string name, string auth0, int rank, int rate) {
			this.id = id;
			this.name = name;
			this.auth0 = auth0;
			this.rank = rank;
			this.rate = rate;
		}

		#endregion

		#region Class-Members

		public static User Factory() {
			User result = new User(
				id: -1,
				name: "",
				auth0: "",
				rank: -1,
				rate: -1
			);
			return result;
		}

		public static User Factory(int id, string name, string auth0, int rank,
			int rate) {

			User result = new User(
				id: id,
				name: name,
				auth0: auth0,
				rank: rank,
				rate: rate
			);
			return result;
		}

		public static User Factory(SQLiteDataReader reader) {
			User result = new User(
				id: Convert.ToInt32(reader["id"]),
				name: (string)reader["name"],
				auth0: (string)reader["auth0"],
				rank: Convert.ToInt32(reader["rank"]),
				rate: reader["rate"] is DBNull ? 0 : 
					Convert.ToInt32(reader["rate"])
			);
			return result;
		}

		/// <summary>
		/// Creates a new user without a primary rate
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name"></param>
		/// <param name="auth0"></param>
		/// <param name="rank"></param>
		/// <returns></returns>
		public static bool Create(ref User output, string name, string auth0,
			int rank) {
			int result = DBI.DoAction(
				$@"INSERT INTO User (name, auth0, rank, rate) 
				VALUES ('{name}', '{auth0}', {rank}, null);");
			if (result == 1) {
				return User.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		/// <summary>
		/// Create a new user with a primary rate
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name"></param>
		/// <param name="auth0"></param>
		/// <param name="rank"></param>
		/// <param name="rate"></param>
		/// <returns></returns>
		public static bool Create(ref User output, string name, string auth0,
			int rank, int rate) {
			int result = DBI.DoAction(
				$@"INSERT INTO User (name, auth0, rank, rate) 
				VALUES ('{name}', '{auth0}', {rank}, {rate});");
			if (result == 1) {
				return User.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		/// <summary>
		/// Gets a list of all users
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public static bool FetchAll(ref List<User> output) {
			output = new List<User>();

			SQLiteDataReader reader = DBI.DoQuery(
				$"SELECT * FROM User WHERE id != 0");
			while (reader != null && reader.Read()) {
				User u = User.Factory(reader);
				output.Add(u);
			}

			return true;
		}

		/// <summary>
		/// Gets a list of users assigned to a ship split up by company
		/// and embarked.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="shipId"></param>
		/// <param name="company"></param>
		/// <returns></returns>
		public static bool FetchAllByAssignment(ref List<User> output, 
			int shipId, bool company) {
			output = new List<User>();

			int isCompany = Convert.ToInt32(company);
			SQLiteDataReader reader = DBI.DoQuery(
				$@"SELECT u.id, u.name, u.auth0, u.rank, u.rate 
				FROM User u, Assignment a, AssignmentRole ar 
				WHERE a.user = u.id AND a.role = ar.id 
				AND ar.isCompany = {isCompany} AND a.ship = {shipId} 
				AND a.until is null ORDER BY ar.id ASC;");
			while (reader != null && reader.Read()) {
				User u = User.Factory(reader);
				output.Add(u);
			}

			return true;
		}

		/// <summary>
		/// Gets a list of all users without a current assignment
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public static bool FetchAllUnassigned(ref List<User> output) {
			output = new List<User>();

			SQLiteDataReader reader = DBI.DoQuery(
				@"SELECT u.id, u.name, u.auth0, u.rank, u.rate 
				FROM User u 
				WHERE u.id NOT IN 
					(SELECT user FROM Assignment WHERE until IS NULL) 
				AND u.id != 0;");
			while (reader != null && reader.Read()) {
				User u = User.Factory(reader);
				output.Add(u);
			}

			return true;
		}

		/// <summary>
		/// Gets a user by ID
		/// </summary>
		/// <param name="output"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool FetchById(ref User output, int id) {
			SQLiteDataReader reader = DBI.DoQuery(
				$"SELECT * FROM User WHERE id = {id} LIMIT 1;");
			if (reader != null && reader.Read()) {
				output = User.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets a user by their name
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool FetchByName(ref User output, string name) {
			SQLiteDataReader reader = DBI.DoQuery(
				$"SELECT * FROM User WHERE name = '{name}' LIMIT 1;");
			if (reader != null && reader.Read()) {
				output = User.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets a user by their Auth0 identifier
		/// </summary>
		/// <param name="output"></param>
		/// <param name="auth0"></param>
		/// <returns></returns>
		public static bool FetchByAuth0(ref User output, string auth0) {
			SQLiteDataReader reader = DBI.DoQuery(
				$"SELECT * FROM User WHERE auth0 = '{auth0}' LIMIT 1;");
			if (reader != null && reader.Read()) {
				output = User.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Updates a user
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool Store(User input) {
			int result = DBI.DoAction(
				$@"UPDATE User SET name = '{input.name}', 
				auth0 = '{input.auth0}', rank = {input.rank},
				rate = {input.rate} WHERE id = {input.id};");
			if (result == 1)
				return true;
			return false;
		}

		#endregion
	}
}