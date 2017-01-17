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

        #region Instance-Members



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
                id: (int)reader["id"],
                name: (string)reader["name"],
                abrv: (string)reader["abrv"],
                icon: (string)reader["icon"]
            );
            return result;
        }

        #endregion
    }
}