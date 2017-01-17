using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the HullMakers table.
    /// </summary>

    public class HullManufacturer
    {
        public static HullManufacturer Factory()
        {
            HullManufacturer result = new HullManufacturer(-1, "", "", "");
            return result;
        }

        public static HullManufacturer Factory(int _id, string _name, string _abbreviation, string _icon_name)
        {
            HullManufacturer result = new HullManufacturer(_id, _name, _abbreviation, _icon_name);
            return result;
        }

        public static HullManufacturer Factory(SQLiteDataReader reader)
        {
            HullManufacturer result = new HullManufacturer(
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

        private HullManufacturer(int _id, string _name, string _abbreviation, string _icon_name)
        {
            id = _id;
            name = _name;
            abbreviation = _abbreviation;
            icon_name = _icon_name;
        }
    }
}
