using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the Users table.
    /// </summary>

    public class User
    {
        public static User Factory()
        {
            User result = new User(-1, "", 0, "", -1, -1, -1, null, null, null);
            return result;
        }

        public static User Factory(int _id, string _name, int _joined, string _auth0_id, int _rank_id, int _primary_rate_id, int _assigned_ship_id)
        {
            User result = new User(_id, _name, _joined, _auth0_id, _rank_id, _primary_rate_id, _assigned_ship_id, null, null, null);
            return result;
        }

        public static User Factory(SQLiteDataReader reader)
        {
            User result = new User(
                (int)reader["id"],
                (string)reader["name"],
                (int)reader["joined"],
                (string)reader["auth0_id"],
                (int)reader["rank_id"],
                (int)reader["primary_rate_id"],
                (int)reader["assigned_ship_id"],
                null,
                null,
                null
            );
            return result;
        }

        public int id;
        public string name;
        public int joined;
        public string auth0_id;
        public int rank_id;
        public int primary_rate_id;
        public int assigned_ship_id;

        Rank rank_object;
        StruckRate primary_rate_object;
        UserShip assigned_ship_object;

        private User(int _id, string _name, int _joined, string _auth0_id, int _rank_id, int _primary_rate_id, int _assigned_ship_id, Rank _rank_object, StruckRate _primary_rate_object, UserShip _assigned_ship_object)
        {
            id = _id;
            name = _name;
            joined = _joined;
            auth0_id = _auth0_id;
            rank_id = _rank_id;
            primary_rate_id = _primary_rate_id;
            assigned_ship_id = _assigned_ship_id;
            rank_object = _rank_object;
            primary_rate_object = _primary_rate_object;
            assigned_ship_object = _assigned_ship_object;
        }

        public Rank rank
        {
            get
            {
                if (rank_object == null)
                    DBI.GetRankById(rank_id, out rank_object);
                return rank_object;
            }
            set
            {
                rank_object = value;
            }
        }

        public StruckRate primary_rate
        {
            get
            {
                if (primary_rate_object == null)
                    DBI.GetStruckRateById(primary_rate_id, out primary_rate_object);
                return primary_rate_object;
            }
            set
            {
                primary_rate_object = value;
            }
        }

        public UserShip assigned_ship
        {
            get
            {
                if (assigned_ship_object == null)
                    DBI.GetUserShipById(assigned_ship_id, out assigned_ship_object);
                return assigned_ship_object;
            }
            set
            {
                assigned_ship_object = value;
            }
        }
    }
}
