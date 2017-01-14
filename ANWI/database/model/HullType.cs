namespace ANWI.DB
{
    /// <summary>
    /// Represents a row of the HullTypes table.
    /// </summary>



    // Data definitions
    public partial struct HullType
    {
        public int id;
        public string name;
    }



    // Accessors, operators, & methods
    public partial struct HullType
    {
        public static bool operator ==(HullType h1, HullType h2)
        {
            return h1.GetHashCode() == h2.GetHashCode();
        }

        public static bool operator !=(HullType h1, HullType h2)
        {
            return h1.GetHashCode() != h2.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + id.GetHashCode();
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
