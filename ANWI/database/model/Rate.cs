using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ANWI.Database.Model {
	/// <summary>
	/// Represents a row of the Rate table.
	/// </summary>

	public class Rate {
		#region Model

		public int id;
		public string name;
		public string abrv;
		public long rank2duration;
		public long rank1duration;

		private Rate(int id, string name, string abrv, long r2dur, long r1dur) {
			this.id = id;
			this.name = name;
			this.abrv = abrv;
			this.rank2duration = r2dur;
			this.rank1duration = r1dur;
		}

		#endregion

		#region Class-Members

		public static Rate Factory() {
			Rate result = new Rate(
				id: -1,
				name: "",
				abrv: "",
				r2dur: -1,
				r1dur: -1
			);
			return result;
		}

		public static Rate Factory(int id, string name, string abrv, long r2dur,
			long r1dur) {
			Rate result = new Rate(
				id: id,
				name: name,
				abrv: abrv,
				r2dur: r2dur,
				r1dur: r1dur
			);
			return result;
		}

		public static Rate Factory(SQLiteDataReader reader) {
			Rate result = new Rate(
				id: Convert.ToInt32(reader["id"]),
				name: (string)reader["name"],
				abrv: (string)reader["abrv"],
				r2dur: Convert.ToInt64(reader["rank2duration"]),
				r1dur: Convert.ToInt64(reader["rank1duration"])
			);
			return result;
		}

		/// <summary>
		/// Creates a new rate
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name"></param>
		/// <param name="abrv"></param>
		/// <param name="r2dur">Time before rank 2 expiration in seconds</param>
		/// <param name="r1dur">Time before rank 1 expiration in seconds</param>
		/// <returns></returns>
		public static bool Create(ref Rate output, string name, string abrv, 
			long r2dur, long r1dur) {
			int result = DBI.DoAction(
				$@"INSERT INTO Rate (name, abrv, rank2duration, rank1duration) 
				VALUES ('{name}', '{abrv}', {r2dur}, {r1dur});");
			if (result == 1) {
				return Rate.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		/// <summary>
		/// Gets all possible rates
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public static bool FetchAll(ref List<Rate> output) {
			output = new List<Rate>();

			SQLiteDataReader reader = DBI.DoQuery(
				$"SELECT * FROM Rate ORDER BY id ASC;");
			while (reader.Read()) {
				output.Add(Rate.Factory(reader));
			}

			return true;
		}

		/// <summary>
		/// Gets a rate by ID
		/// </summary>
		/// <param name="output"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool FetchById(ref Rate output, int id) {
			SQLiteDataReader reader = DBI.DoQuery(
				$"SELECT * FROM Rate WHERE id = {id} LIMIT 1;");
			if (reader.Read()) {
				output = Rate.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets a rate by name
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool FetchByName(ref Rate output, string name) {
			SQLiteDataReader reader = DBI.DoQuery(
				$"SELECT * FROM Rate WHERE name = {name} LIMIT 1;");
			if (reader.Read()) {
				output = Rate.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Updates a rate
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool Store(Rate input) {
			int result = DBI.DoAction(
				$@"UPDATE Rate SET name = '{input.name}', abrv = '{input.abrv}',
				rank2duration = {input.rank2duration}, 
				rank1duration = {input.rank1duration} 
				WHERE id = {input.id};");
			if (result == 1)
				return true;
			return false;
		}

		#endregion
	}
}