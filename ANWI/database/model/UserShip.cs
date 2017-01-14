namespace ANWI.DB
{
    /// <summary>
    /// Represents a row of the UserShips table.
    /// </summary>
    public struct UserShip
    {
        public int id;
        public int user_id;
        public int hull_id;
        public bool is_lti;
        public string name;
    }
}
