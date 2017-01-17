using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the StruckRate table.
    /// </summary>

    public class StruckRate
    {
        #region Model

        public int id;
        public int user;
        public int rate;
        public int rank;

        private User _user;
        private Rate _rate;

        private StruckRate(int id, int user, int rate, int rank, User User, Rate Rate)
        {
            this.id = id;
            this.user = user;
            this.rate = rate;
            this.rank = rank;

            this._user = User;
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

        #endregion

        #region Class-Members

        public static StruckRate Factory()
        {
            StruckRate result = new StruckRate(
                id: -1,
                user: -1,
                rate: -1,
                rank: 0,

                User: null,
                Rate: null
            );
            return result;
        }

        public static StruckRate Factory(int id, int user, int rate, int rank)
        {
            StruckRate result = new StruckRate(
                id: id,
                user: user,
                rate: rate,
                rank: rank,

                User: null,
                Rate: null
            );
            return result;
        }

        public static StruckRate Factory(SQLiteDataReader reader)
        {
            StruckRate result = new StruckRate(
                id: (int)reader["id"],
                user: (int)reader["user"],
                rate: (int)reader["rate"],
                rank: (int)reader["rank"],

                User: null,
                Rate: null
            );
            return result;
        }

        #endregion
    }
}