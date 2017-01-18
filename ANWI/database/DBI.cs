using System;
using System.Data.SQLite;
using ANWI.Database.Model;

namespace ANWI.Database
{
    public static class DBI
    {
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

        public static int LastInsertRowId
        {
            get
            {
                return (int)dbConn.LastInsertRowId;
            }
        }
    }
}