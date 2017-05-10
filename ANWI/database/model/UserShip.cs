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

		private UserShip(int id, int user, int hull, int insurance, int number,
			string name, int status, long statusDate) {
			this.id = id;
			this.user = user;
			this.hull = hull;
			this.insurance = insurance;
			this.number = number;
			this.name = name;
			this.status = status;
			this.statusDate = statusDate;
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
				statusDate: 0
			);
			return result;
		}

		public static UserShip Factory(int id, int user, int hull, 
			int insurance, int number, string name, int status, 
			long statusDate) {

			UserShip result = new UserShip(
				id: id,
				user: user,
				hull: hull,
				insurance: insurance,
				number: number,
				name: name,
				status: status,
				statusDate: statusDate
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
				statusDate: Convert.ToInt64(reader["statusDate"])
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
			int result = DBI.DoAction(
				$@"INSERT INTO UserShip (user, hull, insurance, number, name, 
				status, statusDate) 
				VALUES ({user}, {hull}, {insurance}, 
				COALESCE((
					SELECT MAX(number)+1 FROM UserShip 
					WHERE hull IN (
						SELECT h1.id FROM Hull h1, Hull h2 
						WHERE h1.symbol = h2.symbol AND h2.id = {hull}
					)),1), '{name}', {status}, 
				strftime('%s', 'now'));");

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
			SQLiteDataReader reader = DBI.DoQuery(
				$"SELECT * FROM UserShip WHERE id = {id} LIMIT 1;");
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
			SQLiteDataReader reader = DBI.DoQuery(
				$@"SELECT * FROM UserShip WHERE name = '{name}' LIMIT 1;");
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
				$@"SELECT * FROM UserShip
				WHERE (status != 1 AND status != 4)
				OR (
					(status == 1 OR status == 4)
					AND statusDate > strftime('%s', 'now', '-1 month')
				);");
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
				$@"SELECT * FROM UserShip
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
			int result = DBI.DoAction(
				$@"UPDATE UserShip SET user = {input.user}, hull = {input.hull},
				insurance = {input.insurance}, number = {input.number}, 
				name = '{input.name}', status = {input.status}, 
				statusDate = {input.statusDate} 
				WHERE id = {input.id};");
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
			int result = DBI.DoAction(
				$@"UPDATE UserShip 
				SET status = {input.status}, statusDate = strftime('%s', 'now')
				WHERE id = {input.id};");
			if (result == 1)
				return true;
			else
				return false;
		}

		#endregion
	}
}