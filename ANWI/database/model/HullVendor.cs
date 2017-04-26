using System;
using System.Data.SQLite;

namespace ANWI.Database.Model
{
	/// <summary>
	/// Represents a row of the HullVendor table.
	/// </summary>

	public class HullVendor
	{
		#region Model

		public int id;
		public string name;
		public string abrv;
		public string icon;

		private HullVendor(int id, string name, string abrv, string icon)
		{
			this.id = id;
			this.name = name;
			this.abrv = abrv;
			this.icon = icon;
		}

		#endregion

		#region Class-Members

		public static HullVendor Factory()
		{
			HullVendor result = new HullVendor(
				id: -1,
				name: "",
				abrv: "",
				icon: ""
			);
			return result;
		}

		public static HullVendor Factory(int id, string name, string abrv, string icon)
		{
			HullVendor result = new HullVendor(
				id: id,
				name: name,
				abrv: abrv,
				icon: icon
			);
			return result;
		}

		public static HullVendor Factory(SQLiteDataReader reader)
		{
			HullVendor result = new HullVendor(
				id: Convert.ToInt32(reader["id"]),
				name: (string)reader["name"],
				abrv: (string)reader["abrv"],
				icon: (string)reader["icon"]
			);
			return result;
		}

		public static bool Create(ref HullVendor output, string name, string abrv, string icon = "")
		{
			int result = DBI.DoAction($"insert into HullVendor (name, abrv, icon) values('{name}', '{abrv}', '{icon}');");
			if (result == 1)
			{
				return HullVendor.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		public static bool FetchById(ref HullVendor output, int id)
		{
			SQLiteDataReader reader = DBI.DoQuery($"select * from HullVendor where id = {id} limit 1;");
			if (reader.Read())
			{
				output = HullVendor.Factory(reader);
				return true;
			}
			return false;
		}

		public static bool FetchByName(ref HullVendor output, string name)
		{
			SQLiteDataReader reader = DBI.DoQuery($"select * from HullVendor where name = {name} limit 1;");
			if (reader.Read())
			{
				output = HullVendor.Factory(reader);
				return true;
			}
			return false;
		}

		public static bool Store(HullVendor input)
		{
			int result = DBI.DoAction($"update HullVendor set name = '{input.name}', abrv = '{input.abrv}', icon = '{input.icon}' where id = {input.id};");
			if (result == 1)
				return true;
			return false;
		}

		#endregion
	}
}