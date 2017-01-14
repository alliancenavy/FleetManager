namespace ANWI.DB
{
    /// <summary>
    /// Represents a row of the Hulls table.
    /// </summary>
    public struct Hull
    {
        public int id;
        public int type_id;
        public string subtype;
        public int role_id;
        public int manufacturer_id;
    }
}
