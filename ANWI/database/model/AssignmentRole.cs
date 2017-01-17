using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the AssignmentRole table.
    /// </summary>

    public class AssignmentRole
    {
        #region Model

        public int id;
        public string name;
        public int rate;

        private Rate _rate;

        private AssignmentRole(int id, string name, int rate, Rate Rate)
        {
            this.id = id;
            this.name = name;
            this.rate = rate;

            this._rate = Rate;
        }

        #endregion

        #region Instance-Members

        public Rate Rate
        {
            get
            {
                if (_rate == null)
                    DBI.GetRateById(rate, out _rate);
                return _rate;
            }
            set
            {
                _rate = value;
                rate = _rate.id;
            }
        }

        #endregion

        #region Class-Members

        public static AssignmentRole Factory()
        {
            AssignmentRole result = new AssignmentRole(
                id: -1,
                name: "",
                rate: -1,

                Rate: null
            );
            return result;
        }

        public static AssignmentRole Factory(int id, string name, int rate)
        {
            AssignmentRole result = new AssignmentRole(
                id: id,
                name: name,
                rate: rate,

                Rate: null
            );
            return result;
        }

        public static AssignmentRole Factory(SQLiteDataReader reader)
        {
            AssignmentRole result = new AssignmentRole(
                id: (int)reader["id"],
                name: (string)reader["name"],
                rate: (int)reader["rate"],

                Rate: null
            );
            return result;
        }

        #endregion
    }
}