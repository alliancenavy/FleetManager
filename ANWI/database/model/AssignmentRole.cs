using System;
using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the AssignmentRole table.
    /// </summary>

    public class AssignmentRole
    {
        #region Model

        public int id;
        public string name;
        public bool isCompany;

        private AssignmentRole(int id, string name, bool isCompany)
        {
            this.id = id;
            this.name = name;
            this.isCompany = isCompany;
        }

        #endregion

        #region Class-Members

        public static AssignmentRole Factory()
        {
            AssignmentRole result = new AssignmentRole(
                id: -1,
                name: "",
                isCompany: false
            );
            return result;
        }

        public static AssignmentRole Factory(int id, string name, bool isCompany)
        {
            AssignmentRole result = new AssignmentRole(
                id: id,
                name: name,
                isCompany: isCompany
            );
            return result;
        }

        public static AssignmentRole Factory(SQLiteDataReader reader)
        {
            AssignmentRole result = new AssignmentRole(
                id: Convert.ToInt32(reader["id"]),
                name: (string)reader["name"],
                isCompany: Convert.ToBoolean(reader["isCompany"])
            );
            return result;
        }

        public static bool Create(ref AssignmentRole output, string name, bool isCompany)
        {
            int result = DBI.DoAction($"insert into AssignmentRole (name, isCompany) values('{name}', {isCompany});");
            if (result == 1)
            {
                return AssignmentRole.FetchById(ref output, DBI.LastInsertRowId);
            }
            return false;
        }

        public static bool FetchById(ref AssignmentRole output, int id)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from AssignmentRole where id = {id} limit 1;");
            if (reader.Read())
            {
                output = AssignmentRole.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool FetchByName(ref AssignmentRole output, string name)
        {
            SQLiteDataReader reader = DBI.DoQuery($"select * from AssignmentRole where name = {name} limit 1;");
            if (reader.Read())
            {
                output = AssignmentRole.Factory(reader);
                return true;
            }
            return false;
        }

        public static bool Store(AssignmentRole input)
        {
            int result = DBI.DoAction($"update AssignmentRole set name = '{input.name}', isCompany = {input.isCompany} where id = {input.id};");
            if (result == 1)
                return true;
            return false;
        }

        #endregion
    }
}