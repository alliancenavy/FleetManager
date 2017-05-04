using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ANWI.Database.Model
{
	/// <summary>
	/// Represents a row of the Hull table.
	/// </summary>

	public class Hull
	{
		#region Model

		public int id;
		public int vendor;
		public int role;
		public string series;
		public string symbol;
		public int ordering;

		private Hull(int id, int vendor, int role, string series, string symbol, int ordering)
		{
			this.id = id;
			this.vendor = vendor;
			this.role = role;
			this.series = series;
			this.symbol = symbol;
			this.ordering = ordering;
		}

		#endregion

		#region Class-Members

		public static Hull Factory()
		{
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

		public static Hull Factory(int id, int vendor, int role, string series, string version, string symbol, int ordering)
		{
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

		public static Hull Factory(SQLiteDataReader reader)
		{
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

		public static bool Create(ref Hull output, int vendor, int role, string series, string version, string symbol, int ordering)
		{
			int result = DBI.DoAction($"insert into Hull (vendor, role, series, symbol, ordering) values({vendor}, {role}, '{series}', '{symbol}', {ordering});");
			if (result == 1)
			{
				return Hull.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		public static bool FetchById(ref Hull output, int id)
		{
			SQLiteDataReader reader = DBI.DoQuery($"select * from Hull where id = {id} limit 1;");
			if (reader.Read())
			{
				output = Hull.Factory(reader);
				return true;
			}
			return false;
		}

		public static bool FetchAll(ref List<Hull> output) {
			output = new List<Hull>();

			SQLiteDataReader reader = DBI.DoQuery($"SELECT * FROM Hull ORDER BY ordering ASC;");
			while(reader.Read()) {
				Hull h = Hull.Factory(reader);
				output.Add(h);
			}
			return true;
		}

		public static bool FetchLarge(ref List<Hull> output) {
			output = new List<Hull>();

			SQLiteDataReader reader = DBI.DoQuery($"SELECT * FROM Hull WHERE ordering <= 100 ORDER BY series ASC;");
			while (reader.Read()) {
				Hull h = Hull.Factory(reader);
				output.Add(h);
			}
			return true;
		}

		public static bool FetchSmall(ref List<Hull> output) {
			output = new List<Hull>();

			SQLiteDataReader reader = DBI.DoQuery($"SELECT * FROM Hull WHERE ordering > 100 ORDER BY series ASC;");
			while (reader.Read()) {
				Hull h = Hull.Factory(reader);
				output.Add(h);
			}
			return true;
		}

		public static bool Store(Hull input)
		{
			int result = DBI.DoAction($"update Hull set vendor = {input.vendor}, role = {input.role}, series = '{input.series}', symbol = '{input.symbol}', ordering = {input.ordering} where id = {input.id};");
			if (result == 1)
				return true;
			return false;
		}

		#endregion
	}
}