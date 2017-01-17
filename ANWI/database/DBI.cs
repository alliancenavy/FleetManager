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

        public static SQLiteDataReader DoQuery(string query)
        {
            Open();
            return new SQLiteCommand(query, dbConn).ExecuteReader();
        }

        public static int DoAction(string query)
        {
            Open();
            return new SQLiteCommand(query, dbConn).ExecuteNonQuery();
        }

        #endregion

        #region Assignment

        public static bool GetAssignmentById(int id, out Assignment result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Assignment where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = Assignment.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region AssignmentRole

        public static bool GetAssignmentRoleById(int id, out AssignmentRole result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from AssignmentRole where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = AssignmentRole.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region Hull

        public static bool GetHullById(int id, out Hull result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Hull where id = " + id + " limit 1;", dbConn);
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

        #region HullRole

        public static bool GetHullRoleById(int id, out HullRole result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from HullRole where id = " + id + " limit 1;", dbConn);
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
            SQLiteCommand command = new SQLiteCommand("select * from HullRole where name = " + name + " limit 1;", dbConn);
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

        #region HullSeries

        public static bool GetHullSeriesById(int id, out HullSeries result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from HullSeries where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = HullSeries.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        public static bool GetHullSeriesByName(string name, out HullSeries result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from HullSeries where name = " + name + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = HullSeries.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region HullVendor

        public static bool GetHullVendorById(int id, out HullVendor result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from HullVendor where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = HullVendor.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        public static bool GetHullVendorByName(string name, out HullVendor result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from HullVendor where name = " + name + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = HullVendor.Factory(reader);
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region Rank

        public static bool GetRankById(int id, out Rank result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Rank where id = " + id + " limit 1;", dbConn);
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
            SQLiteCommand command = new SQLiteCommand("select * from Rank where name = " + name + " limit 1;", dbConn);
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

        #region Rate

        public static bool GetRateById(int id, out Rate result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Rate where id = " + id + " limit 1;", dbConn);
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
            SQLiteCommand command = new SQLiteCommand("select * from Rate where name = " + name + " limit 1;", dbConn);
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

        #region StruckRate

        public static bool GetStruckRateById(int id, out StruckRate result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from StruckRate where id = " + id + " limit 1;", dbConn);
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

        #region User

        public static bool GetUserById(int id, out User result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from User where id = " + id + " limit 1;", dbConn);
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
            SQLiteCommand command = new SQLiteCommand("select * from User where name = " + name + " limit 1;", dbConn);
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

        #region UserShip

        public static bool GetUserShipById(int id, out UserShip result)
        {
            SQLiteCommand command = new SQLiteCommand("select * from UserShip where id = " + id + " limit 1;", dbConn);
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
            SQLiteCommand command = new SQLiteCommand("select * from UserShip where name = " + name + " limit 1;", dbConn);
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

    }
}