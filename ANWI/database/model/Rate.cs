using System;
using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the Rate table.
    /// </summary>

    public class Rate
    {
        #region Model

        public int id;
        public string name;
        public string abrv;
        public string icon;

        private Rate(int id, string name, string abrv, string icon)
        {
            this.id = id;
            this.name = name;
            this.abrv = abrv;
            this.icon = icon;
        }

        #endregion

        #region Instance-Members



        #endregion

        #region Class-Members

        public static Rate Factory()
        {
            Rate result = new Rate(
                id: -1,
                name: "",
                abrv: "",
                icon: ""
            );
            return result;
        }

        public static Rate Factory(int id, string name, string abrv, string icon)
        {
            Rate result = new Rate(
                id: id,
                name: name,
                abrv: abrv,
                icon: icon
            );
            return result;
        }

        public static Rate Factory(SQLiteDataReader reader)
        {
            Rate result = new Rate(
                id: Convert.ToInt32(reader["id"]),
                name: (string)reader["name"],
                abrv: (string)reader["abrv"],
                icon: (string)reader["icon"]
            );
            return result;
        }

        public static bool Create(ref Rate output, string name, string abrv, string icon = "")
        {
            int result = DBI.DoAction($"insert into Rate (name, abrv, icon) values('{name}', '{abrv}', '{icon}');");
            if (result == 1)
            {
                return Rate.FetchById(ref output, DBI.LastInsertRowId);
            }
            return false;
        }

        public static bool FetchById(ref Rate output, int id)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from Rate where id = {id} limit 1;");
            if ( reader.Read() )
            {
                output = Rate.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool FetchByName(ref Rate output, string name)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from Rate where name = {name} limit 1;");
            if ( reader.Read() )
            {
                output = Rate.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool Store(Rate input)
        {
            int result = DBI.DoAction($"update Rate set name = '{input.name}', abrv = '{input.abrv}', icon = '{input.icon}' where id = {input.id};");
            if (result == 1)
                return true;
            return false;
        }

        #endregion
    }
}