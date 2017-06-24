using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ANWI.Database.Model {
	/// <summary>
	/// Represents a row of the UserShip table.
	/// </summary>

	public class UserShip {
		#region Model

		public int id;
		public int user;
		public int hull;
		public int insurance;
		public int number;
		public string name;
		public int status;
		public long statusDate;
		public bool final;

		private UserShip(int id, int user, int hull, int insurance, int number,
			string name, int status, long statusDate, bool final) {
			this.id = id;
			this.user = user;
			this.hull = hull;
			this.insurance = insurance;
			this.number = number;
			this.name = name;
			this.status = status;
			this.statusDate = statusDate;
			this.final = final;
		}

		#endregion

		#region Class-Members

		public static UserShip Factory() {
			UserShip result = new UserShip(
				id: -1,
				user: -1,
				hull: -1,
				insurance: 0,
				number: 0,
				name: "",
				status: 0,
				statusDate: 0,
				final: false
			);
			return result;
		}

		public static UserShip Factory(int id, int user, int hull, 
			int insurance, int number, string name, int status, 
			long statusDate, bool final) {

			UserShip result = new UserShip(
				id: id,
				user: user,
				hull: hull,
				insurance: insurance,
				number: number,
				name: name,
				status: status,
				statusDate: statusDate,
				final: final
			);
			return result;
		}

		public static UserShip Factory(SQLiteDataReader reader) {
			UserShip result = new UserShip(
				id: Convert.ToInt32(reader["id"]),
				user: Convert.ToInt32(reader["user"]),
				hull: Convert.ToInt32(reader["hull"]),
				insurance: Convert.ToInt32(reader["insurance"]),
				number: Convert.ToInt32(reader["number"]),
				name: (string)reader["name"],
				status: Convert.ToInt32(reader["status"]),
				statusDate: Convert.ToInt64(reader["statusDate"]),
				final: Convert.ToBoolean(reader["final"])
			);
			return result;
		}

		/// <summary>
		/// Creates a new owned ship.
		/// Ship starts in drydocked status.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="user"></param>
		/// <param name="hull"></param>
		/// <param name="insurance"></param>
		/// <param name="name"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public static bool Create(ref UserShip output, int user, int hull, 
			int insurance, string name, int status) {
			int result = DBI.DoPreparedAction(
				$@"INSERT INTO UserShip (user, hull, insurance, number, name, 
				status, statusDate, final) 
				VALUES (@user, @hull, @insurance, 
				COALESCE((
					SELECT MAX(number)+1 FROM UserShip 
					WHERE hull IN (
						SELECT h1.id FROM Hull h1, Hull h2 
						WHERE h1.symbol = h2.symbol AND h2.id = @hull
					)),1), @name, @status, 
				strftime('%s', 'now'), 0);",
				new Tuple<string, object>("@user", user), 
				new Tuple<string, object>("@hull", hull), 
				new Tuple<string, object>("@insurance", insurance),  
				new Tuple<string, object>("@name", name),
				new Tuple<string, object>("@status", status));

			if (result == 1) {
				return UserShip.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		/// <summary>
		/// Gets an owned ship by ID
		/// </summary>
		/// <param name="output"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool FetchById(ref UserShip output, int id) {
			SQLiteDataReader reader = DBI.DoPreparedQuery(
				"SELECT * FROM UserShip WHERE id = @id LIMIT 1;",
				new Tuple<string, object>("@id", id));
			if (reader != null && reader.Read()) {
				output = UserShip.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets an owned ship by name
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool FetchByName(ref UserShip output, string name) {
			SQLiteDataReader reader = DBI.DoPreparedQuery(
				@"SELECT * FROM UserShip WHERE name = @name LIMIT 1;",
				new Tuple<string, object>("@name", name));
			if (reader != null && reader.Read()) {
				output = UserShip.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the list of all ships.
		/// Does not include ships which were destroyed or decommed 
		/// more than a month ago
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public static bool FetchRegistry(ref List<UserShip> output) {
			output = new List<UserShip>();

			SQLiteDataReader reader = DBI.DoQuery(
				@"SELECT * FROM UserShip
				WHERE (status != 1 AND status != 4)
				OR statusDate > strftime('%s', 'now', '-7 days');");
			while (reader != null && reader.Read()) {
				UserShip us = UserShip.Factory(reader);
				output.Add(us);
			}

			return true;
		}

		/// <summary>
		/// Gets the list of all ships which are active or drydocked.
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public static bool FetchAvailable(ref List<UserShip> output) {
			output = new List<UserShip>();

			SQLiteDataReader reader = DBI.DoQuery(
				@"SELECT * FROM UserShip
				WHERE status = 0 OR status = 3;");
			while (reader != null && reader.Read()) {
				UserShip us = UserShip.Factory(reader);
				output.Add(us);
			}

			return true;
		}

		/// <summary>
		/// Updates a ship
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool Store(UserShip input) {
			int result = DBI.DoPreparedAction(
				@"UPDATE UserShip SET user = @user, hull = @hull,
				insurance = @insurance, number = @number, name = @name, 
				status = @status, statusDate = @statusDate, final = @final 
				WHERE id = @id;",
				new Tuple<string, object>("@user", input.user), 
				new Tuple<string, object>("@hull", input.hull), 
				new Tuple<string, object>("@insurance", input.insurance), 
				new Tuple<string, object>("@number", input.number),
				new Tuple<string, object>("@name", input.name), 
				new Tuple<string, object>("@status", input.status),
				new Tuple<string, object>("@statusDate", input.statusDate),
				new Tuple<string, object>("@final", input.final),
				new Tuple<string, object>("@id", input.id));
			if (result == 1)
				return true;
			return false;
		}

		/// <summary>
		/// Updates only the status and as of date for a ship
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool StoreUpdateStatus(UserShip input) {
			int result = DBI.DoPreparedAction(
				@"UPDATE UserShip 
				SET status = @status, statusDate = strftime('%s', 'now')
				WHERE id = @id;",
				new Tuple<string, object>("@status", input.status), 
				new Tuple<string, object>("@id", input.id));
			if (result == 1)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Checks if a ship exists in the registry with the given name
		/// that is not destroyed or decommed
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool IsNameAvailable(string name) {
			SQLiteDataReader reader = DBI.DoPreparedQuery(
				@"SELECT * FROM UserShip
				WHERE name = @name
				AND status != 1 AND status != 2 AND status != 4",
				new Tuple<string, object>("@name", name)
				);

			// If results exist this name is not available
			if (reader.Read())
				return false;
			else
				return true;
		}

		/// <summary>
		/// Sets the final flag on all instances of a given ship name so their
		/// status cannot be changed.  This is done when a new ship is created
		/// with this name.
		/// </summary>
		/// <param name="name"></param>
		public static bool FinalizeOlderShips(string name) {
			int result1 = DBI.DoPreparedAction(
				@"UPDATE UserShip
				SET status = 1, final = 1
				WHERE name = @name
				AND (status = 1 OR status = 2)",
				new Tuple<string, object>("@name", name)
				);

			int result2 = DBI.DoPreparedAction(
				@"UPDATE UserShip
				SET final = 1
				WHERE name = @name
				AND status = 4",
				new Tuple<string, object>("@name", name)
				);

			if (result1 >= 1 && result2 >= 1)
				return true;
			else
				return false;
		}

		#endregion
	}
}