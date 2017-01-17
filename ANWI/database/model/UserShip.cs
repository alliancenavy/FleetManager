using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the UserShip table.
    /// </summary>

    public class UserShip
    {
        #region Model

        public int id;
        public int user;
        public int hull;
        public int insurance;
        public int number;
        public string name;

        private User _user;
        private Hull _hull;

        private UserShip(int id, int user, int hull, int insurance, int number, string name, User User, Hull Hull)
        {
            this.id = id;
            this.user = user;
            this.hull = hull;
            this.insurance = insurance;
            this.number = number;
            this.name = name;
            this._user = User;
            this._hull = Hull;
        }

        #endregion

        #region Instance-Members

        public User User
        {
            get
            {
                if (_user == null)
                    DBI.GetUserById(user, out _user);
                return _user;
            }
            set
            {
                _user = value;
                user = _user.id;
            }
        }

        public Hull Hull
        {
            get
            {
                if (_hull == null)
                    DBI.GetHullById(hull, out _hull);
                return _hull;
            }
            set
            {
                _hull = value;
                hull = _hull.id;
            }
        }

        #endregion

        #region Class-Members

        public static UserShip Factory()
        {
            UserShip result = new UserShip(
                id: -1,
                user: -1,
                hull: -1,
                insurance: 0,
                number: 0,
                name: "",
                User: null,
                Hull: null
            );
            return result;
        }

        public static UserShip Factory(int id, int user, int hull, int insurance, int number, string name)
        {

            UserShip result = new UserShip(
                id: id,
                user: user,
                hull: hull,
                insurance: insurance,
                number: number,
                name: name,
                User: null,
                Hull: null
            );
            return result;
        }

        public static UserShip Factory(SQLiteDataReader reader)
        {
            UserShip result = new UserShip(
                id: (int)reader["id"],
                user: (int)reader["user"],
                hull: (int)reader["hull"],
                insurance: (int)reader["insurance"],
                number: (int)reader["number"],
                name: (string)reader["name"],
                User: null,
                Hull: null
            );
            return result;
        }

        public static bool CreateEntry(out UserShip result, int user, int hull, int insurance, int number, string name)
        {
            int outval = DBI.DoAction("insert into UserShip (user, hull, insurance, number, name) values ("
                + user + ", "  + hull + ", " + insurance + ", " + number + ", " + name + ");"
            );
            result = UserShip.Factory();
            return true;
        }

        #endregion
    }
}