using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the HullTypes table.
    /// </summary>

    public class HullType
    {
        public static HullType Factory()
        {
            HullType result = new HullType(-1, "");
            return result;
        }

        public static HullType Factory(int _id, string _name)
        {
            HullType result = new HullType(_id, _name);
            return result;
        }

        public static HullType Factory(SQLiteDataReader reader)
        {
            HullType result = new HullType(
                (int)reader["id"],
                (string)reader["name"]
            );
            return result;
        }

        public int id;
        public string name;

        private HullType(int _id, string _name)
        {
            id = _id;
            name = _name;
        }
    }
}
