using System;
using System.Data.SQLite;

namespace ANWI.Database.Model
{
	/// <summary>
	/// Represents a row of the HullRole table.
	/// </summary>

	public class HullRole
	{
		#region Model

		public int id;
		public string name;
		public string icon;

		private HullRole(int id, string name, string icon)
		{
			this.id = id;
			this.name = name;
			this.icon = icon;
		}

		#endregion

		#region Class-Members

		public static HullRole Factory()
		{
			HullRole result = new HullRole(
				id: -1,
				name: "",
				icon: ""
			);
			return result;
		}

		public static HullRole Factory(int id, string name, string icon)
		{
			HullRole result = new HullRole(
				id: id,
				name: name,
				icon: icon
			);
			return result;
		}

		public static HullRole Factory(SQLiteDataReader reader)
		{
			HullRole result = new HullRole(
				id: Convert.ToInt32(reader["id"]),
				name: (string)reader["name"],
				icon: (string)reader["icon"]
			);
			return result;
		}

		public static bool Create(ref HullRole output, string name, string icon = "")
		{
			int result = DBI.DoAction($"insert into HullRole (name, icon) values('{name}', '{icon}');");
			if (result == 1)
			{
				return HullRole.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		public static bool FetchById(ref HullRole output, int id)
		{
			SQLiteDataReader reader = DBI.DoQuery($"select * from HullRole where id = {id} limit 1;");
			if (reader.Read())
			{
				output = HullRole.Factory(reader);
				return true;
			}
			return false;
		}

		public static bool FetchByName(ref HullRole output, string name)
		{
			SQLiteDataReader reader = DBI.DoQuery($"select * from HullRole where name = {name} limit 1;");
			if (reader.Read())
			{
				output = HullRole.Factory(reader);
				return true;
			}
			return false;
		}

		public static bool Store(HullRole input)
		{
			int result = DBI.DoAction($"update HullRole set name = '{input.name}', icon = '{input.icon}' where id = {input.id};");
			if (result == 1)
				return true;
			return false;
		}

		#endregion
	}
}