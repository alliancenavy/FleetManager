using System.Data.SQLite;

namespace ANWI.Database.Model
{
    /// <summary>
    /// Represents a row of the Hulls table.
    /// </summary>
    
    public sealed class Hull
    {
        public static Hull Factory()
        {
            Hull result = new Hull(-1, -1, "", -1, -1, null, null, null);
            return result;
        }

        public static Hull Factory(int _id, int _type_id, string _subtype, int _role_id, int _manufacturer_id)
        {
            Hull result = new Hull(_id, _type_id, _subtype, _role_id, _manufacturer_id, null, null, null);
            return result;
        }

        public static Hull Factory(SQLiteDataReader reader)
        {
            Hull result = new Hull(
                (int)reader["id"],
                (int)reader["type_id"],
                (string)reader["subtype"],
                (int)reader["role_id"],
                (int)reader["manufacturer_id"],
                null,
                null,
                null
            );
            return result;
        }

        public int id;
        public int type_id;
        public string subtype;
        public int role_id;
        public int manufacturer_id;

        HullType type_object;
        HullRole role_object;
        HullManufacturer manufacturer_object;

        private Hull(int _id, int _type_id, string _subtype, int _role_id, int _manufacturer_id, HullType _type_object, HullRole _role_object, HullManufacturer _manufacturer_object)
        {
            id = _id;
            type_id = _type_id;
            subtype = _subtype;
            role_id = _role_id;
            manufacturer_id = _manufacturer_id;
            type_object = _type_object;
            role_object = _role_object;
            manufacturer_object = _manufacturer_object;
        }

        public HullType type
        {
            get
            {
                if (type_object == null)
                    DBI.GetHullTypeById(type_id, out type_object);
                return type_object;
            }
            set
            {
                type_object = value;
            }
        }

        public HullRole role
        {
            get
            {
                if (role_object == null)
                    DBI.GetHullRoleById(role_id, out role_object);
                return role_object;
            }
            set
            {
                role_object = value;
            }
        }

        public HullManufacturer manufacturer
        {
            get
            {
                if (manufacturer_object == null)
                    DBI.GetHullManufacturerById(manufacturer_id, out manufacturer_object);
                return manufacturer_object;
            }
            set
            {
                manufacturer_object = value;
            }
        }
    }
}
