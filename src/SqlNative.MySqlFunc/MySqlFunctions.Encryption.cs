namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Returns the MD5 message-digest of <paramref name="src"/> as a hex string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. Returns a 32-character lowercase hex string.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT MD5('hello'); -&gt; '5d41402abc4b2a76b9719d911017c592'
    /// </code>
    /// </example>
    public static string MD5(string src)
    {
        return $"MD5({src})";
    }

    /// <summary>
    /// Returns the SHA-1 160-bit checksum of <paramref name="src"/> as a hex string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0.2 and MariaDB 5.1. Returns a 40-character hex string. SHA-1 is cryptographically broken — prefer SHA2().
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SHA1('hello'); -&gt; 'aaf4c61ddcc5e8a2dabede0f3b482cd9aea9434d'
    /// </code>
    /// </example>
    public static string SHA1(string src)
    {
        return $"SHA1({src})";
    }

    /// <summary>
    /// Returns the SHA-2 checksum of <paramref name="src"/> with the specified <paramref name="hashLength"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 6.0.5 and MariaDB 10.5. <paramref name="hashLength"/> must be 224, 256, 384, 512, or 0 (equivalent to 256).
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SHA2('hello', 256); -&gt; '2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824'
    /// </code>
    /// </example>
    public static string SHA2(string src, string hashLength)
    {
        return $"SHA2({src}, {hashLength})";
    }

    /// <summary>
    /// Encrypts <paramref name="src"/> using AES with the given <paramref name="key"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0.2 and MariaDB 5.1. AES_ENCRYPT() requires the secure-file-priv system variable to be set properly in modern MySQL.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT HEX(AES_ENCRYPT('hello', 'key123')); -&gt; '7B3E89C5...'
    /// </code>
    /// </example>
    public static string AES_ENCRYPT(string src, string key)
    {
        return $"AES_ENCRYPT({src}, {key})";
    }

    /// <summary>
    /// Decrypts the AES-encrypted <paramref name="crypted"/> string using the given <paramref name="key"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0.2 and MariaDB 5.1. Returns NULL when decryption fails (wrong key or tampered data).
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT AES_DECRYPT(AES_ENCRYPT('hello', 'key123'), 'key123'); -&gt; 'hello'
    /// </code>
    /// </example>
    public static string AES_DECRYPT(string crypted, string key)
    {
        return $"AES_DECRYPT({crypted}, {key})";
    }

    /// <summary>
    /// Compresses <paramref name="src"/> and returns the result as a binary string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1. Result must be stored in a BLOB column.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LENGTH(COMPRESS('hello')), LENGTH('hello'); -&gt; 21, 5
    /// </code>
    /// </example>
    public static string COMPRESS(string src)
    {
        return $"COMPRESS({src})";
    }

    /// <summary>
    /// Uncompresses a previously compressed string <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1. Returns NULL if the input is not a valid compressed value.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT UNCOMPRESS(COMPRESS('hello')); -&gt; 'hello'
    /// </code>
    /// </example>
    public static string UNCOMPRESS(string src)
    {
        return $"UNCOMPRESS({src})";
    }

    /// <summary>
    /// Returns the length of <paramref name="src"/> before compression.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1. Returns NULL if the input is not a valid compressed value.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT UNCOMPRESSED_LENGTH(COMPRESS('hello')); -&gt; 5
    /// </code>
    /// </example>
    public static string UNCOMPRESSED_LENGTH(string src)
    {
        return $"UNCOMPRESSED_LENGTH({src})";
    }

    /// <summary>
    /// Returns a binary string of <paramref name="length"/> random bytes.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.6.17 and MariaDB 10.1.13. <paramref name="length"/> must be a positive integer.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT HEX(RANDOM_BYTES(4)); -&gt; '5A8B1C2D'
    /// </code>
    /// </example>
    public static string RANDOM_BYTES(string length)
    {
        return $"RANDOM_BYTES({length})";
    }

    /// <summary>
    /// Returns an integer indicating the strength of <paramref name="src"/> as a password.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.6 and MariaDB 10.1. Deprecated in MySQL 8.0.17 in favor of the validate_password component.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT VALIDATE_PASSWORD_STRENGTH('weak'); -&gt; 25
    /// </code>
    /// </example>
    public static string VALIDATE_PASSWORD_STRENGTH(string src)
    {
        return $"VALIDATE_PASSWORD_STRENGTH({src})";
    }

    /// <summary>
    /// Synonym for <see cref="SHA1"/>. Returns the SHA-1 160-bit checksum of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0.2 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SHA('hello'); -&gt; 'aaf4c61ddcc5e8a2dabede0f3b482cd9aea9434d'
    /// </code>
    /// </example>
    public static string SHA(string src)
    {
        return $"SHA({src})";
    }

    /// <summary>
    /// Computes the statement digest hash value from a SQL statement as an SHA-256 hex string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.1. Used by the performance_schema events_statements_history tables.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT STATEMENT_DIGEST('SELECT * FROM t'); -&gt; '...'
    /// </code>
    /// </example>
    public static string STATEMENT_DIGEST(string statement)
    {
        return $"STATEMENT_DIGEST({statement})";
    }

    /// <summary>
    /// Computes the normalized statement digest (the canonicalized text) of a SQL statement.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.1. Used by the performance_schema events_statements_history tables.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT STATEMENT_DIGEST_TEXT('SELECT * FROM t WHERE id = 1'); -&gt; 'SELECT * FROM `t` WHERE `id` = ?'
    /// </code>
    /// </example>
    public static string STATEMENT_DIGEST_TEXT(string statement)
    {
        return $"STATEMENT_DIGEST_TEXT({statement})";
    }
}