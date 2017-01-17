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

        #endregion
    }
}