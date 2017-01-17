using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the Hull table.
    /// </summary>

    public class Hull
    {
        #region Model

        public int id;
        public int vendor;
        public int role;
        public int series;
        public string version;
        public string symbol;
        public int ordering;

        private HullVendor _vendor;
        private HullRole _role;
        private HullSeries _series;

        private Hull(int id, int vendor, int role, int series, string version, string symbol, int ordering,
            HullVendor Vendor, HullRole Role, HullSeries Series)
        {
            this.id = id;
            this.vendor = vendor;
            this.role = role;
            this.series = series;
            this.version = version;
            this.symbol = symbol;
            this.ordering = ordering;

            this._vendor = Vendor;
            this._role = Role;
            this._series = Series;
        }

        #endregion

        #region Instance-Members

        public HullVendor Vendor
        {
            get
            {
                if (_vendor == null)
                    HullVendor.FetchById(ref _vendor, vendor);
                return _vendor;
            }
            set
            {
                _vendor = value;
            }
        }

        public HullRole Role
        {
            get
            {
                if (_role == null)
                    HullRole.FetchById(ref _role, role);
                return _role;
            }
            set
            {
                _role = value;
            }
        }

        public HullSeries Series
        {
            get
            {
                if (_series == null)
                    HullSeries.FetchById(ref _series, series);
                return _series;
            }
            set
            {
                _series = value;
            }
        }

        #endregion

        #region Class-Members

        public static Hull Factory()
        {
            Hull result = new Hull(
                id: -1,
                vendor: -1,
                role: -1,
                series: -1,
                version: "",
                symbol: "",
                ordering: 0,

                Vendor: null,
                Role: null,
                Series: null
            );
            return result;
        }

        public static Hull Factory(int id, int vendor, int role, int series, string version, string symbol, int ordering)
        {
            Hull result = new Hull(
                id: id,
                vendor: vendor,
                role: role,
                series: series,
                version: version,
                symbol: symbol,
                ordering: ordering,

                Vendor: null,
                Role: null,
                Series: null
            );
            return result;
        }

        public static Hull Factory(SQLiteDataReader reader)
        {
            Hull result = new Hull(
                id: (int)reader["id"],
                vendor: (int)reader["vendor"],
                role: (int)reader["role"],
                series: (int)reader["series"],
                version: (string)reader["version"],
                symbol: (string)reader["symbol"],
                ordering: (int)reader["order"],

                Vendor: null,
                Role: null,
                Series: null
            );
            return result;
        }

        public static bool Create(ref Hull output, int vendor, int role, int series, string version, string symbol, int ordering)
        {
            int result = DBI.DoAction($"insert into Hull (vendor, role, series, version, symbol, ordering) values({vendor}, {role}, {series}, '{version}', '{symbol}', {ordering});");
            if (result == 1)
            {
                return Hull.FetchById(ref output, DBI.LastInsertRowId);
            }
            return false;
        }

        public static bool FetchById(ref Hull output, int id)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from Hull where id = {id} limit 1;");
            if (reader.Read())
            {
                output = Hull.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool Store(Hull input)
        {
            int result = DBI.DoAction($"update Hull set vendor = {input.vendor}, role = {input.role}, series = {input.series}, version = '{input.version}', symbol = '{input.symbol}', ordering = {input.ordering} where id = {input.id};");
            if (result == 1)
                return true;
            return false;
        }

        #endregion
    }
}