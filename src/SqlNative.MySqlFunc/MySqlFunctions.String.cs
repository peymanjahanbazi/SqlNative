namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Returns the substring of <paramref name="src"/> starting at <paramref name="start"/> for <paramref name="length"/> characters.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SUBSTRING('Hello World', 7, 5); -&gt; 'World'
    /// </code>
    /// </example>
    public static string SUBSTRING(string src, string start, string length)
    {
        return $"SUBSTRING({src} FROM {start} FOR {length})";
    }

    /// <summary>
    /// Returns the substring of <paramref name="src"/> before <paramref name="count"/> occurrences of <paramref name="delimiter"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1. Negative <paramref name="count"/> counts from the right.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SUBSTRING_INDEX('a,b,c,d', ',', 2); -&gt; 'a,b'
    /// </code>
    /// </example>
    public static string SUBSTRING_INDEX(string src, string delimiter, string count)
    {
        return $"SUBSTRING_INDEX({src}, {delimiter}, {count})";
    }

    /// <summary>
    /// Returns the leftmost <paramref name="length"/> characters of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LEFT('Hello', 3); -&gt; 'Hel'
    /// </code>
    /// </example>
    public static string LEFT(string src, string length)
    {
        return $"LEFT({src}, {length})";
    }

    /// <summary>
    /// Returns the rightmost <paramref name="length"/> characters of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT RIGHT('Hello', 3); -&gt; 'llo'
    /// </code>
    /// </example>
    public static string RIGHT(string src, string length)
    {
        return $"RIGHT({src}, {length})";
    }

    /// <summary>
    /// Returns the length of <paramref name="src"/> in bytes.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LENGTH('Hello'); -&gt; 5
    /// </code>
    /// </example>
    public static string LENGTH(string src)
    {
        return $"LENGTH({src})";
    }

    /// <summary>
    /// Returns the length of <paramref name="src"/> in characters.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 10.0.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CHAR_LENGTH('Hello'); -&gt; 5
    /// </code>
    /// </example>
    public static string CHAR_LENGTH(string src)
    {
        return $"CHAR_LENGTH({src})";
    }

    /// <summary>
    /// Returns the length of <paramref name="src"/> in bits.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT BIT_LENGTH('Hello'); -&gt; 40
    /// </code>
    /// </example>
    public static string BIT_LENGTH(string src)
    {
        return $"BIT_LENGTH({src})";
    }

    /// <summary>
    /// Returns the length of <paramref name="src"/> in octets (bytes).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT OCTET_LENGTH('Hello'); -&gt; 5
    /// </code>
    /// </example>
    public static string OCTET_LENGTH(string src)
    {
        return $"OCTET_LENGTH({src})";
    }

    /// <summary>
    /// Synonym for <see cref="CHAR_LENGTH"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CHARACTER_LENGTH('Hello'); -&gt; 5
    /// </code>
    /// </example>
    public static string CHARACTER_LENGTH(string src)
    {
        return $"CHARACTER_LENGTH({src})";
    }

    /// <summary>
    /// Converts <paramref name="src"/> to uppercase.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT UPPER('Hello'); -&gt; 'HELLO'
    /// </code>
    /// </example>
    public static string UPPER(string src)
    {
        return $"UPPER({src})";
    }

    /// <summary>
    /// Converts <paramref name="src"/> to lowercase.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LOWER('Hello'); -&gt; 'hello'
    /// </code>
    /// </example>
    public static string LOWER(string src)
    {
        return $"LOWER({src})";
    }

    /// <summary>
    /// Synonym for <see cref="UPPER"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT UCASE('Hello'); -&gt; 'HELLO'
    /// </code>
    /// </example>
    public static string UCASE(string src)
    {
        return $"UCASE({src})";
    }

    /// <summary>
    /// Synonym for <see cref="LOWER"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LCASE('Hello'); -&gt; 'hello'
    /// </code>
    /// </example>
    public static string LCASE(string src)
    {
        return $"LCASE({src})";
    }

    /// <summary>
    /// Removes leading and trailing spaces from <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT TRIM('  Hello  '); -&gt; 'Hello'
    /// </code>
    /// </example>
    public static string TRIM(string src)
    {
        return $"TRIM({src})";
    }

    /// <summary>
    /// Removes leading spaces from <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LTRIM('  Hello'); -&gt; 'Hello'
    /// </code>
    /// </example>
    public static string LTRIM(string src)
    {
        return $"LTRIM({src})";
    }

    /// <summary>
    /// Removes trailing spaces from <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT RTRIM('Hello  '); -&gt; 'Hello'
    /// </code>
    /// </example>
    public static string RTRIM(string src)
    {
        return $"RTRIM({src})";
    }

    /// <summary>
    /// Concatenates all <paramref name="args"/> into a single string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. Returns NULL if any argument is NULL.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CONCAT('Hello', ' ', 'World'); -&gt; 'Hello World'
    /// </code>
    /// </example>
    public static string CONCAT(params string[] args)
    {
        return $"CONCAT({string.Join(", ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Concatenates all <paramref name="args"/> using <paramref name="separator"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1. Skips NULL values (unlike CONCAT).
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CONCAT_WS(', ', 'a', 'b', 'c'); -&gt; 'a, b, c'
    /// </code>
    /// </example>
    public static string CONCAT_WS(string separator, params string[] args)
    {
        return $"CONCAT_WS({separator}, {string.Join(", ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Replaces occurrences of <paramref name="from"/> in <paramref name="src"/> with <paramref name="to"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. Replaces all matching occurrences.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT REPLACE('Hello World', 'World', 'There'); -&gt; 'Hello There'
    /// </code>
    /// </example>
    public static string REPLACE(string src, string from, string to)
    {
        return $"REPLACE({src}, {from}, {to})";
    }

    /// <summary>
    /// Reverses the characters of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT REVERSE('Hello'); -&gt; 'olleH'
    /// </code>
    /// </example>
    public static string REVERSE(string src)
    {
        return $"REVERSE({src})";
    }

    /// <summary>
    /// Repeats <paramref name="src"/> <paramref name="count"/> times.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT REPEAT('ab', 3); -&gt; 'ababab'
    /// </code>
    /// </example>
    public static string REPEAT(string src, string count)
    {
        return $"REPEAT({src}, {count})";
    }

    /// <summary>
    /// Returns a string of <paramref name="count"/> space characters.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SPACE(5); -&gt; '     '
    /// </code>
    /// </example>
    public static string SPACE(string count)
    {
        return $"SPACE({count})";
    }

    /// <summary>
    /// Inserts <paramref name="newSubstr"/> into <paramref name="src"/> at <paramref name="position"/>, replacing <paramref name="length"/> characters.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. Returns the original string if <paramref name="position"/> is outside its length.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT INSERT('Hello World', 7, 5, 'There'); -&gt; 'Hello There'
    /// </code>
    /// </example>
    public static string INSERT(string src, string position, string length, string newSubstr)
    {
        return $"INSERT({src}, {position}, {length}, {newSubstr})";
    }

    /// <summary>
    /// Returns the position of the first occurrence of <paramref name="substr"/> in <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. Returns 0 if not found.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LOCATE('World', 'Hello World'); -&gt; 7
    /// </code>
    /// </example>
    public static string LOCATE(string substr, string src)
    {
        return $"LOCATE({substr}, {src})";
    }

    /// <summary>
    /// Synonym for <see cref="LOCATE"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT POSITION('World' IN 'Hello World'); -&gt; 7
    /// </code>
    /// </example>
    public static string POSITION(string substr, string src)
    {
        return $"POSITION({substr} IN {src})";
    }

    /// <summary>
    /// Returns the position of the first occurrence of <paramref name="substr"/> in <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT INSTR('Hello World', 'World'); -&gt; 7
    /// </code>
    /// </example>
    public static string INSTR(string src, string substr)
    {
        return $"INSTR({src}, {substr})";
    }

    /// <summary>
    /// Left-pads <paramref name="src"/> to <paramref name="length"/> characters with <paramref name="padStr"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT LPAD('hi', 5, '*'); -&gt; '***hi'
    /// </code>
    /// </example>
    public static string LPAD(string src, string length, string padStr)
    {
        return $"LPAD({src}, {length}, {padStr})";
    }

    /// <summary>
    /// Right-pads <paramref name="src"/> to <paramref name="length"/> characters with <paramref name="padStr"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT RPAD('hi', 5, '*'); -&gt; 'hi***'
    /// </code>
    /// </example>
    public static string RPAD(string src, string length, string padStr)
    {
        return $"RPAD({src}, {length}, {padStr})";
    }

    /// <summary>
    /// Compares two strings using their collation. Returns -1, 0, or 1.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT STRCMP('abc', 'abd'); -&gt; -1
    /// </code>
    /// </example>
    public static string STRCMP(string src1, string src2)
    {
        return $"STRCMP({src1}, {src2})";
    }

    /// <summary>
    /// Returns the 1-based index of <paramref name="src"/> in the <paramref name="values"/> list, or 0 if not found.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT FIELD('b', 'a', 'b', 'c'); -&gt; 2
    /// </code>
    /// </example>
    public static string FIELD(string src, params string[] values)
    {
        return $"FIELD({src}, {string.Join(", ", values.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Returns the 1-based position of <paramref name="src"/> in the comma-separated list <paramref name="strList"/>, or 0 if not found.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT FIND_IN_SET('b', 'a,b,c'); -&gt; 2
    /// </code>
    /// </example>
    public static string FIND_IN_SET(string src, string strList)
    {
        return $"FIND_IN_SET({src}, {strList})";
    }

    /// <summary>
    /// Returns the string at <paramref name="index"/> from <paramref name="strs"/> (1-based). Returns NULL if out of range.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ELT(2, 'a', 'b', 'c'); -&gt; 'b'
    /// </code>
    /// </example>
    public static string ELT(string index, params string[] strs)
    {
        return $"ELT({index}, {string.Join(", ", strs.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Returns a comma-separated string of the <paramref name="strs"/> corresponding to set bits in <paramref name="bits"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT MAKE_SET(5, 'a', 'b', 'c'); -&gt; 'a,c'
    /// </code>
    /// </example>
    public static string MAKE_SET(string bits, params string[] strs)
    {
        return $"MAKE_SET({bits}, {string.Join(", ", strs.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Returns a string of bits from <paramref name="bits"/>, where each bit is replaced by <paramref name="on"/> or <paramref name="off"/>, separated by <paramref name="separator"/>, and padded to <paramref name="numberOfBits"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT EXPORT_SET(5, 'Y', 'N', ',', 4); -&gt; 'Y,N,Y,N'
    /// </code>
    /// </example>
    public static string EXPORT_SET(string bits, string on, string off, string separator, string numberOfBits)
    {
        return $"EXPORT_SET({bits}, {on}, {off}, {separator}, {numberOfBits})";
    }

    /// <summary>
    /// Formats <paramref name="src"/> as a number with <paramref name="decimalPlaces"/> decimal places, using ',' as the thousands separator.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. Uses the default locale for separators.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT FORMAT(1234567.8912, 2); -&gt; '1,234,567.89'
    /// </code>
    /// </example>
    public static string FORMAT(string src, string decimalPlaces)
    {
        return $"FORMAT({src}, {decimalPlaces})";
    }

    /// <summary>
    /// Returns the hexadecimal representation of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT HEX(255); -&gt; 'FF'
    /// </code>
    /// </example>
    public static string HEX(string src)
    {
        return $"HEX({src})";
    }

    /// <summary>
    /// Returns the binary value decoded from the hexadecimal string <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1. Inverse of HEX().
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT UNHEX('48656C6C6F'); -&gt; 'Hello'
    /// </code>
    /// </example>
    public static string UNHEX(string src)
    {
        return $"UNHEX({src})";
    }

    /// <summary>
    /// Returns the numeric ASCII code of the leftmost character of <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ASCII('A'); -&gt; 65
    /// </code>
    /// </example>
    public static string ASCII(string src)
    {
        return $"ASCII({src})";
    }

    /// <summary>
    /// Returns the binary representation of <paramref name="src"/> as a string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. Equivalent to CONV(N, 10, 2).
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT BIN(5); -&gt; '101'
    /// </code>
    /// </example>
    public static string BIN(string src)
    {
        return $"BIN({src})";
    }

    /// <summary>
    /// Returns a soundex string from <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SOUNDEX('Robert'), SOUNDEX('Rupert'); -&gt; 'R163', 'R163'
    /// </code>
    /// </example>
    public static string SOUNDEX(string src)
    {
        return $"SOUNDEX({src})";
    }

    /// <summary>
    /// Decodes the base64-encoded string <paramref name="src"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.6 and MariaDB 10.0. Returns NULL if the input is not valid base64.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT FROM_BASE64('SGVsbG8='); -&gt; 'Hello'
    /// </code>
    /// </example>
    public static string FROM_BASE64(string src)
    {
        return $"FROM_BASE64({src})";
    }

    /// <summary>
    /// Encodes <paramref name="src"/> to a base64 string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.6 and MariaDB 10.0.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT TO_BASE64('Hello'); -&gt; 'SGVsbG8='
    /// </code>
    /// </example>
    public static string TO_BASE64(string src)
    {
        return $"TO_BASE64({src})";
    }

    /// <summary>
    /// Returns whether <paramref name="src"/> matches the regular expression <paramref name="pattern"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.4 and MariaDB 10.4.3.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT REGEXP_LIKE('Hello', '^[A-Z]'); -&gt; 1
    /// </code>
    /// </example>
    public static string REGEXP_LIKE(string src, string pattern)
    {
        return $"{src} REGEXP {pattern}";
    }

    /// <summary>
    /// Replaces substrings of <paramref name="src"/> that match <paramref name="pattern"/> with <paramref name="replacement"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.4 and MariaDB 10.4.3.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT REGEXP_REPLACE('a1b2c3', '[0-9]', '*'); -&gt; 'a*b*c*'
    /// </code>
    /// </example>
    public static string REGEXP_REPLACE(string src, string pattern, string replacement)
    {
        return $"REGEXP_REPLACE({src}, {pattern}, {replacement})";
    }

    /// <summary>
    /// Returns the starting index of the substring of <paramref name="src"/> that matches <paramref name="pattern"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.4 and MariaDB 10.4.3.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT REGEXP_INSTR('hello world', 'l+'); -&gt; 3
    /// </code>
    /// </example>
    public static string REGEXP_INSTR(string src, string pattern)
    {
        return $"REGEXP_INSTR({src}, {pattern})";
    }

    /// <summary>
    /// Returns the substring of <paramref name="src"/> that matches <paramref name="pattern"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.4 and MariaDB 10.4.3.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT REGEXP_SUBSTR('abc 123 def', '[0-9]+'); -&gt; '123'
    /// </code>
    /// </example>
    public static string REGEXP_SUBSTR(string src, string pattern)
    {
        return $"REGEXP_SUBSTR({src}, {pattern})";
    }

    /// <summary>
    /// Synonym for <see cref="SUBSTRING"/>. Returns the substring of <paramref name="src"/> starting at <paramref name="start"/> for <paramref name="length"/> characters.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT SUBSTR('Hello World', 7, 5); -&gt; 'World'
    /// </code>
    /// </example>
    public static string SUBSTR(string src, string start, string length)
    {
        return $"SUBSTR({src}, {start}, {length})";
    }

    /// <summary>
    /// Synonym for <see cref="SUBSTRING"/>. Returns a substring starting from the specified position.
    /// </summary>
    /// <remarks>
    /// MID was introduced as a synonym for SUBSTRING in MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT MID('Hello World', 7, 5); -&gt; 'World'
    /// </code>
    /// </example>
    public static string MID(string src, string start, string length)
    {
        return $"MID({src}, {start}, {length})";
    }

    /// <summary>
    /// Loads the named file <paramref name="fileName"/> and returns the file contents as a string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1. The user must have the FILE privilege and the file must be readable.
    /// </remarks>
    /// <example>
    /// <code>
    /// UPDATE t SET txt = LOAD_FILE('/tmp/note.txt') WHERE id = 1;
    /// </code>
    /// </example>
    public static string LOAD_FILE(string fileName)
    {
        return $"LOAD_FILE({fileName})";
    }

    /// <summary>
    /// Returns the character code for the leftmost character of <paramref name="src"/>, taking multi-byte characters into account.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.1 and MariaDB 5.1. For single-byte characters this is equivalent to ASCII().
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ORD('A'); -&gt; 65
    /// </code>
    /// </example>
    public static string ORD(string src)
    {
        return $"ORD({src})";
    }

    /// <summary>
    /// Returns a string containing the octal representation of <paramref name="N"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. Returns NULL for negative arguments.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT OCT(8); -&gt; '10'
    /// </code>
    /// </example>
    public static string OCT(string N)
    {
        return $"OCT({N})";
    }

    /// <summary>
    /// Escapes the argument <paramref name="src"/> for use in an SQL statement. The result is a properly quoted string that can be used as a literal value in SQL.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 4.0 and MariaDB 5.1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT QUOTE('Hello'); -&gt; 'Hello'
    /// </code>
    /// </example>
    public static string QUOTE(string src)
    {
        return $"QUOTE({src})";
    }

    /// <summary>
    /// Returns the weight string for <paramref name="src"/>, used internally for sorting and comparison.
    /// </summary>
    /// <remarks>
    /// WEIGHT_STRING was introduced in MySQL 5.6 and MariaDB 10.0. Mainly useful for debugging collation order.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT HEX(WEIGHT_STRING('Hello' COLLATE utf8mb4_0900_ai_ci));
    /// </code>
    /// </example>
    public static string WEIGHT_STRING(string src)
    {
        return $"WEIGHT_STRING({src})";
    }

    /// <summary>
    /// Tests whether two strings sound the same when spoken (uses SOUNDEX internally).
    /// </summary>
    /// <remarks>
    /// SOUNDS LIKE was introduced in MySQL 4.0 and MariaDB 5.1. Not a function — emits the SOUNDS LIKE SQL operator.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT 'Robert' SOUNDS LIKE 'Rupert'; -&gt; 1
    /// </code>
    /// </example>
    public static string SOUNDS_LIKE(string src1, string src2)
    {
        return $"{src1} SOUNDS LIKE {src2}";
    }

    /// <summary>
    /// Interprets each integer <paramref name="args"/> as a character code (using the connection charset) and concatenates them into a string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 3.23 and MariaDB 5.1. Values greater than 255 are converted to multiple bytes.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT CHAR(72, 101, 108, 108, 111); -&gt; 'Hello'
    /// </code>
    /// </example>
    public static string CHAR(params string[] args)
    {
        return $"CHAR({string.Join(", ", args.Select(x => x.ToString()))})";
    }
}