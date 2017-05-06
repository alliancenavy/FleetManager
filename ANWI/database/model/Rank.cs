using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ANWI.Database.Model {
	/// <summary>
	/// Represents a row of the Rank table.
	/// </summary>

	public class Rank {
		#region Model

		public int id;
		public string name;
		public string abrv;
		public string icon;
		public int ordering;

		private Rank(int id, string name, string abrv, string icon, 
			int ordering) {
			this.id = id;
			this.name = name;
			this.abrv = abrv;
			this.icon = icon;
			this.ordering = ordering;
		}

		#endregion

		#region Class-Members

		public static Rank Factory() {
			Rank result = new Rank(
				id: -1,
				name: "",
				abrv: "",
				icon: "",
				ordering: 0
			);
			return result;
		}

		public static Rank Factory(int id, string name, string abrv, 
			string icon, int ordering) {
			Rank result = new Rank(
				id: id,
				name: name,
				abrv: abrv,
				icon: icon,
				ordering: ordering
			);
			return result;
		}

		public static Rank Factory(SQLiteDataReader reader) {
			Rank result = new Rank(
				id: Convert.ToInt32(reader["id"]),
				name: (string)reader["name"],
				abrv: (string)reader["abrv"],
				icon: (string)reader["icon"],
				ordering: Convert.ToInt32(reader["ordering"])
			);
			return result;
		}

		/// <summary>
		/// Creates a new rank
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name"></param>
		/// <param name="abrv"></param>
		/// <param name="icon"></param>
		/// <param name="ordering"></param>
		/// <returns></returns>
		public static bool Create(ref Rank output, string name, string abrv, 
			string icon, int ordering) {
			int result = DBI.DoAction(
				$@"INSERT INTO Rank (name, abrv, icon, ordering) 
				VALUES ('{name}', '{abrv}', '{icon}', {ordering});");
			if (result == 1) {
				return Rank.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		/// <summary>
		/// Returns all possible ranks
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public static bool FetchAll(ref List<Rank> output) {
			output = new List<Rank>();

			SQLiteDataReader reader = DBI.DoQuery(
				$"SELECT * FROM Rank ORDER BY ordering ASC");
			while (reader != null && reader.Read()) {
				output.Add(Rank.Factory(reader));
			}

			return true;
		}

		/// <summary>
		/// Gets a rank by ID
		/// </summary>
		/// <param name="output"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool FetchById(ref Rank output, int id) {
			SQLiteDataReader reader = DBI.DoQuery(
				$"SELECT * FROM Rank WHERE id = {id} LIMIT 1;");
			if (reader != null && reader.Read()) {
				output = Rank.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets a rank by name
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool FetchByName(ref Rank output, string name) {
			SQLiteDataReader reader = DBI.DoQuery(
				$"SELECT * FROM Rank WHERE name = {name} LIMIT 1;");
			if (reader != null && reader.Read()) {
				output = Rank.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Updates a rank
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool Store(Rank input) {
			int result = DBI.DoAction(
				$@"UPDATE Rank SET name = '{input.name}', abrv = '{input.abrv}',
				icon = '{input.icon}' 
				WHERE id = {input.id};");
			if (result == 1)
				return true;
			return false;
		}

		#endregion
	}
}