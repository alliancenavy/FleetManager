using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using ANWI.DB;

namespace ANWI
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

        public static ANWI.DB.User GetUserById(int id)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Users where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            ANWI.DB.User result = new ANWI.DB.User();
            if (!reader.Read())
            {
                result.id = -1;
            }
            else
            {
                result.id = (int)reader["id"];
                result.name = (string)reader["name"];
                result.joined = (int)reader["joined"];
                result.auth0_id = (string)reader["auth0_id"];
                result.rank_id = (int)reader["rank_id"];
                result.primary_rate_id = (int)reader["primary_rate_id"];
                result.assigned_ship_id = (int)reader["assigned_ship_id"];
            }
            return result;
        }

        public static ANWI.DB.User GetUserByName(string name)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Users where name = " + name + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            ANWI.DB.User result = new ANWI.DB.User();
            if (!reader.Read())
            {
                result.id = -1;
            }
            else
            {
                result.id = (int)reader["id"];
                result.name = (string)reader["name"];
                result.joined = (int)reader["joined"];
                result.auth0_id = (string)reader["auth0_id"];
                result.rank_id = (int)reader["rank_id"];
                result.primary_rate_id = (int)reader["primary_rate_id"];
                result.assigned_ship_id = (int)reader["assigned_ship_id"];
            }
            return result;
        }

        public static ANWI.DB.Rank GetRankById(int id)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Ranks where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            ANWI.DB.Rank result = new ANWI.DB.Rank();
            if (!reader.Read())
            {
                result.id = -1;
            }
            else
            {
                result.id = (int)reader["id"];
                result.name = (string)reader["name"];
                result.abbreviation = (string)reader["abbreviation"];
                result.ordering = (int)reader["ordering"];
                result.icon_name = (string)reader["icon_name"];
            }
            return result;
        }

        public static ANWI.DB.Rate GetRateById(int id)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Rates where id = " + id + " limit 1;", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            ANWI.DB.Rate result = new ANWI.DB.Rate();
            if (!reader.Read())
            {
                result.id = -1;
            }
            else
            {
                result.id = (int)reader["id"];
                result.name = (string)reader["name"];
                result.abbreviation = (string)reader["abbreviation"];
                result.icon_name = (string)reader["icon_name"];
            }
            return result;
        }

        public static ANWI.DB.UserShip GetUserShipById(int id)
        {
            SQLiteCommand command = new SQLiteCommand("", dbConn);
            SQLiteDataReader reader = command.ExecuteReader();
            ANWI.DB.UserShip result = new ANWI.DB.UserShip();
            if (!reader.Read())
            {
                result.id = -1;
            }
            else
            {
                result.id = (int)reader["id"];
                result.user_id = (int)reader["user_id"];
                result.hull_id = (int)reader["hull_id"];
                result.is_lti = (int)reader["is_lti"] == 1;
                result.name = (string)reader["name"];
            }
            return result;
        }

        public static ANWI.Profile GetProfileById(int id)
        {
            ANWI.DB.User user = GetUserById(id);
            ANWI.Profile result = new ANWI.Profile();
            if (user.id == -1)
            {
                result.nickname = "";
            }
            else
            {
                //NB: Due to type collisions that I haven't resolved yet, these are commented -J
                //result.rank = GetRankById(user.rank_id);
                //result.primaryRate = GetRateById(user.primary_rate_id);
                //result.assignedShip = GetUserShipById(user.assigned_ship_id);
            }
            return result;
        }
    }
}