namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Converts <paramref name="src"/> to the specified type <paramref name="AS_TYPE"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CAST('2024-01-15' AS DATE); -&gt; '2024-01-15'
    /// </code>
    /// </example>
    public static string CAST(string src, string AS_TYPE)
    {
        return $"CAST({src} AS {AS_TYPE})";
    }

    /// <summary>
    /// Converts <paramref name="src"/> to the specified type <paramref name="AS_TYPE"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CONVERT('2024-01-15', DATE); -&gt; '2024-01-15'
    /// </code>
    /// </example>
    public static string CONVERT(string src, string AS_TYPE)
    {
        return $"CONVERT({src}, {AS_TYPE})";
    }

    /// <summary>
    /// Casts <paramref name="src"/> to a binary string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1. Deprecated in MySQL 8.0.27 in favor of <see cref="CAST"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT BINARY 'Hello'; -&gt; 'Hello' (returned as a binary string)
    /// </code>
    /// </example>
    [Obsolete("BINARY is deprecated in MySQL 8.0.27. Use CAST(... AS BINARY) instead.")]
    public static string BINARY(string src)
    {
        return $"BINARY({src})";
    }
}