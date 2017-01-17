using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the HullRoles table.
    /// </summary>
    
    public class HullRole
    {
        public static HullRole Factory()
        {
            HullRole result = new HullRole(-1, "", "", "");
            return result;
        }

        public static HullRole Factory(int _id, string _name, string _abbreviation, string _icon_name)
        {
            HullRole result = new HullRole(_id, _name, _abbreviation, _icon_name);
            return result;
        }

        public static HullRole Factory(SQLiteDataReader reader)
        {
            HullRole result = new HullRole(
                (int)reader["id"],
                (string)reader["name"],
                (string)reader["abbreviation"],
                (string)reader["icon_name"]
            );
            return result;
        }

        public int id;
        public string name;
        public string abbreviation;
        public string icon_name;

        private HullRole(int _id, string _name, string _abbreviation, string _icon_name)
        {
            id = _id;
            name = _name;
            abbreviation = _abbreviation;
            icon_name = _icon_name;
        }
    }
}
