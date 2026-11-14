namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Returns 1 (true) if every GTID in the GTID set <paramref name="subset"/> is also in <paramref name="set"/>; otherwise returns 0.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.6.5 and MariaDB 10.0.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT GTID_SUBSET('3E11FA47-71CA-11E1-9E33-C80AA9429562:1-5', '3E11FA47-71CA-11E1-9E33-C80AA9429562:1-10'); -&gt; 1
    /// </code>
    /// </example>
    public static string GTID_SUBSET(string subset, string set)
    {
        return $"GTID_SUBSET({subset}, {set})";
    }

    /// <summary>
    /// Returns a GTID set containing all GTIDs in <paramref name="set"/> that are not in <paramref name="subset"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.6.5 and MariaDB 10.0.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT GTID_SUBTRACT('3E11FA47-71CA-11E1-9E33-C80AA9429562:1-10', '3E11FA47-71CA-11E1-9E33-C80AA9429562:1-5'); -&gt; '3E11FA47-71CA-11E1-9E33-C80AA9429562:6-10'
    /// </code>
    /// </example>
    public static string GTID_SUBTRACT(string set, string subset)
    {
        return $"GTID_SUBTRACT({set}, {subset})";
    }

    /// <summary>
    /// Waits until the slave has executed all transactions whose GTID is contained in the given <paramref name="gtidSet"/>, or until <paramref name="timeout"/> seconds elapse.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.6.5 and MariaDB 10.0. Returns the number of transactions waited for, 0 on timeout, or NULL on error.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT WAIT_FOR_EXECUTED_GTID_SET('3E11FA47-71CA-11E1-9E33-C80AA9429562:1-5', 10); -&gt; 5
    /// </code>
    /// </example>
    public static string WAIT_FOR_EXECUTED_GTID_SET(string gtidSet, string timeout)
    {
        return $"WAIT_FOR_EXECUTED_GTID_SET({gtidSet}, {timeout})";
    }

    /// <summary>
    /// Waits until the slave's applier thread has executed all transactions whose GTID is contained in <paramref name="gtidSet"/>, or until <paramref name="timeout"/> seconds elapse.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.5 and MariaDB 10.0.13. Useful when running with multiple replication channels.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT WAIT_UNTIL_SQL_THREAD_AFTER_GTIDS('3E11FA47-71CA-11E1-9E33-C80AA9429562:1-5', 10); -&gt; 5
    /// </code>
    /// </example>
    public static string WAIT_UNTIL_SQL_THREAD_AFTER_GTIDS(string gtidSet, string timeout)
    {
        return $"WAIT_UNTIL_SQL_THREAD_AFTER_GTIDS({gtidSet}, {timeout})";
    }
}