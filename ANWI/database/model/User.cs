namespace ANWI.DB
{
    /// <summary>
    /// Represents a row of the Users table.
    /// </summary>
    public struct User
    {
        public int id;
        public string name;
        public int joined;
        public string auth0_id;
        public int rank_id;
        public int primary_rate_id;
        public int assigned_ship_id;
    }
}
