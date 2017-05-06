using System;
using System.Data.SQLite;

namespace ANWI.Database.Model {
	/// <summary>
	/// Represents a row of the HullVendor table.
	/// </summary>

	public class HullVendor {
		#region Model

		public int id;
		public string name;
		public string abrv;
		public string icon;

		private HullVendor(int id, string name, string abrv, string icon) {
			this.id = id;
			this.name = name;
			this.abrv = abrv;
			this.icon = icon;
		}

		#endregion

		#region Class-Members

		public static HullVendor Factory() {
			HullVendor result = new HullVendor(
				id: -1,
				name: "",
				abrv: "",
				icon: ""
			);
			return result;
		}

		public static HullVendor Factory(int id, string name, string abrv, 
			string icon) {
			HullVendor result = new HullVendor(
				id: id,
				name: name,
				abrv: abrv,
				icon: icon
			);
			return result;
		}

		public static HullVendor Factory(SQLiteDataReader reader) {
			HullVendor result = new HullVendor(
				id: Convert.ToInt32(reader["id"]),
				name: (string)reader["name"],
				abrv: (string)reader["abrv"],
				icon: (string)reader["icon"]
			);
			return result;
		}

		/// <summary>
		/// Creates a new Hull Vendor
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name"></param>
		/// <param name="abrv"></param>
		/// <param name="icon"></param>
		/// <returns></returns>
		public static bool Create(ref HullVendor output, string name, 
			string abrv, string icon = "") {
			int result = DBI.DoAction(
				$@"INSERT INTO HullVendor (name, abrv, icon) 
				VALUES ('{name}', '{abrv}', '{icon}');");
			if (result == 1) {
				return HullVendor.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		/// <summary>
		/// Gets a vendor by ID
		/// </summary>
		/// <param name="output"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool FetchById(ref HullVendor output, int id) {
			SQLiteDataReader reader = DBI.DoQuery(
				$@"SELECT * FROM HullVendor 
				WHERE id = {id} LIMIT 1;");
			if (reader != null && reader.Read()) {
				output = HullVendor.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets a vendor by name
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool FetchByName(ref HullVendor output, string name) {
			SQLiteDataReader reader = DBI.DoQuery(
				$@"SELECT * FROM HullVendor 
				WHERE name = {name} LIMIT 1;");
			if (reader != null && reader.Read()) {
				output = HullVendor.Factory(reader);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Updates a vendor
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool Store(HullVendor input) {
			int result = DBI.DoAction(
				$@"UPDATE HullVendor 
				SET name = '{input.name}', abrv = '{input.abrv}', 
				icon = '{input.icon}' 
				WHERE id = {input.id};");
			if (result == 1)
				return true;
			return false;
		}

		#endregion
	}
}