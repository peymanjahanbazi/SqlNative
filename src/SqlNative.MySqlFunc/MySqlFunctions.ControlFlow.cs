namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Returns <paramref name="thenExpr"/> if <paramref name="condition"/> is TRUE, otherwise <paramref name="elseExpr"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT IF(1 &lt; 2, 'yes', 'no'); -&gt; 'yes'
    /// </code>
    /// </example>
    public static string IF(string condition, object thenExpr, object elseExpr)
    {
        return $"IF({condition}, {thenExpr}, {elseExpr})";
    }

    /// <summary>
    /// Returns <paramref name="src"/> if not NULL, otherwise <paramref name="alt"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT IFNULL(NULL, 'fallback'); -&gt; 'fallback'
    /// </code>
    /// </example>
    public static string IFNULL(string src, object alt)
    {
        return $"IFNULL({src}, {alt})";
    }

    /// <summary>
    /// Returns NULL if <paramref name="expr1"/> equals <paramref name="expr2"/>, otherwise <paramref name="expr1"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT NULLIF(1, 1), NULLIF(1, 2); -&gt; NULL, 1
    /// </code>
    /// </example>
    public static string NULLIF(string expr1, string expr2)
    {
        return $"NULLIF({expr1}, {expr2})";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> is NULL.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ISNULL(NULL), ISNULL(1); -&gt; 1, 0
    /// </code>
    /// </example>
    public static string ISNULL(string src)
    {
        return $"ISNULL({src})";
    }

    /// <summary>
    /// Returns the first non-NULL argument.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT COALESCE(NULL, NULL, 'third', 'fourth'); -&gt; 'third'
    /// </code>
    /// </example>
    public static string COALESCE(params string[] args)
    {
        return $"COALESCE({string.Join(", ", args.Select(x => x.ToString()))})";
    }
}