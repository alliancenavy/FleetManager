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

		/// <summary>
		/// Opens a database connection
		/// </summary>
		/// <param name="dbFileName"></param>
		/// <returns></returns>
		public static bool Open(string dbFileName = "fleetManager.sqlite3db")
        {
            if (dbConn != null)
                return false;
			if (dbFileName == null)
				dbFileName = "fleetManager.sqlite3db";
            dbConn = 
				new SQLiteConnection($"Data Source={dbFileName};Version=3;");
            dbConn.Open();
            return true;
        }

		/// <summary>
		/// Closes a database connection
		/// </summary>
        public static void Close()
        {
            if (dbConn == null)
                return;
            dbConn.Close();
        }

		/// <summary>
		/// Checks if the connection is open
		/// </summary>
		/// <returns></returns>
		public static bool IsOpen() {
			return dbConn != null;
		}

		/// <summary>
		/// Runs a query which returns data
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
        public static SQLiteDataReader DoQuery(string query)
        {
			try {
				Open();
				return new SQLiteCommand(query, dbConn).ExecuteReader();
			} catch (SQLiteException e) {
				logger.Error("Failed to run query\n\n" + query +
					"\n\nException: " + e);
				return null;
			}
        }

		/// <summary>
		/// Runs a prepared query which returns data
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static SQLiteDataReader 
		DoPreparedQuery(string query, params Tuple<string, object>[] parameters) {
			SQLiteCommand statement = new SQLiteCommand(query, dbConn);

			foreach(Tuple<string, object> param in parameters) {
				statement.Parameters.Add(
					new SQLiteParameter(param.Item1, param.Item2)
					);
			}

			try {
				Open();
				return statement.ExecuteReader();
			} catch(SQLiteException e) {
				logger.Error("Failed to run prepared query\n\n" + 
					statement.CommandText +
					"\n\nException: " + e);
				return null;
			}
		}

		/// <summary>
		/// Runs a query which does not return data
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
        public static int DoAction(string query)
        {
			try {
				Open();
				return new SQLiteCommand(query, dbConn).ExecuteNonQuery();
			} catch (SQLiteException e) {
				logger.Error("Failed to run query\n\n" + query +
					"\n\nException: " + e);
				return -1;
			}
        }

		/// <summary>
		/// Runs a prepared query which does not return data
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static int 
		DoPreparedAction(string query, params Tuple<string, object>[] parameters) {
			SQLiteCommand statement = new SQLiteCommand(query, dbConn);

			foreach(Tuple<string, object> param in parameters) {
				statement.Parameters.Add(
					new SQLiteParameter(param.Item1, param.Item2)
					);
			}

			try {
				Open();
				return statement.ExecuteNonQuery();
			} catch(SQLiteException e) {
				logger.Error("Failed to run prepared query\n\n" + 
					statement.CommandText +
					"\n\nException: " + e);
				return -1;
			}
		}

		/// <summary>
		/// Returns the last inserted row id
		/// </summary>
        public static int LastInsertRowId
        {
            get
            {
                return (int)dbConn.LastInsertRowId;
            }
        }
    }
}