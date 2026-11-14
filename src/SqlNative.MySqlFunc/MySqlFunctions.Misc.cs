namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Returns a Universal Unique Identifier (UUID) generated according to RFC 4122.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. The result is a 36-character string of the form 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee'.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT UUID(); -&gt; '6ccd780c-baba-1026-b00d-a502-da0000c0ffee'
    /// </code>
    /// </example>
    public static string UUID()
    {
        return "UUID()";
    }

    /// <summary>
    /// Returns a "short" Universal Unique Identifier as a 64-bit unsigned integer.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.1.20 and MariaDB 5.1. Values from UUID_SHORT() are unique across servers provided the server_id is unique and the system time is not rolled back.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT UUID_SHORT(); -&gt; 2718281828458605811
    /// </code>
    /// </example>
    public static string UUID_SHORT()
    {
        return "UUID_SHORT()";
    }

    /// <summary>
    /// Converts the binary UUID <paramref name="uuidBin"/> (as produced by IS_UUID on a BINARY(16) value) into a human-readable 36-character string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.7. <paramref name="swapFlag"/> (optional) swaps the time-low and time-high parts to improve index locality; pass 1 to enable.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT BIN_TO_UUID(UUID_TO_BIN('6ccd780c-baba-1026-b00d-a502-da0000c0ffee')); -&gt; '6ccd780c-baba-1026-b00d-a502-da0000c0ffee'
    /// </code>
    /// </example>
    public static string BIN_TO_UUID(string uuidBin)
    {
        return $"BIN_TO_UUID({uuidBin})";
    }

    /// <summary>
    /// Converts the binary UUID <paramref name="uuidBin"/> into a human-readable string, swapping the time-low and time-high fields when <paramref name="swapFlag"/> is 1.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.7.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT BIN_TO_UUID(UUID_TO_BIN('6ccd780c-baba-1026-b00d-a502-da0000c0ffee'), 1); -&gt; '1026baba-6ccd-780c-b00d-a502-da0000c0ffee'
    /// </code>
    /// </example>
    public static string BIN_TO_UUID(string uuidBin, string swapFlag)
    {
        return $"BIN_TO_UUID({uuidBin}, {swapFlag})";
    }

    /// <summary>
    /// Converts the text UUID <paramref name="uuidText"/> into the binary form (BINARY(16)).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.7. <paramref name="swapFlag"/> (optional) reverses the swap done by BIN_TO_UUID(uuid, 1).
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT HEX(UUID_TO_BIN('6ccd780c-baba-1026-b00d-a502-da0000c0ffee')); -&gt; '6CCD780CBABA1026B00DA502DA0000C0FFEE'
    /// </code>
    /// </example>
    public static string UUID_TO_BIN(string uuidText)
    {
        return $"UUID_TO_BIN({uuidText})";
    }

    /// <summary>
    /// Converts the text UUID <paramref name="uuidText"/> to binary form, reversing the time-low/time-high swap when <paramref name="swapFlag"/> is 1.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.7.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT HEX(UUID_TO_BIN('6ccd780c-baba-1026-b00d-a502-da0000c0ffee', 1)); -&gt; '1026BABA6CCD780CB00DA502DA0000C0FFEE'
    /// </code>
    /// </example>
    public static string UUID_TO_BIN(string uuidText, string swapFlag)
    {
        return $"UUID_TO_BIN({uuidText}, {swapFlag})";
    }

    /// <summary>
    /// Returns 1 if <paramref name="src"/> is a valid textual UUID, 0 otherwise. Accepts BINARY(16) or hyphenated text.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.7.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT IS_UUID('6ccd780c-baba-1026-b00d-a502-da0000c0ffee'), IS_UUID('not-a-uuid'); -&gt; 1, 0
    /// </code>
    /// </example>
    public static string IS_UUID(string src)
    {
        return $"IS_UUID({src})";
    }

    /// <summary>
    /// Suppresses ONLY_FULL_GROUP_BY rejection by returning any value from the group when used inside an aggregate.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.5 and MariaDB 10.1. The result is not deterministic.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT name, ANY_VALUE(city) FROM t GROUP BY name; -&gt; returns one city per name without an ONLY_FULL_GROUP_BY error
    /// </code>
    /// </example>
    public static string ANY_VALUE(string src)
    {
        return $"ANY_VALUE({src})";
    }

    /// <summary>
    /// Returns the default value for a table column.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.13 and MariaDB 10.2.1. Only supported for explicit defaults; returns an error otherwise.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT DEFAULT(col_name) FROM t LIMIT 1;
    /// </code>
    /// </example>
    public static string DEFAULT(string column)
    {
        return $"DEFAULT({column})";
    }

    /// <summary>
    /// Sleeps (pauses) for the given number of <paramref name="seconds"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.0.12 and MariaDB 5.1. Returns 0 when the sleep finishes. Affected by server-side precision (0.001 second).
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SLEEP(2); -&gt; 0
    /// </code>
    /// </example>
    public static string SLEEP(string seconds)
    {
        return $"SLEEP({seconds})";
    }

    /// <summary>
    /// In an INSERT ... ON DUPLICATE KEY UPDATE statement, refers to the column value that would have been inserted if no duplicate-key conflict occurred.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1.0 and MariaDB 5.1. Deprecated in MySQL 8.0.20 — use the row alias + INSERT ... AS new syntax.
    /// </remarks>
    /// <example>
    /// <code>
    /// INSERT INTO t (id, hits) VALUES (1, 1) ON DUPLICATE KEY UPDATE hits = VALUES(hits) + 1;
    /// </code>
    /// </example>
    [Obsolete("VALUES() in ON DUPLICATE KEY UPDATE is deprecated in MySQL 8.0.20. Use the new row-alias INSERT ... AS syntax instead.")]
    public static string VALUES(string column)
    {
        return $"VALUES({column})";
    }

    /// <summary>
    /// Returns the name of the named constant created with NAME_CONST.
    /// </summary>
    /// <remarks>
    /// NAME_CONST is available since MySQL 5.0.12. MariaDB does not support it. Intended primarily for replicating stored procedures containing local variables.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT NAME_CONST('my_const', 1); -&gt; 1 (the constant value, with the name only visible in the column metadata)
    /// </code>
    /// </example>
    public static string NAME_CONST(string name, string value)
    {
        return $"NAME_CONST({name}, {value})";
    }
}