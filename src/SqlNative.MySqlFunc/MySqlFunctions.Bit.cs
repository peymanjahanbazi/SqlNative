namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Returns the number of 1-bits in the binary representation of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. As of MySQL 8.0, this is BIT_COUNT() — returns BIGINT.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT BIT_COUNT(b'1010'); -&gt; 2
    /// </code>
    /// </example>
    public static string BIT_COUNT(string src)
    {
        return $"BIT_COUNT({src})";
    }
}