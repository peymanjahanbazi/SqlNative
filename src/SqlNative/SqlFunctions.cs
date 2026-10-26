namespace SqlNative;

/// <summary>
/// Provides reusable SQL expression helpers.
/// </summary>
public static partial class SqlFunctions
{
    /// <summary>
    /// SQL <c>NULL</c> literal.
    /// </summary>
    public static QueryPart NULL => new QueryPart("NULL");

    /// <summary>
    /// Builds a comma separated list from its arguments, so <c>List(a, b, c)</c> renders as <c>a, b, c</c>.
    /// This is the way to build a column list now that <c>TableColumn + TableColumn</c> joins with a space.
    /// Null and blank arguments are dropped.
    /// </summary>
    public static string CList(params object[] args)
    {
        if (args is null || args.Length == 0)
            return "";

        var parts = new List<string>(args.Length);
        foreach (var arg in args)
        {
            if (arg is null)
                continue;

            var part = arg.ToString();
            if (!string.IsNullOrWhiteSpace(part))
                parts.Add(part);
        }

        return parts.Count == 0 ? "" : string.Join(", ", parts);
    }
}