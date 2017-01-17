using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the User table.
    /// </summary>

    public class User
    {
        #region Model

        public int id;
        public string name;
        public string auth0;
        public int rank;
        public int rate;

        private Rank _rank;
        private StruckRate _rate;

        private User(int id, string name, string auth0, int rank, int rate, Rank Rank, StruckRate Rate)
        {
            this.id = id;
            this.name = name;
            this.auth0 = auth0;
            this.rank = rank;
            this.rate = rate;

            this._rank = Rank;
            this._rate = Rate;
        }

        #endregion

        #region Instance-Members

        public Rank Rank
        {
            get
            {
                if (_rank == null)
                    DBI.GetRankById(rank, out _rank);
                return _rank;
            }
            set
            {
                _rank = value;
                rank = _rank.id;
            }
        }

        public StruckRate Rate
        {
            get
            {
                if (_rate == null)
                    DBI.GetStruckRateById(rate, out _rate);
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

        public static User Factory()
        {
            User result = new User(
                id: -1,
                name: "",
                auth0: "",
                rank: -1,
                rate: -1,

                Rank: null,
                Rate: null
            );
            return result;
        }

        public static User Factory(int id, string name, string auth0, int rank, int rate)
        {

            User result = new User(
                id: id,
                name: name,
                auth0: auth0,
                rank: rank,
                rate: rate,

                Rank: null,
                Rate: null
            );
            return result;
        }

        public static User Factory(SQLiteDataReader reader)
        {
            User result = new User(
                id: (int)reader["id"],
                name: (string)reader["name"],
                auth0: (string)reader["auth0"],
                rank: (int)reader["rank"],
                rate: (int)reader["rate"],

                Rank: null,
                Rate: null
            );
            return result;
        }

        #endregion
    }
}