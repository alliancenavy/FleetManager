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
                    Rate.FetchById(ref _rate, rate);
                return _rate;
            }
            set
            {
                _rate = value;
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

        public static bool Create(ref AssignmentRole output, string name, int rate)
        {
            int result = DBI.DoAction($"insert into AssignmentRole (name, rate) values('{name}', {rate});");
            if (result == 1)
            {
                return AssignmentRole.FetchById(ref output, DBI.LastInsertRowId);
            }
            return false;
        }

        public static bool FetchById(ref AssignmentRole output, int id)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from AssignmentRole where id = {id} limit 1;");
            if (reader.Read())
            {
                output = AssignmentRole.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool FetchByName(ref AssignmentRole output, string name)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from AssignmentRole where name = {name} limit 1;");
            if (reader.Read())
            {
                output = AssignmentRole.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool Store(AssignmentRole input)
        {
            int result = DBI.DoAction($"update AssignmentRole set name = '{input.name}', rate = {input.rate} where id = {input.id};");
            if (result == 1)
                return true;
            return false;
        }

        #endregion
    }
}