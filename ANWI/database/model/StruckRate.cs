namespace ANWI.DB
{
    /// <summary>
    /// Represents a row of the StruckRates table.
    /// </summary>



    // Data definitions
    public partial struct StruckRate
    {
        public int id;
        public int user_id;
        public int rate_id;
        public int rank;

        public User user_object;
        public Rate rate_object;
    }



    // Accessors, operators, & methods
    public partial struct StruckRate
    {
        public User user
        {
            get
            {
                if (user_object == default(User))
                {
                    // if user_object is not resolved, resolve it first
                }
                return user_object;
            }
            set
            {
                user_object = value;
            }
        }

        public static bool operator ==(StruckRate sr1, StruckRate sr2)
        {
            return sr1.GetHashCode() == sr2.GetHashCode();
        }

        public static bool operator !=(StruckRate sr1, StruckRate sr2)
        {
            return sr1.GetHashCode() != sr2.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + id.GetHashCode();
                hash = hash * 23 + user_id.GetHashCode();
                hash = hash * 23 + rate_id.GetHashCode();
                hash = hash * 23 + rank.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }
    }
}
