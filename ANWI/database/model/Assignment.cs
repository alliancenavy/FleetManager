using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the Assignment table.
    /// </summary>

    public class Assignment
    {
        #region Model

        public int id;
        public int user;
        public int ship;
        public int role;
        public int from;
        public int until;

        private User _user;
        private UserShip _ship;
        private AssignmentRole _role;

        private Assignment(int id, int user, int ship, int role, int from, int until, User User, UserShip Ship, AssignmentRole Role)
        {
            this.id = id;
            this.user = user;
            this.ship = ship;
            this.role = role;
            this.from = from;
            this.until = until;

            this._user = User;
            this._ship = Ship;
            this._role = Role;
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

        public UserShip Ship
        {
            get
            {
                if (_ship == null)
                    DBI.GetUserShipById(ship, out _ship); ;
                return _ship;
            }
            set
            {
                _ship = value;
                ship = _ship.id;
            }
        }

        public AssignmentRole Role
        {
            get
            {
                if (_role == null)
                    DBI.GetAssignmentRoleById(role, out _role); ;
                return _role;
            }
            set
            {
                _role = value;
                role = _role.id;
            }
        }

        #endregion

        #region Class-Members

        public static Assignment Factory()
        {
            Assignment result = new Assignment(
                id: -1,
                user: -1,
                ship: -1,
                role: -1,
                from: 0,
                until: 0,

                User: null,
                Ship: null,
                Role: null
            );
            return result;
        }

        public static Assignment Factory(int id, int user, int ship, int role, int from, int until)
        {

            Assignment result = new Assignment(
                id: id,
                user: user,
                ship: ship,
                role: role,
                from: from,
                until: until,

                User: null,
                Ship: null,
                Role: null
            );
            return result;
        }

        public static Assignment Factory(SQLiteDataReader reader)
        {
            Assignment result = new Assignment(
                id: (int)reader["id"],
                user: (int)reader["user"],
                ship: (int)reader["ship"],
                role: (int)reader["role"],
                from: (int)reader["from"],
                until: (int)reader["until"],

                User: null,
                Ship: null,
                Role: null
            );
            return result;
        }

        #endregion
    }
}