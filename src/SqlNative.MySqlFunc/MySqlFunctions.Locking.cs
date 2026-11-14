namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Acquires a user-level lock named <paramref name="lockName"/> and waits up to <paramref name="timeout"/> seconds.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.5 and MariaDB 5.1. Returns 1 if the lock was obtained, 0 on timeout, or NULL on error.
    /// A single connection can hold multiple named locks simultaneously. Locks are released via RELEASE_LOCK() or by connection termination.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT GET_LOCK('my_lock', 10); -&gt; 1
    /// </code>
    /// </example>
    public static string GET_LOCK(string lockName, string timeout)
    {
        return $"GET_LOCK({lockName}, {timeout})";
    }

    /// <summary>
    /// Releases the user-level lock named <paramref name="lockName"/> held by the current session.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.5 and MariaDB 5.1. Returns 1 if released, 0 if the lock was not held by this thread, NULL if the lock did not exist.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT RELEASE_LOCK('my_lock'); -&gt; 1
    /// </code>
    /// </example>
    public static string RELEASE_LOCK(string lockName)
    {
        return $"RELEASE_LOCK({lockName})";
    }

    /// <summary>
    /// Returns 1 if the named lock <paramref name="lockName"/> is free (no one holds it), 0 if it is in use, NULL on error.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.5 and MariaDB 10.0.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT IS_FREE_LOCK('my_lock'); -&gt; 1
    /// </code>
    /// </example>
    public static string IS_FREE_LOCK(string lockName)
    {
        return $"IS_FREE_LOCK({lockName})";
    }

    /// <summary>
    /// Returns the connection identifier of the session that holds the named lock <paramref name="lockName"/>, or NULL if no one holds it.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.5 and MariaDB 10.0.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT IS_USED_LOCK('my_lock'); -&gt; 12345
    /// </code>
    /// </example>
    public static string IS_USED_LOCK(string lockName)
    {
        return $"IS_USED_LOCK({lockName})";
    }
}