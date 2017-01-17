using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the StruckRates table.
    /// </summary>

    public class StruckRate
    {
        public static StruckRate Factory()
        {
            StruckRate result = new StruckRate(-1, -1, -1, 0, null, null);
            return result;
        }

        public static StruckRate Factory(int _id, int _user_id, int _rate_id, int _rank)
        {
            StruckRate result = new StruckRate(_id, _user_id, _rate_id, _rank, null, null);
            return result;
        }

        public static StruckRate Factory(SQLiteDataReader reader)
        {
            StruckRate result = new StruckRate(
                (int)reader["id"],
                (int)reader["user_id"],
                (int)reader["rate_id"],
                (int)reader["_rank"],
                null,
                null
            );
            return result;
        }

        public int id;
        public int user_id;
        public int rate_id;
        public int rank;

        User user_object;
        Rate rate_object;

        private StruckRate(int _id, int _user_id, int _rate_id, int _rank, User _user_object, Rate _rate_object)
        {
            id = _id;
            user_id = _user_id;
            rate_id = _rate_id;
            rank = _rank;
            user_object = _user_object;
            rate_object = _rate_object;
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

        public Rate rate
        {
            get
            {
                if (rate_object == null)
                    DBI.GetRateById(rate_id, out rate_object);
                return rate_object;
            }
            set
            {
                rate_object = value;
            }
        }
    }
}
