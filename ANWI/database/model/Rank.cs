namespace ANWI.DB
{
    /// <summary>
    /// Represents a row of the Ranks table.
    /// </summary>

    
    
    // Data definitions
    public partial struct Rank
    {
        public int id;
        public string name;
        public string abbreviation;
        public int ordering;
        public string icon_name;
    }



    // Accessors, operators, & methods
    public partial struct Rank
    {
        public static bool operator ==(Rank r1, Rank r2)
        {
            return r1.GetHashCode() == r2.GetHashCode();
        }

        public static bool operator !=(Rank r1, Rank r2)
        {
            return r1.GetHashCode() != r2.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + id.GetHashCode();
                hash = hash * 23 + name.GetHashCode();
                hash = hash * 23 + abbreviation.GetHashCode();
                hash = hash * 23 + ordering.GetHashCode();
                hash = hash * 23 + icon_name.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }
    }
}
