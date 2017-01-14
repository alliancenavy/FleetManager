namespace ANWI.DB
{
    /// <summary>
    /// Represents a row of the Rates table.
    /// </summary>



    // Data definitions
    public partial struct Rate
    {
        public int id;
        public string name;
        public string abbreviation;
        public string icon_name;
    }



    // Accessors, operators, & methods
    public partial struct Rate
    {
        public static bool operator ==(Rate r1, Rate r2)
        {
            return r1.GetHashCode() == r2.GetHashCode();
        }

        public static bool operator !=(Rate r1, Rate r2)
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
