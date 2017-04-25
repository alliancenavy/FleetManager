using System;
using System.Collections.Generic;
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
		private UserPrivs _privs;

        private User(int id, string name, string auth0, int rank, int rate, Rank Rank, StruckRate Rate, UserPrivs privs)
        {
            this.id = id;
            this.name = name;
            this.auth0 = auth0;
            this.rank = rank;
            this.rate = rate;

            this._rank = Rank;
            this._rate = Rate;
			this._privs = privs;
        }

        #endregion

        #region Instance-Members

        public Rank Rank
        {
            get
            {
                if (_rank == null)
                    Rank.FetchById(ref _rank, rank);
                return _rank;
            }
            set
            {
                _rank = value;
            }
        }

        public StruckRate Rate
        {
            get
            {
                if (_rate == null)
                    StruckRate.FetchById(ref _rate, rate);
                return _rate;
            }
            set
            {
                _rate = value;
            }
        }

		public UserPrivs Privs {
			get {
				if (_privs == null)
					UserPrivs.FetchByUser(ref _privs, id);
				return _privs;
			}

			set {
				_privs = value;
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
                Rate: null,
				privs: null
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
                Rate: null,
				privs: null
            );
            return result;
        }

        public static User Factory(SQLiteDataReader reader)
        {
            User result = new User(
                id: Convert.ToInt32(reader["id"]),
                name: (string)reader["name"],
                auth0: (string)reader["auth0"],
                rank: Convert.ToInt32(reader["rank"]),
                rate: reader["rate"] is DBNull ? 0 : Convert.ToInt32(reader["rate"]),

                Rank: null,
                Rate: null,
				privs: null
            );
            return result;
        }

		public static bool Create(ref User output, string name, string auth0, int rank) {
			int result = DBI.DoAction($"insert into User (name, auth0, rank, rate) values ('{name}', '{auth0}', {rank}, null);");
			if (result == 1) {
				return User.FetchById(ref output, DBI.LastInsertRowId);
			}
			return false;
		}

		public static bool Create(ref User output, string name, string auth0, int rank, int rate)
        {
            int result = DBI.DoAction($"insert into User (name, auth0, rank, rate) values ('{name}', '{auth0}', {rank}, {rate});");
            if (result == 1)
            {
                return User.FetchById(ref output, DBI.LastInsertRowId);
            }
            return false;
        }

		public static bool FetchAll(ref List<User> output) {
			output = new List<User>();

			SQLiteDataReader reader = DBI.DoQuery($"select * from User where id != 0");
			while(reader.Read()) {
				User u = User.Factory(reader);
				output.Add(u);
			}

			return true;
		}

		public static bool FetchAllByAssignment(ref List<User> output, int shipId, bool company) {
			output = new List<User>();

			int isCompany = Convert.ToInt32(company);
			SQLiteDataReader reader = DBI.DoQuery($"SELECT u.id, u.name, u.auth0, u.rank, u.rate FROM User u, Assignment a, AssignmentRole ar WHERE a.user = u.id AND a.role = ar.id AND ar.isCompany = {isCompany} AND a.ship = {shipId};");
			while(reader.Read()) {
				User u = User.Factory(reader);
				output.Add(u);
			}

			return true;
		}

		public static bool FetchById(ref User output, int id)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from User where id = {id} limit 1;");
            if ( reader.Read() )
            {
                output = User.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool FetchByName(ref User output, string name)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from User where name = '{name}' limit 1;");
            if ( reader.Read() )
            {
                output = User.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool FetchByAuth0(ref User output, string auth0)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from User where auth0 = '{auth0}' limit 1;");
            if ( reader.Read() )
            {
                output = User.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool Store(User input)
        {
            int result = DBI.DoAction($"update User set name = '{input.name}', auth0 = '{input.auth0}', rank = {input.rank}, rate = {input.rate} where id = {input.id};");
            if (result == 1)
                return true;
            return false;
        }

        #endregion
    }
}