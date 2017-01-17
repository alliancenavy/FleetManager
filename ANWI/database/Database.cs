using System;
using System.Data.SQLite;
using ANWI.Database.Model;

namespace ANWI.Database
{
    public static class DBI
    {

        #region Sqlite
        static SQLiteConnection dbConn = null;

        public static bool Open(string dbFileName = "fleetManager.sqlite3db")
        {
            if (dbConn != null)
                return false;
            dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
            dbConn.Open();
            return true;
        }

        public static void Close()
        {
            if (dbConn == null)
                return;
            dbConn.Close();
        }

        #endregion

        #region Users

        public static bool GetUserById(int id, out User result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Users where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = User.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        public static bool GetUserByName(string name, out User result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Users where name = " + name + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = User.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region Ranks

        public static bool GetRankById(int id, out Rank result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Ranks where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = Rank.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        public static bool GetRankByName(string name, out Rank result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Ranks where name = " + name + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = Rank.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region Rates

        public static bool GetRateById(int id, out Rate result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Rates where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = Rate.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        public static bool GetRateByName(string name, out Rate result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Rates where name = " + name + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = Rate.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region Hulls

        public static bool GetHullById(int id, out Hull result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Hulls where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = Hull.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region HullManufacturers

        public static bool GetHullManufacturerById(int id, out HullManufacturer result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from HullManufacturers where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = HullManufacturer.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        public static bool GetHullManufacturerByName(string name, out HullManufacturer result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from HullManufacturers where name = " + name + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = HullManufacturer.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region HullRoles

        public static bool GetHullRoleById(int id, out HullRole result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from HullRoles where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = HullRole.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        public static bool GetHullRoleByName(string name, out HullRole result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from HullRoles where name = " + name + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = HullRole.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region HullTypes

        public static bool GetHullTypeById(int id, out HullType result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from HullTypes where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = HullType.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        public static bool GetHullTypeByName(string name, out HullType result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from HullTypes where name = " + name + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = HullType.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region UserShips

        public static bool GetUserShipById(int id, out UserShip result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from UserShips where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = UserShip.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        public static bool GetUserShipByName(string name, out UserShip result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from UserShips where name = " + name + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = UserShip.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region StruckRates

        public static bool GetStruckRateById(int id, out StruckRate result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from StruckRates where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = StruckRate.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        #endregion

    }
}