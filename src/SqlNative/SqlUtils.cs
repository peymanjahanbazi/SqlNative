using System.Globalization;

namespace SqlNative;

/// <summary>
/// Provides reusable SQL expression helpers: clause fragments, comma separated lists and
/// predicates over a list of values.
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
    /// Builds a comma separated list from its arguments, so <c>Join(a, b, c)</c> renders as <c>a, b, c</c>.
    /// This is the way to build a column list now that <c>TableColumn + TableColumn</c> joins with a space.
    /// Null and blank arguments are dropped.
    /// </summary>
    /// <param name="args">The values to join. Null or whitespace entries are ignored.</param>
    /// <returns>A comma separated list, or an empty string when no usable arguments are supplied.</returns>
    public static string Join(params object[] args)
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

    public static string As(string column, string alias)
    {
        return $"{column} AS {alias}";
    }

    /// <summary>
    /// Builds a <c>SELECT</c> column list from its arguments, so <c>Select(a, b, c)</c> renders as
    /// <c>SELECT a, b, c</c>. Null and blank arguments are dropped.
    /// </summary>
    /// <param name="args">The columns or expressions to select. Null or whitespace entries are ignored.</param>
    /// <returns>A <c>SELECT</c> statement prefix and its column list, or an empty string when no usable arguments are supplied.</returns>
    public static string Select(params object[] args)
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
    /// Builds an <c>IN</c> predicate, so <c>In("Id", [1, 2], q)</c> renders as <c>Id IN (1, 2)</c>.
    /// Numbers and GUIDs are rendered inline using the invariant culture, with null entries rendered as
    /// <c>NULL</c>. All other types (including strings, which may carry SQL injection payloads) are added
    /// to <paramref name="q"/> as parameters.
    /// </summary>
    /// <typeparam name="T">The type of the compared values. Must be a basic type; complex objects are not supported.</typeparam>
    /// <param name="field">The column or expression to compare.</param>
    /// <param name="values">The values to match. When null or empty, an empty <c>IN ()</c> clause is returned.</param>
    /// <param name="q">The query that receives the generated parameters when the values are not inlined.</param>
    /// <returns>An <c>IN</c> predicate, inlined for numbers and GUIDs and parameterised otherwise.</returns>
    /// <remarks>
    /// An empty list renders as <c>field IN ()</c>, which is a syntax error in SQL Server, MySQL and
    /// PostgreSQL. Check the list before calling, or omit the clause altogether.
    /// </remarks>
    public static string In<T>(string field, List<T> values, Query q)
    {
        if (values is null || values.Count == 0)
            return $" {field} IN ()";

        if (IsInlineableType(typeof(T)))
        {
            var literals = new List<string>(values.Count);
            foreach (var value in values)
            {
                literals.Add(value is null ? "NULL" : ToSqlLiteral(value));
            }

            return $"{field} IN ({string.Join(", ", literals)})";
        }

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

    private static bool IsInlineableType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        return type == typeof(Guid)
            || type == typeof(sbyte)
            || type == typeof(byte)
            || type == typeof(short)
            || type == typeof(ushort)
            || type == typeof(int)
            || type == typeof(uint)
            || type == typeof(long)
            || type == typeof(ulong)
            || type == typeof(float)
            || type == typeof(double)
            || type == typeof(decimal)
            || type == typeof(bool);
    }

    private static string ToSqlLiteral(object value)
    {
        if (value is Guid guid)
            return $"'{guid:D}'";

        if (value is IFormattable formattable)
            return formattable.ToString(null, CultureInfo.InvariantCulture) ?? "NULL";

        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "NULL";
    }

    /// <summary>
    /// Builds a <c>NOT IN</c> predicate, so <c>NotIn("Id", [1, 2], q)</c> renders as <c>Id NOT IN (1, 2)</c>.
    /// Numbers and GUIDs are rendered inline using the invariant culture, with null entries rendered as
    /// <c>NULL</c>. All other types (including strings, which may carry SQL injection payloads) are added
    /// to <paramref name="q"/> as parameters.
    /// </summary>
    /// <typeparam name="T">The type of the compared values. Must be a basic type; complex objects are not supported.</typeparam>
    /// <param name="field">The column or expression to compare.</param>
    /// <param name="values">The values to exclude. When null or empty, an empty <c>NOT IN ()</c> clause is returned.</param>
    /// <param name="q">The query that receives the generated parameters when the values are not inlined.</param>
    /// <returns>A <c>NOT IN</c> predicate, inlined for numbers and GUIDs and parameterised otherwise.</returns>
    /// <remarks>
    /// An empty list renders as <c>field NOT IN ()</c>, which is a syntax error in SQL Server, MySQL and
    /// PostgreSQL. Check the list before calling, or omit the clause altogether.
    /// </remarks>
    public static string NotIn<T>(string field, List<T> values, Query q)
    {
        if (values is null || values.Count == 0)
            return $" {field} NOT IN ()";

        if (IsInlineableType(typeof(T)))
        {
            var literals = new List<string>(values.Count);
            foreach (var value in values)
            {
                literals.Add(value is null ? "NULL" : ToSqlLiteral(value));
            }

            return $"{field} NOT IN ({string.Join(", ", literals)})";
        }

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
    /// Builds a parenthesised <c>OR</c> chain of equality checks, so <c>AnyOf("Id", [1, 2], q)</c> renders as
    /// <c>(Id = 1 OR Id = 2)</c>. Numbers and GUIDs are rendered inline using the invariant culture, with null
    /// entries rendered as <c>Id IS NULL</c>. All other types (including strings, which may carry SQL injection
    /// payloads) are added to <paramref name="q"/> as parameters, one parameter per value.
    /// </summary>
    /// <typeparam name="T">The type of the compared values. Must be a basic type; complex objects are not supported.</typeparam>
    /// <param name="field">The column or expression to compare.</param>
    /// <param name="values">The values to match. When null or empty, an empty string is returned.</param>
    /// <param name="q">The query that receives the generated parameters when the values are not inlined.</param>
    /// <returns>A parenthesised <c>OR</c> chain, or an empty string when <paramref name="values"/> is null or empty.</returns>
    /// <remarks>
    /// In the parameterised form a null value becomes <c>field = @pl0</c>, which never matches a null row.
    /// </remarks>
    public static string AnyOf<T>(string field, List<T> values, Query q)
    {
        if (values is null || values.Count == 0)
            return $"";

        if (IsInlineableType(typeof(T)))
        {
            var literals = new List<string>(values.Count);
            foreach (var value in values)
            {
                literals.Add(value is null ? $"{field} IS NULL" : $"{field} = {ToSqlLiteral(value)}");
            }

            return $"({string.Join(" OR ", literals)})";
        }

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
    /// Builds a parenthesised <c>AND</c> chain of inequality checks, so <c>NoneOf("Id", [1, 2], q)</c> renders as
    /// <c>(Id &lt;&gt; 1 AND Id &lt;&gt; 2)</c>. Numbers and GUIDs are rendered inline using the invariant culture,
    /// with null entries rendered as <c>Id IS NOT NULL</c>. All other types (including strings, which may carry
    /// SQL injection payloads) are added to <paramref name="q"/> as parameters, one parameter per value.
    /// </summary>
    /// <typeparam name="T">The type of the compared values. Must be a basic type; complex objects are not supported.</typeparam>
    /// <param name="field">The column or expression to compare.</param>
    /// <param name="values">The values to exclude. When null or empty, an empty string is returned.</param>
    /// <param name="q">The query that receives the generated parameters when the values are not inlined.</param>
    /// <returns>A parenthesised <c>AND</c> chain, or an empty string when <paramref name="values"/> is null or empty.</returns>
    /// <remarks>
    /// In the parameterised form a null value becomes <c>field &lt;&gt; @pl0</c>, which never matches a null row.
    /// </remarks>
    public static string NoneOf<T>(string field, List<T> values, Query q)
    {
        if (values is null || values.Count == 0)
            return $"";

        if (IsInlineableType(typeof(T)))
        {
            var literals = new List<string>(values.Count);
            foreach (var value in values)
            {
                literals.Add(value is null ? $"{field} IS NOT NULL" : $"{field} <> {ToSqlLiteral(value)}");
            }

            return $"({string.Join(" AND ", literals)})";
        }

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

/// <summary>
/// A short alias for <see cref="SqlUtils"/>. The helpers are inherited, so <c>Su.Join(...)</c> and
/// <c>Su.In(...)</c> behave exactly like their <see cref="SqlUtils"/> counterparts.
/// </summary>
public sealed class Su : SqlUtils
{
    internal Su() : base()
    { }
}