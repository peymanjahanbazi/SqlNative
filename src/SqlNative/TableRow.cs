namespace SqlNative

{
    /// <summary>
    /// Base class for rows persisted in a database table.
    /// </summary>
    /// <typeparam name="Pk">The primary key type of the row.</typeparam>
    public abstract class TableRow<Pk> : QueryResult
    {
        /// <summary>Gets the primary key of this row.</summary>
        public abstract Pk Key { get; }
    }
}
