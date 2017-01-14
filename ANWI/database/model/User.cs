namespace ANWI.DB
{
    /// <summary>
    /// Represents a row of the Users table.
    /// </summary>

    
    
    // Data definitions
    public partial struct User
    {
        public int id;
        public string name;
        public int joined;
        public string auth0_id;
        public int rank_id;
        public int primary_rate_id;
        public int assigned_ship_id;

        private Rank rank_object;
        private Rate primary_rate_object;
        private UserShip assigned_ship_object;
    }



    // Accessors, operators, & methods
    public partial struct User
    {
        public Rank rank
        {
            get
            {
                if (rank_object == default(Rank))
                {
                    //resolve rank_object
                }
                return rank_object;
            }
            set
            {
                rank_object = value;
            }
        }

        public Rate primary_rate
        {
            get
            {
                if (primary_rate_object == default(Rate))
                {
                    //resolve primary_rate_object
                }
                return primary_rate_object;
            }
            set
            {
                primary_rate_object = value;
            }
        }

        public UserShip assigned_ship
        {
            get
            {
                if (assigned_ship_object == default(UserShip))
                {
                    //resolve assigned_ship_object
                }
                return assigned_ship_object;
            }
        }
    }
}
