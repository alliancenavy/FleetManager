using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the HullSeries table.
    /// </summary>

    public class HullSeries
    {
        #region Model

        public int id;
        public string name;

        private HullSeries(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        #endregion

        #region Instance-Members



        #endregion

        #region Class-Members

        public static HullSeries Factory()
        {
            HullSeries result = new HullSeries(
                id: -1,
                name: ""
            );
            return result;
        }

        public static HullSeries Factory(int id, string name)
        {
            HullSeries result = new HullSeries(
                id: id,
                name: name
            );
            return result;
        }

        public static HullSeries Factory(SQLiteDataReader reader)
        {
            HullSeries result = new HullSeries(
                id: (int)reader["id"],
                name: (string)reader["name"]
            );
            return result;
        }

        public static bool Create(ref HullSeries output, string name)
        {
            int result = DBI.DoAction($"insert into HullSeries (name) values('{name}');");
            if (result == 1)
            {
                return HullSeries.FetchById(ref output, DBI.LastInsertRowId);
            }
            return false;
        }

        public static bool FetchById(ref HullSeries output, int id)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from HullSeries where id = {id} limit 1;");
            if (reader.Read())
            {
                output = HullSeries.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool FetchByName(ref HullSeries output, string name)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from HullSeries where name = {name} limit 1;");
            if (reader.Read())
            {
                output = HullSeries.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool Store(HullSeries input)
        {
            int result = DBI.DoAction($"update HullSeries set name = '{input.name}' where id = {input.id};");
            if (result == 1)
                return true;
            return false;
        }

        #endregion
    }
}