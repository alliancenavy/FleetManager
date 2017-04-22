using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the Rank table.
    /// </summary>

    public class Rank
    {
        #region Model

        public int id;
        public string name;
        public string abrv;
        public string icon;
        public int ordering;

        private Rank(int id, string name, string abrv, string icon, int ordering)
        {
            this.id = id;
            this.name = name;
            this.abrv = abrv;
            this.icon = icon;
            this.ordering = ordering;
        }

        #endregion

        #region Instance-Members



        #endregion

        #region Class-Members

        public static Rank Factory()
        {
            Rank result = new Rank(
                id: -1,
                name: "",
                abrv: "",
                icon: "",
                ordering: 0
            );
            return result;
        }

        public static Rank Factory(int id, string name, string abrv, string icon, int ordering)
        {
            Rank result = new Rank(
                id: id,
                name: name,
                abrv: abrv,
                icon: icon,
                ordering: ordering
            );
            return result;
        }

        public static Rank Factory(SQLiteDataReader reader)
        {
            Rank result = new Rank(
                id: Convert.ToInt32(reader["id"]),
                name: (string)reader["name"],
                abrv: (string)reader["abrv"],
                icon: (string)reader["icon"],
                ordering: Convert.ToInt32(reader["ordering"])
            );
            return result;
        }

        public static bool Create(ref Rank output, string name, string abrv, string icon, int ordering)
        {
            int result = DBI.DoAction($"insert into Rank (name, abrv, icon, ordering) values('{name}', '{abrv}', '{icon}', {ordering});");
            if (result == 1)
            {
                return Rank.FetchById(ref output, DBI.LastInsertRowId);
            }
            return false;
        }

		public static bool FetchAll(ref List<Rank> output) {
			output = new List<Rank>();

			SQLiteDataReader reader = DBI.DoQuery($"select * from Rank order by ordering asc");
			while(reader.Read()) {
				output.Add(Rank.Factory(reader));
			}

			return true;
		}

        public static bool FetchById(ref Rank output, int id)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from Rank where id = {id} limit 1;");
            if (reader.Read())
            {
                output = Rank.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool FetchByName(ref Rank output, string name)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from Rank where name = {name} limit 1;");
            if (reader.Read())
            {
                output = Rank.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool Store(Rank input)
        {
            int result = DBI.DoAction($"update Rank set name = '{input.name}', abrv = '{input.abrv}', icon = '{input.icon}' where id = {input.id};");
            if (result == 1)
                return true;
            return false;
        }

        #endregion
    }
}