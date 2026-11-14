namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Returns the absolute value of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ABS(-7), ABS(7); -&gt; 7, 7
    /// </code>
    /// </example>
    public static string ABS(string src)
    {
        return $"ABS({src})";
    }

    /// <summary>
    /// Returns the smallest integer value not less than <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CEIL(1.23), CEIL(-1.23); -&gt; 2, -1
    /// </code>
    /// </example>
    public static string CEIL(string src)
    {
        return $"CEIL({src})";
    }

    /// <summary>
    /// Synonym for <see cref="CEIL"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CEILING(1.23); -&gt; 2
    /// </code>
    /// </example>
    public static string CEILING(string src)
    {
        return $"CEILING({src})";
    }

    /// <summary>
    /// Returns the largest integer value not greater than <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT FLOOR(1.99); -&gt; 1
    /// </code>
    /// </example>
    public static string FLOOR(string src)
    {
        return $"FLOOR({src})";
    }

    /// <summary>
    /// Rounds <paramref name="src"/> to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ROUND(1.4567, 2); -&gt; 1.46
    /// </code>
    /// </example>
    public static string ROUND(string src, string decimals)
    {
        return $"ROUND({src}, {decimals})";
    }

    /// <summary>
    /// Truncates <paramref name="src"/> to <paramref name="decimals"/> decimal places.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT TRUNCATE(1.4567, 2); -&gt; 1.45
    /// </code>
    /// </example>
    public static string TRUNCATE(string src, string decimals)
    {
        return $"TRUNCATE({src}, {decimals})";
    }

    /// <summary>
    /// Modulo operation. Returns the remainder of <paramref name="a"/> divided by <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT MOD(10, 3); -&gt; 1
    /// </code>
    /// </example>
    public static string MOD(string a, string b)
    {
        return $"MOD({a}, {b})";
    }

    /// <summary>
    /// Returns <paramref name="x"/> raised to the power of <paramref name="y"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT POW(2, 8); -&gt; 256
    /// </code>
    /// </example>
    public static string POW(string x, string y)
    {
        return $"POW({x}, {y})";
    }

    /// <summary>
    /// Synonym for <see cref="POW"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT POWER(2, 8); -&gt; 256
    /// </code>
    /// </example>
    public static string POWER(string x, string y)
    {
        return $"POWER({x}, {y})";
    }

    /// <summary>
    /// Returns the square root of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SQRT(16); -&gt; 4
    /// </code>
    /// </example>
    public static string SQRT(string src)
    {
        return $"SQRT({src})";
    }

    /// <summary>
    /// Returns <c>e</c> raised to the power of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT EXP(1); -&gt; 2.718281828459045
    /// </code>
    /// </example>
    public static string EXP(string src)
    {
        return $"EXP({src})";
    }

    /// <summary>
    /// Returns the natural logarithm of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LN(2); -&gt; 0.6931471805599453
    /// </code>
    /// </example>
    public static string LN(string src)
    {
        return $"LN({src})";
    }

    /// <summary>
    /// Returns the logarithm of <paramref name="src"/> to base <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. The single-argument form LOG(x) is equivalent to LN(x) and was added in MySQL 4.0.10.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LOG(10, 100); -&gt; 2
    /// </code>
    /// </example>
    public static string LOG(string b, string src)
    {
        return $"LOG({b}, {src})";
    }

    /// <summary>
    /// Returns the base-2 logarithm of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LOG2(8); -&gt; 3
    /// </code>
    /// </example>
    public static string LOG2(string src)
    {
        return $"LOG2({src})";
    }

    /// <summary>
    /// Returns the base-10 logarithm of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LOG10(1000); -&gt; 3
    /// </code>
    /// </example>
    public static string LOG10(string src)
    {
        return $"LOG10({src})";
    }

    /// <summary>
    /// Returns the sign of <paramref name="src"/> as -1, 0, or 1.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SIGN(-42), SIGN(0), SIGN(42); -&gt; -1, 0, 1
    /// </code>
    /// </example>
    public static string SIGN(string src)
    {
        return $"SIGN({src})";
    }

    /// <summary>
    /// Returns a random floating-point value in the range [0, 1).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT RAND(); -&gt; 0.4137538411957428
    /// </code>
    /// </example>
    public static string RAND()
    {
        return "RAND()";
    }

    /// <summary>
    /// Returns a reproducible random floating-point value in [0, 1) using the given <paramref name="seed"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT RAND(1), RAND(1); -&gt; 0.40540353712147724, 0.40540353712147724
    /// </code>
    /// </example>
    public static string RAND(string seed)
    {
        return $"RAND({seed})";
    }

    /// <summary>
    /// Returns the value of π (pi) as a DOUBLE.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT PI(); -&gt; 3.141593
    /// </code>
    /// </example>
    public static string PI()
    {
        return "PI()";
    }

    /// <summary>
    /// Returns the sine of <paramref name="src"/> in radians.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SIN(PI() / 2); -&gt; 1
    /// </code>
    /// </example>
    public static string SIN(string src)
    {
        return $"SIN({src})";
    }

    /// <summary>
    /// Returns the cosine of <paramref name="src"/> in radians.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT COS(0); -&gt; 1
    /// </code>
    /// </example>
    public static string COS(string src)
    {
        return $"COS({src})";
    }

    /// <summary>
    /// Returns the tangent of <paramref name="src"/> in radians.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT TAN(0); -&gt; 0
    /// </code>
    /// </example>
    public static string TAN(string src)
    {
        return $"TAN({src})";
    }

    /// <summary>
    /// Returns the arc sine of <paramref name="src"/>, in radians in the range [-π/2, π/2].
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ASIN(1); -&gt; 1.5707963267948966
    /// </code>
    /// </example>
    public static string ASIN(string src)
    {
        return $"ASIN({src})";
    }

    /// <summary>
    /// Returns the arc cosine of <paramref name="src"/>, in radians in the range [0, π].
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ACOS(1); -&gt; 0
    /// </code>
    /// </example>
    public static string ACOS(string src)
    {
        return $"ACOS({src})";
    }

    /// <summary>
    /// Returns the arc tangent of <paramref name="src"/>, in radians in the range [-π/2, π/2].
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ATAN(1); -&gt; 0.7853981633974483
    /// </code>
    /// </example>
    public static string ATAN(string src)
    {
        return $"ATAN({src})";
    }

    /// <summary>
    /// Returns the arc tangent of the two arguments <paramref name="y"/> and <paramref name="x"/>, in the range [-π, π].
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ATAN2(1, 1); -&gt; 0.7853981633974483
    /// </code>
    /// </example>
    public static string ATAN2(string y, string x)
    {
        return $"ATAN2({y}, {x})";
    }

    /// <summary>
    /// Returns the cotangent of <paramref name="src"/> in radians.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT COT(1); -&gt; 0.642092615934331
    /// </code>
    /// </example>
    public static string COT(string src)
    {
        return $"COT({src})";
    }

    /// <summary>
    /// Converts <paramref name="src"/> from radians to degrees.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT DEGREES(PI()); -&gt; 180
    /// </code>
    /// </example>
    public static string DEGREES(string src)
    {
        return $"DEGREES({src})";
    }

    /// <summary>
    /// Converts <paramref name="src"/> from degrees to radians.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT RADIANS(180); -&gt; 3.141592653589793
    /// </code>
    /// </example>
    public static string RADIANS(string src)
    {
        return $"RADIANS({src})";
    }

    /// <summary>
    /// Converts <paramref name="N"/> from one number base (<paramref name="fromBase"/>) to another (<paramref name="toBase"/>).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1. The minimum base is 2 and the maximum base is 36.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CONV('a', 16, 2); -&gt; '1010'
    /// </code>
    /// </example>
    public static string CONV(string N, string fromBase, string toBase)
    {
        return $"CONV({N}, {fromBase}, {toBase})";
    }

    /// <summary>
    /// Computes a cyclic redundancy check (CRC32) value for <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1. Uses the ISO 3309 polynomial (same as zlib/gzip).
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CRC32('MySQL'); -&gt; 3259397556
    /// </code>
    /// </example>
    public static string CRC32(string src)
    {
        return $"CRC32({src})";
    }
}