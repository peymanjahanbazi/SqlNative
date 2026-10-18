namespace SqlNative

{
    public abstract class TableRow<Pk> : QueryResult
    {
        public abstract Pk Key { get; }
    }
}