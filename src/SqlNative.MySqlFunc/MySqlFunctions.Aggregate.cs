namespace SqlNative;

/// <summary>
/// Provides the MySQL and MariaDB functions that can be used in a query.
/// </summary>
public partial class MySqlFunctions
{
    /// <summary>
    /// Returns a count of the number of non-NULL values of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT COUNT(price) FROM orders; -&gt; 100
    /// </code>
    /// </example>
    public static string COUNT(string src)
    {
        return $"COUNT({src})";
    }

    /// <summary>
    /// Returns the sum of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SUM(amount) FROM orders; -&gt; 4500.00
    /// </code>
    /// </example>
    public static string SUM(string src)
    {
        return $"SUM({src})";
    }

    /// <summary>
    /// Returns the average value of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT AVG(price) FROM products; -&gt; 24.75
    /// </code>
    /// </example>
    public static string AVG(string src)
    {
        return $"AVG({src})";
    }

    /// <summary>
    /// Returns the minimum value of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT MIN(price) FROM products; -&gt; 4.99
    /// </code>
    /// </example>
    public static string MIN(string src)
    {
        return $"MIN({src})";
    }

    /// <summary>
    /// Returns the maximum value of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT MAX(price) FROM products; -&gt; 99.50
    /// </code>
    /// </example>
    public static string MAX(string src)
    {
        return $"MAX({src})";
    }

    /// <summary>
    /// Returns the population standard deviation of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1. Deprecated in MySQL 8.0.17 in favor of <see cref="STDDEV_POP"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT STD(score) FROM tests; -&gt; 12.3456
    /// </code>
    /// </example>
    public static string STD(string src)
    {
        return $"STD({src})";
    }

    /// <summary>
    /// Returns the population standard deviation of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1. Deprecated in MySQL 8.0.17 in favor of <see cref="STDDEV_POP"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT STDDEV(score) FROM tests; -&gt; 12.3456
    /// </code>
    /// </example>
    public static string STDDEV(string src)
    {
        return $"STDDEV({src})";
    }

    /// <summary>
    /// Returns the population standard deviation of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT STDDEV_POP(score) FROM tests; -&gt; 12.3456
    /// </code>
    /// </example>
    public static string STDDEV_POP(string src)
    {
        return $"STDDEV_POP({src})";
    }

    /// <summary>
    /// Returns the sample standard deviation of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT STDDEV_SAMP(score) FROM tests; -&gt; 12.5023
    /// </code>
    /// </example>
    public static string STDDEV_SAMP(string src)
    {
        return $"STDDEV_SAMP({src})";
    }

    /// <summary>
    /// Returns the population standard variance of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT VAR_POP(score) FROM tests; -&gt; 152.4138
    /// </code>
    /// </example>
    public static string VAR_POP(string src)
    {
        return $"VAR_POP({src})";
    }

    /// <summary>
    /// Returns the sample variance of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT VAR_SAMP(score) FROM tests; -&gt; 156.3075
    /// </code>
    /// </example>
    public static string VAR_SAMP(string src)
    {
        return $"VAR_SAMP({src})";
    }

    /// <summary>
    /// Returns the population standard variance of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT VARIANCE(score) FROM tests; -&gt; 152.4138
    /// </code>
    /// </example>
    public static string VARIANCE(string src)
    {
        return $"VARIANCE({src})";
    }

    /// <summary>
    /// Returns the bitwise AND of all bits in <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0.17 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT BIT_AND(flags) FROM permissions; -&gt; 4
    /// </code>
    /// </example>
    public static string BIT_AND(string src)
    {
        return $"BIT_AND({src})";
    }

    /// <summary>
    /// Returns the bitwise OR of all bits in <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0.17 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT BIT_OR(flags) FROM permissions; -&gt; 7
    /// </code>
    /// </example>
    public static string BIT_OR(string src)
    {
        return $"BIT_OR({src})";
    }

    /// <summary>
    /// Returns the bitwise XOR of all bits in <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0.17 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT BIT_XOR(flags) FROM permissions; -&gt; 3
    /// </code>
    /// </example>
    public static string BIT_XOR(string src)
    {
        return $"BIT_XOR({src})";
    }

    /// <summary>
    /// Returns a concatenated string of non-NULL values of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT GROUP_CONCAT(name) FROM customers; -&gt; 'Alice,Bob,Carol'
    /// </code>
    /// </example>
    public static string GROUP_CONCAT(string src)
    {
        return $"GROUP_CONCAT({src})";
    }

    /// <summary>
    /// Aggregates a result set as a single JSON array whose elements are the values of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// JSON_ARRAYAGG() is available since MySQL 8.0 and MariaDB 10.5. ORDER BY is optional.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_ARRAYAGG(name) FROM customers;
    /// </code>
    /// </example>
    public static string JSON_ARRAYAGG(string src)
    {
        return $"JSON_ARRAYAGG({src})";
    }

    /// <summary>
    /// Aggregates a result set as a single JSON object whose key/value pairs are formed by <paramref name="key"/> and <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// JSON_OBJECTAGG() is available since MySQL 8.0 and MariaDB 10.5. ORDER BY is optional.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_OBJECTAGG(name, salary) FROM employees;
    /// </code>
    /// </example>
    public static string JSON_OBJECTAGG(string key, string value)
    {
        return $"JSON_OBJECTAGG({key}, {value})";
    }
}