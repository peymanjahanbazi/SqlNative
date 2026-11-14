namespace SqlNative;

public partial class MySqlFunctions
{
    // ===================================================================
    // Arithmetic operators
    // ===================================================================

    /// <summary>
    /// Adds two or more values: <c>a + b</c>, <c>a + b + c</c>, ...
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>+</c> arithmetic operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpAdd(a, b)        -&gt; (a + b)
    /// OpAdd(a, b, c)     -&gt; (a + b + c)
    /// OpAdd(a, b, c, d)  -&gt; (a + b + c + d)
    /// </code>
    /// </example>
    public static string OpAdd(params string[] args)
    {
        if (args is null || args.Length < 2)
            throw new ArgumentException("OpAdd requires at least two operands.", nameof(args));
        return $"({string.Join(" + ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Subtracts the second and following operands from the first: <c>a - b - c - ...</c>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>-</c> arithmetic operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpSub(a, b)       -&gt; (a - b)
    /// OpSub(a, b, c)    -&gt; (a - b - c)
    /// </code>
    /// </example>
    public static string OpSub(params string[] args)
    {
        if (args is null || args.Length < 2)
            throw new ArgumentException("OpSub requires at least two operands.", nameof(args));
        return $"({string.Join(" - ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Multiplies two or more values: <c>a * b * c * ...</c>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>*</c> arithmetic operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpMul(a, b)      -&gt; (a * b)
    /// OpMul(a, b, c)   -&gt; (a * b * c)
    /// </code>
    /// </example>
    public static string OpMul(params string[] args)
    {
        if (args is null || args.Length < 2)
            throw new ArgumentException("OpMul requires at least two operands.", nameof(args));
        return $"({string.Join(" * ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Divides the first operand by the second and following: <c>a / b / c / ...</c>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>/</c> arithmetic operator. Division by zero returns NULL. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpDiv(a, b)      -&gt; (a / b)
    /// OpDiv(a, b, c)   -&gt; (a / b / c)
    /// </code>
    /// </example>
    public static string OpDiv(params string[] args)
    {
        if (args is null || args.Length < 2)
            throw new ArgumentException("OpDiv requires at least two operands.", nameof(args));
        return $"({string.Join(" / ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Returns the remainder of division (modulo). Two or more operands: <c>a % b % c % ...</c>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>%</c> arithmetic operator. Also accessible as MOD() in MySQL. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpMod(a, b)      -&gt; (a % b)
    /// OpMod(a, b, c)   -&gt; (a % b % c)
    /// </code>
    /// </example>
    public static string OpMod(params string[] args)
    {
        if (args is null || args.Length < 2)
            throw new ArgumentException("OpMod requires at least two operands.", nameof(args));
        return $"({string.Join(" % ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Integer division of <paramref name="a"/> by <paramref name="b"/>, dropping any fractional part.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>DIV</c> arithmetic operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpDivInt(a, b)   -&gt; (a DIV b)
    /// </code>
    /// </example>
    public static string OpDivInt(string a, string b)
    {
        return $"({a} DIV {b})";
    }

    /// <summary>
    /// Returns the unary minus of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL unary <c>-</c> operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpUnaryMinus(a)  -&gt; (-a)
    /// </code>
    /// </example>
    public static string OpUnaryMinus(string src)
    {
        return $"(-{src})";
    }

    // ===================================================================
    // Comparison operators
    // ===================================================================

    /// <summary>
    /// Tests whether <paramref name="a"/> equals <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>=</c> comparison operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpEqEq(a, b)     -&gt; (a = b)
    /// </code>
    /// </example>
    public static string OpEqEq(string a, string b)
    {
        return $"({a} = {b})";
    }

    /// <summary>
    /// Tests whether <paramref name="a"/> is not equal to <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>&lt;&gt;</c> comparison operator. MySQL also accepts <c>!=</c> as a synonym. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpNotEq(a, b)    -&gt; (a &lt;&gt; b)
    /// </code>
    /// </example>
    public static string OpNotEq(string a, string b)
    {
        return $"({a} <> {b})";
    }

    /// <summary>
    /// NULL-safe equal-to: returns 1 when both operands are NULL or equal, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>&lt;=&gt;</c> NULL-safe equality operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpNullSafeEq(a, b)  -&gt; (a &lt;=&gt; b)
    /// </code>
    /// </example>
    public static string OpNullSafeEq(string a, string b)
    {
        return $"({a} <=> {b})";
    }

    /// <summary>
    /// Tests whether <paramref name="a"/> is less than <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>&lt;</c> comparison operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpLt(a, b)       -&gt; (a &lt; b)
    /// </code>
    /// </example>
    public static string OpLt(string a, string b)
    {
        return $"({a} < {b})";
    }

    /// <summary>
    /// Tests whether <paramref name="a"/> is less than or equal to <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>&lt;=</c> comparison operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpLte(a, b)      -&gt; (a &lt;= b)
    /// </code>
    /// </example>
    public static string OpLte(string a, string b)
    {
        return $"({a} <= {b})";
    }

    /// <summary>
    /// Tests whether <paramref name="a"/> is greater than <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>&gt;</c> comparison operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpGt(a, b)       -&gt; (a &gt; b)
    /// </code>
    /// </example>
    public static string OpGt(string a, string b)
    {
        return $"({a} > {b})";
    }

    /// <summary>
    /// Tests whether <paramref name="a"/> is greater than or equal to <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>&gt;=</c> comparison operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpGte(a, b)      -&gt; (a &gt;= b)
    /// </code>
    /// </example>
    public static string OpGte(string a, string b)
    {
        return $"({a} >= {b})";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> IS NULL.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>IS NULL</c> operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpIsNull(a)      -&gt; (a IS NULL)
    /// </code>
    /// </example>
    public static string OpIsNull(string src)
    {
        return $"({src} IS NULL)";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> IS NOT NULL.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>IS NOT NULL</c> operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpIsNotNull(a)   -&gt; (a IS NOT NULL)
    /// </code>
    /// </example>
    public static string OpIsNotNull(string src)
    {
        return $"({src} IS NOT NULL)";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> IS TRUE.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>IS TRUE</c> operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpIsTrue(a)      -&gt; (a IS TRUE)
    /// </code>
    /// </example>
    public static string OpIsTrue(string src)
    {
        return $"({src} IS TRUE)";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> IS FALSE.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>IS FALSE</c> operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpIsFalse(a)     -&gt; (a IS FALSE)
    /// </code>
    /// </example>
    public static string OpIsFalse(string src)
    {
        return $"({src} IS FALSE)";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> IS UNKNOWN (NULL).
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>IS UNKNOWN</c> operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpIsUnknown(a)   -&gt; (a IS UNKNOWN)
    /// </code>
    /// </example>
    public static string OpIsUnknown(string src)
    {
        return $"({src} IS UNKNOWN)";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> IS NOT TRUE.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>IS NOT TRUE</c> operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpIsNotTrue(a)   -&gt; (a IS NOT TRUE)
    /// </code>
    /// </example>
    public static string OpIsNotTrue(string src)
    {
        return $"({src} IS NOT TRUE)";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> IS NOT FALSE.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>IS NOT FALSE</c> operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpIsNotFalse(a)  -&gt; (a IS NOT FALSE)
    /// </code>
    /// </example>
    public static string OpIsNotFalse(string src)
    {
        return $"({src} IS NOT FALSE)";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> IS NOT UNKNOWN (i.e. is NOT NULL).
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>IS NOT UNKNOWN</c> operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpIsNotUnknown(a) -&gt; (a IS NOT UNKNOWN)
    /// </code>
    /// </example>
    public static string OpIsNotUnknown(string src)
    {
        return $"({src} IS NOT UNKNOWN)";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> matches the SQL LIKE <paramref name="pattern"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>LIKE</c> operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpLike(a, b)     -&gt; (a LIKE b)
    /// </code>
    /// </example>
    public static string OpLike(string src, string pattern)
    {
        return $"({src} LIKE {pattern})";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> does NOT match the SQL LIKE <paramref name="pattern"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>NOT LIKE</c> operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpNotLike(a, b)  -&gt; (a NOT LIKE b)
    /// </code>
    /// </example>
    public static string OpNotLike(string src, string pattern)
    {
        return $"({src} NOT LIKE {pattern})";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> matches the regular expression <paramref name="pattern"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>REGEXP</c> operator (also <c>RLIKE</c>). Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpRegexp(a, b)   -&gt; (a REGEXP b)
    /// </code>
    /// </example>
    public static string OpRegexp(string src, string pattern)
    {
        return $"({src} REGEXP {pattern})";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> does NOT match the regular expression <paramref name="pattern"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>NOT REGEXP</c> operator (also <c>NOT RLIKE</c>). Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpNotRegexp(a, b) -&gt; (a NOT REGEXP b)
    /// </code>
    /// </example>
    public static string OpNotRegexp(string src, string pattern)
    {
        return $"({src} NOT REGEXP {pattern})";
    }

    /// <summary>
    /// Tests whether two strings sound the same when spoken (uses SOUNDEX internally).
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>SOUNDS LIKE</c> operator. Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpSoundsLike(a, b) -&gt; (a SOUNDS LIKE b)
    /// </code>
    /// </example>
    public static string OpSoundsLike(string a, string b)
    {
        return $"({a} SOUNDS LIKE {b})";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> is between <paramref name="low"/> and <paramref name="high"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>BETWEEN ... AND ...</c> operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpBetween(a, l, h) -&gt; (a BETWEEN l AND h)
    /// </code>
    /// </example>
    public static string OpBetween(string src, string low, string high)
    {
        return $"({src} BETWEEN {low} AND {high})";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> is NOT between <paramref name="low"/> and <paramref name="high"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>NOT BETWEEN ... AND ...</c> operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpNotBetween(a, l, h) -&gt; (a NOT BETWEEN l AND h)
    /// </code>
    /// </example>
    public static string OpNotBetween(string src, string low, string high)
    {
        return $"({src} NOT BETWEEN {low} AND {high})";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> equals any of the supplied <paramref name="values"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>IN (...)</c> operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpIn(a, b, c)       -&gt; (a IN (b, c))
    /// OpIn(a, b, c, d)    -&gt; (a IN (b, c, d))
    /// </code>
    /// </example>
    public static string OpIn(string src, params string[] values)
    {
        if (values is null || values.Length < 1)
            throw new ArgumentException("OpIn requires at least one value.", nameof(values));
        return $"({src} IN ({string.Join(", ", values.Select(x => x.ToString()))}))";
    }

    /// <summary>
    /// Tests whether <paramref name="src"/> does NOT equal any of the supplied <paramref name="values"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>NOT IN (...)</c> operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpNotIn(a, b, c)    -&gt; (a NOT IN (b, c))
    /// OpNotIn(a, b, c, d) -&gt; (a NOT IN (b, c, d))
    /// </code>
    /// </example>
    public static string OpNotIn(string src, params string[] values)
    {
        if (values is null || values.Length < 1)
            throw new ArgumentException("OpNotIn requires at least one value.", nameof(values));
        return $"({src} NOT IN ({string.Join(", ", values.Select(x => x.ToString()))}))";
    }

    /// <summary>
    /// Returns the greatest (maximum) of the supplied arguments.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>GREATEST(...)</c> function (used as an operator). Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpGreatest(a, b)       -&gt; GREATEST(a, b)
    /// OpGreatest(a, b, c)    -&gt; GREATEST(a, b, c)
    /// </code>
    /// </example>
    public static string OpGreatest(params string[] args)
    {
        if (args is null || args.Length < 2)
            throw new ArgumentException("OpGreatest requires at least two arguments.", nameof(args));
        return $"GREATEST({string.Join(", ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Returns the smallest (minimum) of the supplied arguments.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>LEAST(...)</c> function (used as an operator). Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpLeast(a, b)       -&gt; LEAST(a, b)
    /// OpLeast(a, b, c)    -&gt; LEAST(a, b, c)
    /// </code>
    /// </example>
    public static string OpLeast(params string[] args)
    {
        if (args is null || args.Length < 2)
            throw new ArgumentException("OpLeast requires at least two arguments.", nameof(args));
        return $"LEAST({string.Join(", ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Returns the index of the first argument in <paramref name="Ns"/> that is less than <paramref name="N"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>INTERVAL(N, N1, N2, ...)</c> function (used as an operator). Returns 0 if <paramref name="N"/> is less than the first value, 1 if less than the second, etc. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpInterval(23, 1, 15, 17, 30)  -&gt; INTERVAL(23, 1, 15, 17, 30)
    /// </code>
    /// </example>
    public static string OpInterval(string N, params string[] Ns)
    {
        if (Ns is null || Ns.Length < 1)
            throw new ArgumentException("OpInterval requires at least one comparison value.", nameof(Ns));
        return $"INTERVAL({N}, {string.Join(", ", Ns.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Tests whether the <paramref name="subquery"/> returns any rows.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>EXISTS (subquery)</c> operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpExists(sub) -&gt; EXISTS (sub)
    /// </code>
    /// </example>
    public static string OpExists(string subquery)
    {
        return $"EXISTS ({subquery})";
    }

    /// <summary>
    /// Tests whether the <paramref name="subquery"/> returns no rows.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>NOT EXISTS (subquery)</c> operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpNotExists(sub) -&gt; NOT EXISTS (sub)
    /// </code>
    /// </example>
    public static string OpNotExists(string subquery)
    {
        return $"NOT EXISTS ({subquery})";
    }

    /// <summary>
    /// Returns TRUE if <paramref name="src"/> equals any row returned by <paramref name="subquery"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>= ANY (subquery)</c> operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpAny(a, sub) -&gt; a = ANY (sub)
    /// </code>
    /// </example>
    public static string OpAny(string src, string subquery)
    {
        return $"{src} = ANY ({subquery})";
    }

    /// <summary>
    /// Synonym for <see cref="OpAny"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>= SOME (subquery)</c> operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpSome(a, sub) -&gt; a = SOME (sub)
    /// </code>
    /// </example>
    public static string OpSome(string src, string subquery)
    {
        return $"{src} = SOME ({subquery})";
    }

    /// <summary>
    /// Returns TRUE if <paramref name="src"/> is not equal to every row returned by <paramref name="subquery"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>&lt;&gt; ALL (subquery)</c> operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpAll(a, sub) -&gt; a &lt;&gt; ALL (sub)
    /// </code>
    /// </example>
    public static string OpAll(string src, string subquery)
    {
        return $"{src} <> ALL ({subquery})";
    }

    // ===================================================================
    // Logical operators
    // ===================================================================

    /// <summary>
    /// Logical AND of two or more conditions: <c>a AND b AND c AND ...</c>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>AND</c> operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpAnd(a, b)      -&gt; (a AND b)
    /// OpAnd(a, b, c)   -&gt; (a AND b AND c)
    /// </code>
    /// </example>
    public static string OpAnd(params string[] args)
    {
        if (args is null || args.Length < 2)
            throw new ArgumentException("OpAnd requires at least two operands.", nameof(args));
        return $"({string.Join(" AND ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Logical OR of two or more conditions: <c>a OR b OR c OR ...</c>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>OR</c> operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpOr(a, b)      -&gt; (a OR b)
    /// OpOr(a, b, c)   -&gt; (a OR b OR c)
    /// </code>
    /// </example>
    public static string OpOr(params string[] args)
    {
        if (args is null || args.Length < 2)
            throw new ArgumentException("OpOr requires at least two operands.", nameof(args));
        return $"({string.Join(" OR ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Logical XOR of <paramref name="a"/> and <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>XOR</c> operator. Returns TRUE when exactly one operand is TRUE. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpXor(a, b)     -&gt; (a XOR b)
    /// </code>
    /// </example>
    public static string OpXor(string a, string b)
    {
        return $"({a} XOR {b})";
    }

    /// <summary>
    /// Logical negation of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL unary <c>NOT</c> operator. Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpNot(a)        -&gt; (NOT a)
    /// </code>
    /// </example>
    public static string OpNot(string src)
    {
        return $"(NOT {src})";
    }

    // ===================================================================
    // Bitwise operators
    // ===================================================================

    /// <summary>
    /// Bitwise AND of two or more values: <c>a &amp; b &amp; c &amp; ...</c>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>&amp;</c> bitwise operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpBitAnd(a, b)     -&gt; (a &amp; b)
    /// OpBitAnd(a, b, c)  -&gt; (a &amp; b &amp; c)
    /// </code>
    /// </example>
    public static string OpBitAnd(params string[] args)
    {
        if (args is null || args.Length < 2)
            throw new ArgumentException("OpBitAnd requires at least two operands.", nameof(args));
        return $"({string.Join(" & ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Bitwise OR of two or more values: <c>a | b | c | ...</c>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>|</c> bitwise operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpBitOr(a, b)      -&gt; (a | b)
    /// OpBitOr(a, b, c)   -&gt; (a | b | c)
    /// </code>
    /// </example>
    public static string OpBitOr(params string[] args)
    {
        if (args is null || args.Length < 2)
            throw new ArgumentException("OpBitOr requires at least two operands.", nameof(args));
        return $"({string.Join(" | ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Bitwise XOR of two or more values: <c>a ^ b ^ c ^ ...</c>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>^</c> bitwise operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpBitXor(a, b)     -&gt; (a ^ b)
    /// OpBitXor(a, b, c)  -&gt; (a ^ b ^ c)
    /// </code>
    /// </example>
    public static string OpBitXor(params string[] args)
    {
        if (args is null || args.Length < 2)
            throw new ArgumentException("OpBitXor requires at least two operands.", nameof(args));
        return $"({string.Join(" ^ ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Returns the bitwise inversion (one's complement) of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL unary <c>~</c> bitwise operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpBitNot(a)  -&gt; (~a)
    /// </code>
    /// </example>
    public static string OpBitNot(string src)
    {
        return $"(~{src})";
    }

    /// <summary>
    /// Bitwise left shift of <paramref name="src"/> by <paramref name="count"/> positions.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>&lt;&lt;</c> bitwise operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpBitLeftShift(a, n)  -&gt; (a &lt;&lt; n)
    /// </code>
    /// </example>
    public static string OpBitLeftShift(string src, string count)
    {
        return $"({src} << {count})";
    }

    /// <summary>
    /// Bitwise right shift of <paramref name="src"/> by <paramref name="count"/> positions.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>&gt;&gt;</c> bitwise operator. Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpBitRightShift(a, n)  -&gt; (a &gt;&gt; n)
    /// </code>
    /// </example>
    public static string OpBitRightShift(string src, string count)
    {
        return $"({src} >> {count})";
    }

    // ===================================================================
    // Cast / Conversion operator
    // ===================================================================

    /// <summary>
    /// Casts <paramref name="src"/> to a binary string.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>BINARY</c> operator. Deprecated in MySQL 8.0.27 in favor of CAST(... AS BINARY). Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpBinary(a)  -&gt; BINARY a
    /// </code>
    /// </example>
    [Obsolete("BINARY is deprecated in MySQL 8.0.27. Use CAST(... AS BINARY) instead.")]
    public static string OpBinary(string src)
    {
        return $"BINARY {src}";
    }

    // ===================================================================
    // Assignment operator
    // ===================================================================

    /// <summary>
    /// Assigns <paramref name="value"/> to <paramref name="target"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>:=</c> assignment operator. Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpAssign(t, v)  -&gt; t := v
    /// </code>
    /// </example>
    public static string OpAssign(string target, string value)
    {
        return $"{target} := {value}";
    }

    // ===================================================================
    // JSON operator
    // ===================================================================

    /// <summary>
    /// Returns true (1) if the <paramref name="operand"/> matches any element of the JSON array <paramref name="jsonArray"/>.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>MEMBER OF</c> operator. Available since MySQL 8.0.17 and MariaDB 10.9.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpMemberOf(2, json_arr)  -&gt; 2 MEMBER OF (json_arr)
    /// </code>
    /// </example>
    public static string OpMemberOf(string operand, string jsonArray)
    {
        return $"{operand} MEMBER OF ({jsonArray})";
    }

    // ===================================================================
    // String comparison helpers (function form, used as operators)
    // ===================================================================

    /// <summary>
    /// Compares two strings using their collation. Returns -1, 0, or 1.
    /// </summary>
    /// <remarks>
    /// Emits the SQL <c>STRCMP(a, b)</c> function. Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// OpStrCmp(a, b)  -&gt; STRCMP(a, b)
    /// </code>
    /// </example>
    public static string OpStrCmp(string a, string b)
    {
        return $"STRCMP({a}, {b})";
    }
}