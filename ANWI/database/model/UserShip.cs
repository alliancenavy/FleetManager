namespace ANWI.DB
{
    /// <summary>
    /// Represents a row of the UserShips table.
    /// </summary>

    
    
    // Data definitions
    public partial struct UserShip
    {
        public int id;
        public int user_id;
        public int hull_id;
        public bool is_lti;
        public string name;
    }



    // Accessors, operators, & methods
    public partial struct UserShip
    {
        public static bool operator ==(UserShip left, UserShip right)
        {
            return left.GetHashCode() == right.GetHashCode();
        }

        public static bool operator !=(UserShip left, UserShip right)
        {
            return left.GetHashCode() != right.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + id.GetHashCode();
                hash = hash * 23 + user_id.GetHashCode();
                hash = hash * 23 + hull_id.GetHashCode();
                hash = hash * 23 + is_lti.GetHashCode();
                hash = hash * 23 + name.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }
    }
}
