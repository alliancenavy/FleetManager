using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the Ranks table.
    /// </summary>

    public class Rank
    {
        public static Rank Factory()
        {
            Rank result = new Rank(-1, "", "", 0, "");
            return result;
        }

        public static Rank Factory(int _id, string _name, string _abbreviation, int _ordering, string _icon_name)
        {
            Rank result = new Rank(_id, _name, _abbreviation, _ordering, _icon_name);
            return result;
        }

        public static Rank Factory(SQLiteDataReader reader)
        {
            Rank result = new Rank(
                (int)reader["id"],
                (string)reader["name"],
                (string)reader["abbreviation"],
                (int)reader["ordering"],
                (string)reader["icon_name"]
            );
            return result;
        }

        public int id;
        public string name;
        public string abbreviation;
        public int ordering;
        public string icon_name;

        private Rank(int _id, string _name, string _abbreviation, int _ordering, string _icon_name)
        {
            id = _id;
            name = _name;
            abbreviation = _abbreviation;
            ordering = _ordering;
            icon_name = _icon_name;
        }
    }
}
