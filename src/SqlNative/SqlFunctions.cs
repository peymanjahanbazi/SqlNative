namespace SqlNative;

public static partial class SqlFunctions
{
    /// <summary>
    /// SQL <c>NULL</c> literal.
    /// </summary>
    public static QueryPart NULL => new QueryPart("NULL");
}