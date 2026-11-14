namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Formats a number of bytes <paramref name="bytes"/> into a human-readable string with a unit suffix (KiB, MiB, GiB, ...).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.0. Uses powers of 1024 (IEC binary prefixes).
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT FORMAT_BYTES(1048576); -&gt; '1.00 MiB'
    /// </code>
    /// </example>
    public static string FORMAT_BYTES(string bytes)
    {
        return $"FORMAT_BYTES({bytes})";
    }

    /// <summary>
    /// Formats a performance-schema picosecond value <paramref name="picoseconds"/> into a human-readable time string with unit suffix.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.0. Up to "seconds" with adaptive fractional precision.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT FORMAT_PICO_TIME(1500000000000); -&gt; '1.50 ns'
    /// </code>
    /// </example>
    public static string FORMAT_PICO_TIME(string picoseconds)
    {
        return $"FORMAT_PICO_TIME({picoseconds})";
    }

    /// <summary>
    /// Returns the internal thread ID of the current foreground thread, as stored in the performance_schema.threads table.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.16 and MariaDB 10.7. Returns NULL for background threads or if the instrument is missing.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT PS_CURRENT_THREAD_ID(); -&gt; 1234
    /// </code>
    /// </example>
    public static string PS_CURRENT_THREAD_ID()
    {
        return "PS_CURRENT_THREAD_ID()";
    }

    /// <summary>
    /// Returns the internal performance-schema thread ID for the connection ID <paramref name="connectionId"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.16 and MariaDB 10.7. Returns NULL when no thread exists for the given connection ID.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT PS_THREAD_ID(CONNECTION_ID()); -&gt; 1234
    /// </code>
    /// </example>
    public static string PS_THREAD_ID(string connectionId)
    {
        return $"PS_THREAD_ID({connectionId})";
    }
}