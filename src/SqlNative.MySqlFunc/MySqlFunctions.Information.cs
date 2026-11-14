namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Returns the default (current) database name as a UTF-8 string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. Returns NULL if no current database.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT DATABASE(); -&gt; 'mydb'
    /// </code>
    /// </example>
    public static string DATABASE()
    {
        return "DATABASE()";
    }

    /// <summary>
    /// Synonym for <see cref="DATABASE"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SCHEMA(); -&gt; 'mydb'
    /// </code>
    /// </example>
    public static string SCHEMA()
    {
        return "SCHEMA()";
    }

    /// <summary>
    /// Returns the current MySQL user name and host name as a UTF-8 string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. The host portion is the client host.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT USER(); -&gt; 'root@localhost'
    /// </code>
    /// </example>
    public static string USER()
    {
        return "USER()";
    }

    /// <summary>
    /// Returns the user name and host name of the current authenticated user.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1. Returns the account that the server authenticated the connection as, not necessarily the active user.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CURRENT_USER(); -&gt; 'appuser@%'
    /// </code>
    /// </example>
    public static string CURRENT_USER()
    {
        return "CURRENT_USER()";
    }

    /// <summary>
    /// Synonym for <see cref="USER"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SESSION_USER(); -&gt; 'root@localhost'
    /// </code>
    /// </example>
    public static string SESSION_USER()
    {
        return "SESSION_USER()";
    }

    /// <summary>
    /// Synonym for <see cref="SESSION_USER"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SYSTEM_USER(); -&gt; 'root@localhost'
    /// </code>
    /// </example>
    public static string SYSTEM_USER()
    {
        return "SYSTEM_USER()";
    }

    /// <summary>
    /// Returns the version of the MySQL server as a UTF-8 string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT VERSION(); -&gt; '8.0.36'
    /// </code>
    /// </example>
    public static string VERSION()
    {
        return "VERSION()";
    }

    /// <summary>
    /// Returns the connection ID for the current connection.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CONNECTION_ID(); -&gt; 1234
    /// </code>
    /// </example>
    public static string CONNECTION_ID()
    {
        return "CONNECTION_ID()";
    }

    /// <summary>
    /// Returns the character set of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CHARSET('hello'); -&gt; 'utf8mb4'
    /// </code>
    /// </example>
    public static string CHARSET(string src)
    {
        return $"CHARSET({src})";
    }

    /// <summary>
    /// Returns the collation of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT COLLATION('hello'); -&gt; 'utf8mb4_0900_ai_ci'
    /// </code>
    /// </example>
    public static string COLLATION(string src)
    {
        return $"COLLATION({src})";
    }

    /// <summary>
    /// Returns the collation coercibility of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1. Lower values are more coercible.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT COERCIBILITY('hello'); -&gt; 4
    /// </code>
    /// </example>
    public static string COERCIBILITY(string src)
    {
        return $"COERCIBILITY({src})";
    }

    /// <summary>
    /// Returns the number of rows affected by the previous INSERT, UPDATE, or DELETE statement.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.0 and MariaDB 5.1. Returns 0 for SELECT statements.
    /// </remarks>
    /// <example>
    /// <code>
    /// UPDATE t SET x = 1; SELECT ROW_COUNT(); -&gt; 5
    /// </code>
    /// </example>
    public static string ROW_COUNT()
    {
        return "ROW_COUNT()";
    }

    /// <summary>
    /// Returns the most recent AUTO_INCREMENT value generated for an AUTO_INCREMENT column.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// INSERT INTO t (name) VALUES ('first'); SELECT LAST_INSERT_ID(); -&gt; 42
    /// </code>
    /// </example>
    public static string LAST_INSERT_ID()
    {
        return "LAST_INSERT_ID()";
    }

    /// <summary>
    /// Sets the value to be returned by subsequent calls to <see cref="LAST_INSERT_ID()"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. Returns the supplied argument.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LAST_INSERT_ID(1000); -&gt; 1000
    /// </code>
    /// </example>
    public static string LAST_INSERT_ID(string expr)
    {
        return $"LAST_INSERT_ID({expr})";
    }

    /// <summary>
    /// Returns the number of rows in the result set of the previous SELECT statement.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1. Deprecated in MySQL 8.0.17 in favor of <c>COUNT(*)</c> in the same query.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SQL_CALC_FOUND_ROWS * FROM t LIMIT 10; SELECT FOUND_ROWS(); -&gt; 100
    /// </code>
    /// </example>
    public static string FOUND_ROWS()
    {
        return "FOUND_ROWS()";
    }

    /// <summary>
    /// Repeatedly executes the <paramref name="expr"/> <paramref name="count"/> times. Used for timing MySQL expressions.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1. Returns 0. The result of the inner expression is discarded.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT BENCHMARK(1000000, MD5('test')); -&gt; 0
    /// </code>
    /// </example>
    public static string BENCHMARK(string count, string expr)
    {
        return $"BENCHMARK({count}, {expr})";
    }
}