using System;
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
                    User.FetchById(ref _user, user);
                return _user;
            }
            set
            {
                _user = value;
            }
        }

        public Hull Hull
        {
            get
            {
                if (_hull == null)
                    Hull.FetchById(ref _hull, hull);
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
                id: Convert.ToInt32(reader["id"]),
                user: Convert.ToInt32(reader["user"]),
                hull: Convert.ToInt32(reader["hull"]),
                insurance: Convert.ToInt32(reader["insurance"]),
                number: Convert.ToInt32(reader["number"]),
                name: (string)reader["name"],
                User: null,
                Hull: null
            );
            return result;
        }

        public static bool Create(ref UserShip output, int user, int hull, int insurance, int number, string name)
        {
            int result = DBI.DoAction($"insert into UserShip (user, hull, insurance, number, name) values ({user}, {hull}, {insurance}, {number}, '{name}');");
            if (result == 1)
            {
                return UserShip.FetchById(ref output, DBI.LastInsertRowId);
            }
            return false;
        }

        public static bool FetchById(ref UserShip output, int id)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from UserShip where id = {id} limit 1;");
            if ( reader.Read() )
            {
                output = UserShip.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool FetchByName(ref UserShip output, string name)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from UserShip where name = '{name}' limit 1;");
            if ( reader.Read() )
            {
                output = UserShip.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool Store(UserShip input)
        {
            int result = DBI.DoAction($"update UserShip set name = '{input.name}', user = {input.user}, hull = {input.hull}, insurance = {input.insurance}, number = {input.number}, name = '{input.name}' where id = {input.id};");
            if (result == 1)
                return true;
            return false;
        }

        #endregion
    }
}