namespace SqlNative
{
    /// <summary>
    /// Describes an INSERT statement.
    /// </summary>
    public class InsertModel
    {
        /// <summary>Gets or sets the comma-separated list of inserted columns.</summary>
        public string Columns { get; set; }

        /// <summary>Gets or sets the comma-separated list of inserted values.</summary>
        public string Values { get; set; }

        /// <summary>Gets or sets the parameter values used by the statement.</summary>
        public object Parameters { get; set; }
    }
}
