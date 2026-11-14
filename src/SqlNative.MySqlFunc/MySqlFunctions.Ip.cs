using System;

namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Returns the numeric value of an IPv4 address in dotted-notation <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. The inverse of <see cref="INET_NTOA"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT INET_ATON('192.168.0.1'); -&gt; 3232235521
    /// </code>
    /// </example>
    public static QueryPart INET_ATON(QueryPart src)
    {
        return new QueryPart($"INET_ATON({src})");
    }

    /// <summary>
    /// Returns the dotted-notation IPv4 address from the numeric value <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. The inverse of <see cref="INET_ATON"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT INET_NTOA(3232235521); -&gt; '192.168.0.1'
    /// </code>
    /// </example>
    public static QueryPart INET_NTOA(QueryPart src)
    {
        return new QueryPart($"INET_NTOA({src})");
    }

    /// <summary>
    /// Returns the numeric value of the IPv6 address <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.6 and MariaDB 10.0. Returns a VARBINARY(16) value.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT HEX(INET6_ATON('::ffff:192.168.0.1')); -&gt; '00000000000000000000FFFFC0A80001'
    /// </code>
    /// </example>
    public static QueryPart INET6_ATON(QueryPart src)
    {
        return new QueryPart($"INET6_ATON({src})");
    }

    /// <summary>
    /// Returns the IPv6 address from the numeric value <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.6 and MariaDB 10.0. The inverse of <see cref="INET6_ATON"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT INET6_NTOA(INET6_ATON('::ffff:192.168.0.1')); -&gt; '::ffff:192.168.0.1'
    /// </code>
    /// </example>
    public static QueryPart INET6_NTOA(QueryPart src)
    {
        return new QueryPart($"INET6_NTOA({src})");
    }

    /// <summary>
    /// Returns 1 if <paramref name="src"/> is a valid IPv4 address, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.6 and MariaDB 10.0.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT IS_IPV4('192.168.0.1'), IS_IPV4('::1'); -&gt; 1, 0
    /// </code>
    /// </example>
    public static QueryPart IS_IPV4(QueryPart src)
    {
        return new QueryPart($"IS_IPV4({src})");
    }

    /// <summary>
    /// Returns 1 if <paramref name="src"/> is a valid IPv6 address, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.6 and MariaDB 10.0.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT IS_IPV6('::1'), IS_IPV6('192.168.0.1'); -&gt; 1, 0
    /// </code>
    /// </example>
    public static QueryPart IS_IPV6(QueryPart src)
    {
        return new QueryPart($"IS_IPV6({src})");
    }

    /// <summary>
    /// Returns 1 if <paramref name="src"/> is an IPv4-compatible IPv6 address, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.6 and MariaDB 10.0. Deprecated in MySQL 8.0.13.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT IS_IPV4_COMPAT(INET6_ATON('::192.168.0.1')); -&gt; 1
    /// </code>
    /// </example>
    [Obsolete("IS_IPV4_COMPAT is deprecated in MySQL 8.0.13.")]
    public static QueryPart IS_IPV4_COMPAT(QueryPart src)
    {
        return new QueryPart($"IS_IPV4_COMPAT({src})");
    }

    /// <summary>
    /// Returns 1 if <paramref name="src"/> is an IPv4-mapped IPv6 address, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.6 and MariaDB 10.0. Deprecated in MySQL 8.0.13.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT IS_IPV4_MAPPED(INET6_ATON('::ffff:192.168.0.1')); -&gt; 1
    /// </code>
    /// </example>
    [Obsolete("IS_IPV4_MAPPED is deprecated in MySQL 8.0.13.")]
    public static QueryPart IS_IPV4_MAPPED(QueryPart src)
    {
        return new QueryPart($"IS_IPV4_MAPPED({src})");
    }
}