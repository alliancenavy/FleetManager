using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the UserShips table.
    /// </summary>
    
    public class UserShip
    {
        public static UserShip Factory()
        {
            UserShip result = new UserShip(-1, -1, -1, false, "", 0, null, null);
            return result;
        }

        public static UserShip Factory(int _id, int _user_id, int _hull_id, bool _is_lti, string _name, int _hull_number)
        {
            UserShip result = new UserShip(_id, _user_id, _hull_id, _is_lti, _name, _hull_number, null, null);
            return result;
        }

        public static UserShip Factory(SQLiteDataReader reader)
        {
            UserShip result = new UserShip(
                (int)reader["id"],
                (int)reader["user_id"],
                (int)reader["hull_id"],
                (int)reader["is_lti"] == 1,
                (string)reader["name"],
                (int)reader["hull_number"],
                null,
                null
            );
            return result;
        }

        public int id;
        public int user_id;
        public int hull_id;
        public bool is_lti;
        public string name;
        public int hull_number;

        User user_object;
        Hull hull_object;

        private UserShip(int _id, int _user_id, int _hull_id, bool _is_lti, string _name, int _hull_number, User _user_object, Hull _hull_object)
        {
            id = _id;
            user_id = _user_id;
            hull_id = _hull_id;
            is_lti = _is_lti;
            name = _name;
            hull_number = _hull_number;
            user_object = _user_object;
            hull_object = _hull_object;
        }

        public User user
        {
            get
            {
                if (user_object == null)
                    DBI.GetUserById(user_id, out user_object);
                return user_object;
            }
            set
            {
                user_object = value;
            }
        }

        public Hull hull
        {
            get
            {
                if (hull_object == null)
                    DBI.GetHullById(user_id, out hull_object);
                return hull_object;
            }
            set
            {
                hull_object = value;
            }
        }
    }
}
