namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Returns data from a JSON document located at the given <paramref name="path"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.3.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_EXTRACT('{"name": "Alice"}', '$.name'); -&gt; '"Alice"'
    /// </code>
    /// </example>
    public static string JSON_EXTRACT(string json, string path)
    {
        return $"JSON_EXTRACT({json}, {path})";
    }

    /// <summary>
    /// Returns a JSON value extracted from a JSON document at the given <paramref name="path"/>, with the given SQL <paramref name="TYPE"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.21 and MariaDB 10.6.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_VALUE('{"price": 9.95}', '$.price' RETURNING DECIMAL(10,2)); -&gt; 9.95
    /// </code>
    /// </example>
    public static string JSON_VALUE(string json, string path, string TYPE)
    {
        return $"JSON_VALUE({json}, {path} {TYPE})";
    }

    /// <summary>
    /// Returns a JSON value extracted from a JSON document at the given <paramref name="path"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.21 and MariaDB 10.6.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_VALUE('{"price": 9.95}', '$.price'); -&gt; 9.95
    /// </code>
    /// </example>
    public static string JSON_VALUE(string json, string path)
    {
        return $"JSON_VALUE({json}, {path})";
    }

    /// <summary>
    /// Builds a JSON object from key/value pairs.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.22 and MariaDB 10.2.4. Argument list must alternate between key and value.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_OBJECT('name', 'Alice', 'age', 30); -&gt; '{"name": "Alice", "age": 30}'
    /// </code>
    /// </example>
    public static string JSON_OBJECT(params string[] args)
    {
        return $"JSON_OBJECT({string.Join(", ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Creates a JSON array from a list of values.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.22 and MariaDB 10.2.4.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_ARRAY(1, 'two', 3.0); -&gt; '[1, "two", 3.0]'
    /// </code>
    /// </example>
    public static string JSON_ARRAY(params string[] args)
    {
        return $"JSON_ARRAY({string.Join(", ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Returns 1 if the JSON document <paramref name="target"/> contains the <paramref name="candidate"/>, 0 otherwise. <paramref name="path"/> is optional.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.3.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_CONTAINS('{"a": 1, "b": 2}', '1', '$.a'); -&gt; 1
    /// </code>
    /// </example>
    public static string JSON_CONTAINS(string target, string candidate, string path)
    {
        return $"JSON_CONTAINS({target}, {candidate}, {path})";
    }

    /// <summary>
    /// Returns 1 if any of the JSON <paramref name="paths"/> exist within the document, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.3. <paramref name="oneOrAll"/> must be 'one' or 'all'.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_CONTAINS_PATH('{"a": 1, "b": 2}', 'one', '$.a', '$.c'); -&gt; 1
    /// </code>
    /// </example>
    public static string JSON_CONTAINS_PATH(string json, string oneOrAll, params string[] paths)
    {
        return $"JSON_CONTAINS_PATH({json}, {oneOrAll}, {string.Join(", ", paths.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Returns the maximum depth of the JSON document.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.3.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_DEPTH('{"a": {"b": 1}}'); -&gt; 2
    /// </code>
    /// </example>
    public static string JSON_DEPTH(string json)
    {
        return $"JSON_DEPTH({json})";
    }

    /// <summary>
    /// Returns the number of elements in the JSON document.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.3. For scalars returns 1.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_LENGTH('[1, 2, 3]'); -&gt; 3
    /// </code>
    /// </example>
    public static string JSON_LENGTH(string json)
    {
        return $"JSON_LENGTH({json})";
    }

    /// <summary>
    /// Returns the number of elements at the specified <paramref name="path"/> in the JSON document.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.3.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_LENGTH('{"a": [1, 2, 3]}', '$.a'); -&gt; 3
    /// </code>
    /// </example>
    public static string JSON_LENGTH(string json, string path)
    {
        return $"JSON_LENGTH({json}, {path})";
    }

    /// <summary>
    /// Returns the top-level keys of the JSON object.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.3. Returns NULL if the document is not an object.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_KEYS('{"a": 1, "b": 2}'); -&gt; '["a", "b"]'
    /// </code>
    /// </example>
    public static string JSON_KEYS(string json)
    {
        return $"JSON_KEYS({json})";
    }

    /// <summary>
    /// Returns the keys at the given <paramref name="path"/> within the JSON object.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.3.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_KEYS('{"a": {"x": 1, "y": 2}}', '$.a'); -&gt; '["x", "y"]'
    /// </code>
    /// </example>
    public static string JSON_KEYS(string json, string path)
    {
        return $"JSON_KEYS({json}, {path})";
    }

    /// <summary>
    /// Merges two or more JSON documents, preserving duplicate keys (later values become array elements).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.22 and MariaDB 10.2.4. Deprecated in MySQL 8.0.16 in favor of <see cref="JSON_MERGE_PATCH"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_MERGE_PRESERVE('{"a": 1}', '{"b": 2}'); -&gt; '{"a": 1, "b": 2}'
    /// </code>
    /// </example>
    public static string JSON_MERGE_PRESERVE(params string[] args)
    {
        return $"JSON_MERGE_PRESERVE({string.Join(", ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Merges two or more JSON documents, replacing duplicate key values with the value from the later document.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.3 and MariaDB 10.2.4.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_MERGE_PATCH('{"a": 1}', '{"a": 2}'); -&gt; '{"a": 2}'
    /// </code>
    /// </example>
    public static string JSON_MERGE_PATCH(params string[] args)
    {
        return $"JSON_MERGE_PATCH({string.Join(", ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Returns 1 if the two JSON documents share any key/value pairs, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.17 and MariaDB 10.9.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_OVERLAPS('[1, 2, 3]', '[3, 4]'); -&gt; 1
    /// </code>
    /// </example>
    public static string JSON_OVERLAPS(string json1, string json2)
    {
        return $"JSON_OVERLAPS({json1}, {json2})";
    }

    /// <summary>
    /// Returns a JSON document in pretty-printed format.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.22 and MariaDB 10.2.4.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_PRETTY('{"a":1,"b":2}');
    /// </code>
    /// </example>
    public static string JSON_PRETTY(string json)
    {
        return $"JSON_PRETTY({json})";
    }

    /// <summary>
    /// Quotes a string as a JSON value.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.3.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_QUOTE('hello'); -&gt; '"hello"'
    /// </code>
    /// </example>
    public static string JSON_QUOTE(string src)
    {
        return $"JSON_QUOTE({src})";
    }

    /// <summary>
    /// Removes data from a JSON document at the specified <paramref name="paths"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.13 and MariaDB 10.2.4.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_REMOVE('{"a": 1, "b": 2}', '$.b'); -&gt; '{"a": 1}'
    /// </code>
    /// </example>
    public static string JSON_REMOVE(string json, params string[] paths)
    {
        return $"JSON_REMOVE({json}, {string.Join(", ", paths.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Replaces existing values in a JSON document at the specified paths.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.13 and MariaDB 10.2.4. Does not add keys that do not exist.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_REPLACE('{"a": 1, "b": 2}', '$.a', 9); -&gt; '{"a": 9, "b": 2}'
    /// </code>
    /// </example>
    public static string JSON_REPLACE(string json, params string[] pathValuePairs)
    {
        return $"JSON_REPLACE({json}, {string.Join(", ", pathValuePairs.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Returns the path to the given <paramref name="searchStr"/> within the JSON document.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.3. <paramref name="oneOrAll"/> must be 'one' or 'all'.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_SEARCH('{"a": "hello", "b": "world"}', 'one', 'world'); -&gt; '"$.b"'
    /// </code>
    /// </example>
    public static string JSON_SEARCH(string json, string oneOrAll, string searchStr)
    {
        return $"JSON_SEARCH({json}, {oneOrAll}, {searchStr})";
    }

    /// <summary>
    /// Inserts or updates data in a JSON document at the specified paths.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.4. Adds keys that do not exist and overwrites existing ones.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_SET('{"a": 1}', '$.a', 9, '$.b', 2); -&gt; '{"a": 9, "b": 2}'
    /// </code>
    /// </example>
    public static string JSON_SET(string json, params string[] pathValuePairs)
    {
        return $"JSON_SET({json}, {string.Join(", ", pathValuePairs.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Returns the storage size in bytes for the binary representation of a JSON document.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.22 and MariaDB 10.2.4.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_STORAGE_SIZE('{"a": 1}'); -&gt; 19
    /// </code>
    /// </example>
    public static string JSON_STORAGE_SIZE(string json)
    {
        return $"JSON_STORAGE_SIZE({json})";
    }

    /// <summary>
    /// Returns the type of a JSON value as a string (object, array, string, number, boolean, null).
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.3.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_TYPE('{"a": 1}'), JSON_TYPE('hello'); -&gt; 'OBJECT', 'STRING'
    /// </code>
    /// </example>
    public static string JSON_TYPE(string json)
    {
        return $"JSON_TYPE({json})";
    }

    /// <summary>
    /// Unquotes a JSON value, returning the value as a string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.3.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_UNQUOTE('"hello"'); -&gt; 'hello'
    /// </code>
    /// </example>
    public static string JSON_UNQUOTE(string json)
    {
        return $"JSON_UNQUOTE({json})";
    }

    /// <summary>
    /// Returns 1 if the JSON document is valid, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.4.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_VALID('{}'), JSON_VALID('not json'); -&gt; 1, 0
    /// </code>
    /// </example>
    public static string JSON_VALID(string json)
    {
        return $"JSON_VALID({json})";
    }

    /// <summary>
    /// Appends values to the end of the arrays designated by paths.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.13 and MariaDB 10.2.4.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_ARRAY_APPEND('{"a": [1, 2]}', '$.a', 3); -&gt; '{"a": [1, 2, 3]}'
    /// </code>
    /// </example>
    public static string JSON_ARRAY_APPEND(string json, params string[] pathValuePairs)
    {
        return $"JSON_ARRAY_APPEND({json}, {string.Join(", ", pathValuePairs.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Inserts values into arrays at the specified paths.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.17 and MariaDB 10.2.4. Negative path elements count from the end.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_ARRAY_INSERT('{"a": [1, 2]}', '$.a[0]', 0); -&gt; '{"a": [0, 1, 2]}'
    /// </code>
    /// </example>
    public static string JSON_ARRAY_INSERT(string json, params string[] pathValuePairs)
    {
        return $"JSON_ARRAY_INSERT({json}, {string.Join(", ", pathValuePairs.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Inserts data into a JSON document at the specified paths without overwriting existing values.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.9 and MariaDB 10.2.4. Adds only keys that do not exist.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_INSERT('{"a": 1}', '$.a', 9, '$.b', 2); -&gt; '{"a": 1, "b": 2}'
    /// </code>
    /// </example>
    public static string JSON_INSERT(string json, params string[] pathValuePairs)
    {
        return $"JSON_INSERT({json}, {string.Join(", ", pathValuePairs.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Deprecated synonym for <see cref="JSON_MERGE_PRESERVE"/>. Merges two or more JSON documents, preserving duplicate keys.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.22 and MariaDB 10.2.4. Deprecated in MySQL 8.0.16 in favor of <see cref="JSON_MERGE_PRESERVE"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_MERGE('{"a": 1}', '{"b": 2}'); -&gt; '{"a": 1, "b": 2}'
    /// </code>
    /// </example>
    [Obsolete("JSON_MERGE is deprecated in MySQL 8.0.16. Use JSON_MERGE_PRESERVE() instead.")]
    public static string JSON_MERGE(params string[] args)
    {
        return $"JSON_MERGE({string.Join(", ", args.Select(x => x.ToString()))})";
    }

    /// <summary>
    /// Validates a JSON document against a JSON schema. Returns TRUE (1) if it validates, FALSE (0) otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.17.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_SCHEMA_VALID('{"type": "object"}', '{}'); -&gt; 1
    /// </code>
    /// </example>
    public static string JSON_SCHEMA_VALID(string schema, string document)
    {
        return $"JSON_SCHEMA_VALID({schema}, {document})";
    }

    /// <summary>
    /// Validates a JSON document against a JSON schema and returns a JSON-formatted validation report.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.17.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_VALIDATION_REPORT(); -&gt; returns a JSON document describing any errors
    /// </code>
    /// </example>
    public static string JSON_SCHEMA_VALIDATION_REPORT(string schema, string document)
    {
        return $"JSON_SCHEMA_VALIDATION_REPORT({schema}, {document})";
    }

    /// <summary>
    /// Returns the freed space within the binary representation of a JSON column value following a partial update.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT JSON_STORAGE_FREE(JSON_SET(j, '$.a', 0)) FROM t;
    /// </code>
    /// </example>
    public static string JSON_STORAGE_FREE(string json)
    {
        return $"JSON_STORAGE_FREE({json})";
    }

    /// <summary>
    /// Returns data from a JSON expression as a relational table.
    /// </summary>
    /// <remarks>
    /// JSON_TABLE was introduced in MySQL 8.0 and MariaDB 10.6.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT * FROM JSON_TABLE('[{"a":1},{"a":2}]', '$[*]' COLUMNS (a INT PATH '$.a')) AS jt;
    /// </code>
    /// </example>
    public static string JSON_TABLE(string json, string path, string columns)
    {
        return $"JSON_TABLE({json}, {path} COLUMNS({columns}))";
    }

    /// <summary>
    /// Returns true (1) if the first operand matches any element of the JSON array passed as the second operand, otherwise returns false (0).
    /// </summary>
    /// <remarks>
    /// MEMBER OF() is a JSON operator introduced in MySQL 8.0.17. Not a function — emits the MEMBER OF SQL operator.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT 2 MEMBER OF ('[1, 2, 3]'); -&gt; 1
    /// </code>
    /// </example>
    public static string MEMBER_OF(string operand, string jsonArray)
    {
        return $"{operand} MEMBER OF ({jsonArray})";
    }
}