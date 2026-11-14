namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Returns the number of the current row within its partition. Rows numbered 1, 2, ... with no gaps.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.2. Used as a window function — combine with OVER() and optional PARTITION BY / ORDER BY clauses.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ROW_NUMBER() OVER (ORDER BY salary DESC) AS rn, name FROM employees;
    /// </code>
    /// </example>
    public static string ROW_NUMBER()
    {
        return "ROW_NUMBER() OVER ()";
    }

    /// <summary>
    /// Returns the rank of the current row within its partition, with gaps. Rows with equal values receive the same rank; the next row gets rank + count of peers.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.2. Must be used as a window function with OVER().
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT RANK() OVER (ORDER BY score DESC) AS rk, name FROM players;
    /// </code>
    /// </example>
    public static string RANK()
    {
        return "RANK() OVER ()";
    }

    /// <summary>
    /// Returns the rank of the current row within its partition, with no gaps. Tied rows share a rank and the next rank is consecutive.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.2. Must be used as a window function with OVER().
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT DENSE_RANK() OVER (ORDER BY score DESC) AS rk, name FROM players;
    /// </code>
    /// </example>
    public static string DENSE_RANK()
    {
        return "DENSE_RANK() OVER ()";
    }

    /// <summary>
    /// Returns the relative rank of the current row as (rank - 1) / (partition_rows - 1). Always returns 0 for the first row in the partition.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.2. Must be used as a window function with OVER().
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT PERCENT_RANK() OVER (ORDER BY score DESC) AS pr, name FROM players;
    /// </code>
    /// </example>
    public static string PERCENT_RANK()
    {
        return "PERCENT_RANK() OVER ()";
    }

    /// <summary>
    /// Returns the cumulative distribution of the current row within its partition — count of preceding or peer rows / total partition rows.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.2. Must be used as a window function with OVER().
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CUME_DIST() OVER (ORDER BY score DESC) AS cd, name FROM players;
    /// </code>
    /// </example>
    public static string CUME_DIST()
    {
        return "CUME_DIST() OVER ()";
    }

    /// <summary>
    /// Divides the partition into <paramref name="n"/> buckets and assigns each row its 1-based bucket number.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.2. Must be used as a window function with OVER().
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT NTILE(4) OVER (ORDER BY score DESC) AS bucket, name FROM players;
    /// </code>
    /// </example>
    public static string NTILE(string n)
    {
        return $"NTILE({n}) OVER ()";
    }

    /// <summary>
    /// Returns the value of <paramref name="expr"/> from the row that is <paramref name="offset"/> rows before the current row within its partition.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.2. <paramref name="defaultValue"/> is returned when there is no row at the offset. Must be used with OVER().
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LAG(salary, 1, 0) OVER (ORDER BY hire_date) AS prev_salary FROM employees;
    /// </code>
    /// </example>
    public static string LAG(string expr, string offset, string defaultValue)
    {
        return $"LAG({expr}, {offset}, {defaultValue}) OVER ()";
    }

    /// <summary>
    /// Returns the value of <paramref name="expr"/> from the row that is <paramref name="offset"/> rows after the current row within its partition.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.2. <paramref name="defaultValue"/> is returned when there is no row at the offset. Must be used with OVER().
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LEAD(salary, 1, 0) OVER (ORDER BY hire_date) AS next_salary FROM employees;
    /// </code>
    /// </example>
    public static string LEAD(string expr, string offset, string defaultValue)
    {
        return $"LEAD({expr}, {offset}, {defaultValue}) OVER ()";
    }

    /// <summary>
    /// Returns the value of <paramref name="expr"/> from the first row of the window frame.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.2. Must be used as a window function with OVER().
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT FIRST_VALUE(name) OVER (PARTITION BY dept ORDER BY salary DESC) AS top_earner FROM employees;
    /// </code>
    /// </example>
    public static string FIRST_VALUE(string expr)
    {
        return $"FIRST_VALUE({expr}) OVER ()";
    }

    /// <summary>
    /// Returns the value of <paramref name="expr"/> from the last row of the window frame.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.2. Must be used as a window function with OVER().
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LAST_VALUE(name) OVER (PARTITION BY dept ORDER BY salary DESC) FROM employees;
    /// </code>
    /// </example>
    public static string LAST_VALUE(string expr)
    {
        return $"LAST_VALUE({expr}) OVER ()";
    }

    /// <summary>
    /// Returns the value of <paramref name="expr"/> from the row at <paramref name="n"/> (1-based) within the window frame.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0 and MariaDB 10.2. Must be used as a window function with OVER().
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT NTH_VALUE(salary, 2) OVER (PARTITION BY dept ORDER BY salary DESC) AS second_highest FROM employees;
    /// </code>
    /// </example>
    public static string NTH_VALUE(string expr, string n)
    {
        return $"NTH_VALUE({expr}, {n}) OVER ()";
    }
}