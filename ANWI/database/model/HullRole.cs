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
        public string abrv;
        public string icon;

        private HullRole(int id, string name, string abrv, string icon)
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

        public static HullRole Factory()
        {
            HullRole result = new HullRole(
                id: -1,
                name: "",
                abrv: "",
                icon: ""
            );
            return result;
        }

        public static HullRole Factory(int id, string name, string abrv, string icon)
        {
            HullRole result = new HullRole(
                id: id,
                name: name,
                abrv: abrv,
                icon: icon
            );
            return result;
        }

        public static HullRole Factory(SQLiteDataReader reader)
        {
            HullRole result = new HullRole(
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