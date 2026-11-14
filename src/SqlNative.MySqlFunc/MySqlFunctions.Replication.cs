namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Blocks until the slave has read and applied all updates up to the specified position in the master's binary log, or until <paramref name="timeout"/> seconds elapse.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.6 and MariaDB 10.0. Deprecated as of MySQL 8.0.26 in favor of SOURCE_POS_WAIT(). Returns the number of log events the slave had to wait for, 0 on timeout, or NULL on error.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT MASTER_POS_WAIT('master-bin.000001', 4, 5); -&gt; 0
    /// </code>
    /// </example>
    [Obsolete("MASTER_POS_WAIT is deprecated in MySQL 8.0.26. Use SOURCE_POS_WAIT() instead.")]
    public static string MASTER_POS_WAIT(string logName, string pos, string timeout)
    {
        return $"MASTER_POS_WAIT({logName}, {pos}, {timeout})";
    }

    /// <summary>
    /// Blocks until the replica has read and applied all updates up to the specified position in the source's binary log, or until <paramref name="timeout"/> seconds elapse.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.26 and MariaDB 10.0. Returns the number of log events the replica had to wait for, 0 on timeout, or NULL on error.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SOURCE_POS_WAIT('source-bin.000001', 4, 5); -&gt; 0
    /// </code>
    /// </example>
    public static string SOURCE_POS_WAIT(string logName, string pos, string timeout)
    {
        return $"SOURCE_POS_WAIT({logName}, {pos}, {timeout})";
    }
}