using System;
using System.Data.SQLite;
using ANWI.Database.Model;
using NLog;

namespace ANWI.Database
{
    public static class DBI
    {
        static SQLiteConnection dbConn = null;

		private static NLog.Logger logger = LogManager.GetLogger("DBI");

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

		public static bool IsOpen() {
			return dbConn != null;
		}

        public static SQLiteDataReader DoQuery(string query)
        {
            Open();
			logger.Info("Running query: " + query);
            return new SQLiteCommand(query, dbConn).ExecuteReader();
        }

        public static int DoAction(string query)
        {
            Open();
			logger.Info("Running action: " + query);
            return new SQLiteCommand(query, dbConn).ExecuteNonQuery();
        }

        public static int LastInsertRowId
        {
            get
            {
                return (int)dbConn.LastInsertRowId;
            }
        }
    }
}