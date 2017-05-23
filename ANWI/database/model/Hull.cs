using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ANWI.Database.Model {
	/// <summary>
	/// Represents a row of the Hull table.
	/// </summary>

	public class Hull {
		#region Model

		public int id;
		public int vendor;
		public int role;
		public string series;
		public string symbol;
		public int ordering;

		private Hull(int id, int vendor, int role, string series, 
			string symbol, int ordering) {
			this.id = id;
			this.vendor = vendor;
			this.role = role;
			this.series = series;
			this.symbol = symbol;
			this.ordering = ordering;
		}

		#endregion

		#region Class-Members

		public static Hull Factory() {
			Hull result = new Hull(
				id: -1,
				vendor: -1,
				role: -1,
				series: "",
				symbol: "",
				ordering: 0
			);
			return result;
		}

		public static Hull Factory(int id, int vendor, int role, string series, 
			string version, string symbol, int ordering) {
			Hull result = new Hull(
				id: id,
				vendor: vendor,
				role: role,
				series: series,
				symbol: symbol,
				ordering: ordering
			);
			return result;
		}

		public static Hull Factory(SQLiteDataReader reader) {
			Hull result = new Hull(
				id: Convert.ToInt32(reader["id"]),
				vendor: Convert.ToInt32(reader["vendor"]),
				role: Convert.ToInt32(reader["role"]),
				series: (string)reader["series"],
				symbol: (string)reader["symbol"],
				ordering: Convert.ToInt32(reader["ordering"])
			);
			return result;
		}

		/// <summary>
		/// Creates a new hull
		/// </summary>
		/// <param name="output"></param>
		/// <param name="vendor"></param>
		/// <param name="role"></param>
		/// <param name="series"></param>
		/// <param name="version"></param>
		/// <param name="symbol"></param>
		/// <param name="ordering"></param>
		/// <returns></returns>
		public static bool Create(ref Hull output, int vendor, int role, 
			string series, string version, string symbol, int ordering) {
			int result = DBI.DoPreparedAction(
				@"INSERT INTO Hull (vendor, role, series, symbol, ordering) 
				VALUES (@vendor, @role, @series, @symbol, @ordering);",
				new Tuple<string, object>("@vendor", vendor), 
				new Tuple<string, object>("@role", role), 
				new Tuple<string, object>("@series", series), 
				new Tuple<string, object>("@symbol", symbol),
				new Tuple<string, object>("@ordering", ordering));
			if (result == 1) {
				return Hull.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		/// <summary>
		/// Fetches a hull by ID
		/// </summary>
		/// <param name="output"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool FetchById(ref Hull output, int id) {
			SQLiteDataReader reader = DBI.DoPreparedQuery(
				@"SELECT * FROM Hull 
				WHERE id = @id LIMIT 1;",
				new Tuple<string, object>("@id", id));
			if (reader != null && reader.Read()) {
				output = Hull.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Fetches all possible hulls
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public static bool FetchAll(ref List<Hull> output) {
			output = new List<Hull>();

			SQLiteDataReader reader = DBI.DoQuery(
				"SELECT * FROM Hull ORDER BY ordering ASC;");
			while (reader != null && reader.Read()) {
				Hull h = Hull.Factory(reader);
				output.Add(h);
			}
			return true;
		}

		/// <summary>
		/// Fetches only large, named ships.  Their current cutoff value in 
		/// the ordering column is 100.
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public static bool FetchLarge(ref List<Hull> output) {
			output = new List<Hull>();

			SQLiteDataReader reader = DBI.DoQuery(
				@"SELECT * FROM Hull 
				WHERE ordering <= 100 ORDER BY series ASC;");
			while (reader != null && reader.Read()) {
				Hull h = Hull.Factory(reader);
				output.Add(h);
			}
			return true;
		}

		/// <summary>
		/// Fetches all small, unnamed ships such as fighters and gunships.
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public static bool FetchSmall(ref List<Hull> output) {
			output = new List<Hull>();

			SQLiteDataReader reader = DBI.DoQuery(
				@"SELECT * FROM Hull 
				WHERE ordering > 100 ORDER BY series ASC;");
			while (reader != null && reader.Read()) {
				Hull h = Hull.Factory(reader);
				output.Add(h);
			}
			return true;
		}

		/// <summary>
		/// Updates a hull in the database
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool Store(Hull input) {
			int result = DBI.DoPreparedAction(
				@"UPDATE Hull SET vendor = @vendor, role = @role,
				series = @series, symbol = @symbol, ordering = @ordering 
				WHERE id = @id;",
				new Tuple<string, object>("@vendor", input.vendor), 
				new Tuple<string, object>("@role", input.role), 
				new Tuple<string, object>("@series", input.series), 
				new Tuple<string, object>("@symbol", input.symbol),
				new Tuple<string, object>("@ordering", input.ordering), 
				new Tuple<string, object>("@id", input.id));
			if (result == 1)
				return true;
			return false;
		}

		#endregion
	}
}