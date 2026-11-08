namespace SqlNative;

/// <summary>
/// Provides reusable SQL expression helpers.
/// </summary>
public class SqlUtils
{
    internal SqlUtils()
    { }

    /// <summary>
    /// SQL <c>NULL</c> literal.
    /// </summary>
    public static QueryPart NULL => new QueryPart("NULL");

    /// <summary>
    /// Builds a comma separated list from its arguments, so <c>List(a, b, c)</c> renders as <c>a, b, c</c>.
    /// This is the way to build a column list now that <c>TableColumn + TableColumn</c> joins with a space.
    /// Null and blank arguments are dropped.
    /// </summary>
    /// <param name="args">The values to join. Null or whitespace entries are ignored.</param>
    /// <returns>A comma separated list, or an empty string when no usable arguments are supplied.</returns>
    public static string CommaList(params object[] args)
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

    /// <summary>
    /// Builds a <c>SELECT</c> column list from its arguments, so <c>SelectList(a, b, c)</c> renders as
    /// <c>SELECT a, b, c</c>. Null and blank arguments are dropped.
    /// </summary>
    /// <param name="args">The columns or expressions to select. Null or whitespace entries are ignored.</param>
    /// <returns>A <c>SELECT</c> statement prefix and its column list, or an empty string when no usable arguments are supplied.</returns>
    public static string SelectList(params object[] args)
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

        return parts.Count == 0 ? "" : "SELECT " + string.Join(", ", parts);
    }

    /// <summary>
    /// Builds an <c>IN</c> predicate, so <c>ListIn("Id", [1, 2], q)</c> renders as <c>Id IN (@pl0, @pl1)</c>.
    /// Each value is added to <paramref name="q"/> as a separate parameter.
    /// </summary>
    /// <typeparam name="T">The type of the compared values.</typeparam>
    /// <param name="field">The column or expression to compare.</param>
    /// <param name="values">The values to match. When null or empty, an empty <c>IN ()</c> clause is returned.</param>
    /// <param name="q">The query that receives the generated parameters.</param>
    /// <returns>An <c>IN</c> predicate containing one parameter per value.</returns>
    public static string ListIn<T>(string field, List<T> values, Query q)
    {
        if (values is null || values.Count == 0)
            return $" {field} IN ()";

        var parts = new List<string>(values.Count);
        var parameters = new Dictionary<string, object?>();
        int idx = 0;
        foreach (var value in values)
        {
            var p = "@pl" + idx++;
            parameters.Add(p, value);
            parts.Add(p);
        }
        q.AddParameters(parameters);
        return $"{field} IN ({string.Join(", ", parts)})";
    }

    /// <summary>
    /// Builds a <c>NOT IN</c> predicate, so <c>ListNotIn("Id", [1, 2], q)</c> renders as <c>Id NOT IN (@pl0, @pl1)</c>.
    /// Each value is added to <paramref name="q"/> as a separate parameter.
    /// </summary>
    /// <typeparam name="T">The type of the compared values.</typeparam>
    /// <param name="field">The column or expression to compare.</param>
    /// <param name="values">The values to exclude. When null or empty, an empty <c>NOT IN ()</c> clause is returned.</param>
    /// <param name="q">The query that receives the generated parameters.</param>
    /// <returns>A <c>NOT IN</c> predicate containing one parameter per value.</returns>
    public static string ListNotIn<T>(string field, List<T> values, Query q)
    {
        if (values is null || values.Count == 0)
            return $" {field} NOT IN ()";

        var parts = new List<string>(values.Count);
        var parameters = new Dictionary<string, object?>();
        int idx = 0;
        foreach (var value in values)
        {
            var p = "@pl" + idx++;
            parameters.Add(p, value);
            parts.Add(p);
        }
        q.AddParameters(parameters);
        return $"{field} NOT IN ({string.Join(", ", parts)})";
    }

    /// <summary>
    /// Builds a parenthesised <c>OR</c> chain of equality checks, so <c>ListOr("Id", [1, 2], q)</c> renders as
    /// <c>(Id = @pl0 OR Id = @pl1)</c>. Each value is added to <paramref name="q"/> as a separate parameter.
    /// </summary>
    /// <typeparam name="T">The type of the compared values.</typeparam>
    /// <param name="field">The column or expression to compare.</param>
    /// <param name="values">The values to match. When null or empty, an empty string is returned.</param>
    /// <param name="q">The query that receives the generated parameters.</param>
    /// <returns>A parenthesised <c>OR</c> chain, or an empty string when <paramref name="values"/> is null or empty.</returns>
    public static string ListOr<T>(string field, List<T> values, Query q)
    {
        if (values is null || values.Count == 0)
            return $"";

        var parts = new List<string>(values.Count);
        var parameters = new Dictionary<string, object?>();
        int idx = 0;
        foreach (var value in values)
        {
            var p = "@pl" + idx++;
            parameters.Add(p, value);
            parts.Add($"{field} = {p}");
        }
        q.AddParameters(parameters);
        return $"({string.Join(" OR ", parts)})";
    }

    /// <summary>
    /// Builds a parenthesised <c>AND</c> chain of inequality checks, so <c>ListNoneOf("Id", [1, 2], q)</c> renders as
    /// <c>(Id &lt;&gt; @pl0 AND Id &lt;&gt; @pl1)</c>. Each value is added to <paramref name="q"/> as a separate parameter.
    /// </summary>
    /// <typeparam name="T">The type of the compared values.</typeparam>
    /// <param name="field">The column or expression to compare.</param>
    /// <param name="values">The values to exclude. When null or empty, an empty string is returned.</param>
    /// <param name="q">The query that receives the generated parameters.</param>
    /// <returns>A parenthesised <c>AND</c> chain, or an empty string when <paramref name="values"/> is null or empty.</returns>
    public static string ListNoneOf<T>(string field, List<T> values, Query q)
    {
        if (values is null || values.Count == 0)
            return $"";

        var parts = new List<string>(values.Count);
        var parameters = new Dictionary<string, object?>();
        int idx = 0;
        foreach (var value in values)
        {
            var p = "@pl" + idx++;
            parameters.Add(p, value);
            parts.Add($"{field} <> {p}");
        }
        q.AddParameters(parameters);
        return $"({string.Join(" AND ", parts)})";
    }
}

public sealed class Su : SqlUtils
{
    internal Su() : base()
    { }
}