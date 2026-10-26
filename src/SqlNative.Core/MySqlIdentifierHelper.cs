using System.Text;
using System.Text.RegularExpressions;

namespace SqlNative.Core;

internal static class MySqlIdentifierHelper
{
    private static readonly Regex _nonAlphanumericRegex = new Regex(@"[^A-Za-z0-9]+");

    private static readonly HashSet<string> _mysqlReservedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "accessible", "add", "all", "alter", "analyze", "and", "as", "asc", "asensitive",
        "before", "between", "bigint", "binary", "blob", "both", "by", "call", "cascade",
        "case", "change", "char", "character", "check", "collate", "column", "condition",
        "constraint", "continue", "convert", "create", "cross", "current_date", "current_time",
        "current_timestamp", "current_user", "cursor", "database", "databases", "day_hour",
        "day_microsecond", "day_minute", "day_second", "dec", "decimal", "declare", "default",
        "delayed", "delete", "desc", "describe", "deterministic", "distinct", "distinctrow",
        "div", "double", "drop", "dual", "each", "else", "elseif", "enclosed", "escaped",
        "exists", "exit", "explain", "false", "fetch", "float", "float4", "float8", "for",
        "force", "foreign", "from", "fulltext", "generated", "get", "grant", "group", "having",
        "high_priority", "hour_microsecond", "hour_minute", "hour_second", "if", "ignore",
        "in", "index", "infile", "inner", "inout", "insensitive", "insert", "int", "int1",
        "int2", "int3", "int4", "int8", "integer", "interval", "into", "io_after_gtids",
        "io_before_gtids", "is", "iterate", "join", "key", "keys", "kill", "leading", "leave",
        "left", "like", "limit", "linear", "lines", "load", "localtime", "localtimestamp",
        "lock", "long", "longblob", "longtext", "loop", "low_priority", "master_bind",
        "master_ssl_verify_server_cert", "match", "maxvalue", "mediumblob", "mediumint",
        "mediumtext", "middleint", "minute_microsecond", "minute_second", "mod", "modifies",
        "natural", "not", "no_write_to_binlog", "null", "numeric", "on", "optimize",
        "optimizer_costs", "option", "optionally", "or", "order", "out", "outer", "outfile",
        "partition", "precision", "primary", "procedure", "purge", "range", "read", "reads",
        "read_write", "real", "references", "regexp", "release", "rename", "repeat", "replace",
        "require", "resignal", "restrict", "return", "revoke", "right", "rlike", "schema",
        "schemas", "second_microsecond", "select", "sensitive", "separator", "set", "show",
        "signal", "smallint", "spatial", "specific", "sql", "sqlexception", "sqlstate",
        "sqlwarning", "sql_big_result", "sql_calc_found_rows", "sql_small_result", "ssl",
        "starting", "stored", "straight_join", "table", "terminated", "then", "tinyblob",
        "tinyint", "tinytext", "to", "trailing", "trigger", "true", "undo", "union", "unique",
        "unlock", "unsigned", "update", "usage", "use", "using", "utc_date", "utc_time",
        "utc_timestamp", "values", "varbinary", "varchar", "varcharacter", "varying", "virtual",
        "when", "where", "while", "with", "write", "xor", "year_month", "zerofill"
    };

    public static string GetObjectNameInCs(string originalname)
    {
        var parts = _nonAlphanumericRegex.Split(originalname.Trim());
        var builder = new StringBuilder();

        foreach (var part in parts)
        {
            if (part.Length == 0)
            {
                continue;
            }

            builder.Append(char.ToUpperInvariant(part[0]));
            if (part.Length > 1)
            {
                builder.Append(part.Substring(1).ToLowerInvariant());
            }
        }

        if (builder.Length == 0)
        {
            return "Item";
        }

        if (char.IsDigit(builder[0]))
        {
            builder.Insert(0, '_');
        }

        return builder.ToString();
    }

    public static string WrapObjectName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            return objectName;
        }

        if (_mysqlReservedWords.Contains(objectName))
        {
            return "`" + objectName + "`";
        }

        for (int i = 0; i < objectName.Length; i++)
        {
            char c = objectName[i];
            if (!(char.IsLetterOrDigit(c) || c == '_'))
            {
                return "`" + objectName + "`";
            }
        }

        if (char.IsDigit(objectName[0]))
        {
            return "`" + objectName + "`";
        }

        return objectName;
    }

    public static string EnsureUnique(string candidate, ISet<string> usedNames)
    {
        if (usedNames.Add(candidate))
        {
            return candidate;
        }

        var suffix = 2;
        while (!usedNames.Add($"{candidate}{suffix}"))
        {
            suffix++;
        }

        return $"{candidate}{suffix}";
    }

    public static string EscapeCsStringLiteral(string value)
    {
        if (value == null)
        {
            return "null";
        }

        var builder = new StringBuilder(value.Length + 2);
        builder.Append('"');
        foreach (var c in value)
        {
            switch (c)
            {
                case '\\': builder.Append("\\\\"); break;
                case '"': builder.Append("\\\""); break;
                case '\n': builder.Append("\\n"); break;
                case '\r': builder.Append("\\r"); break;
                case '\t': builder.Append("\\t"); break;
                default:
                    if (char.IsControl(c))
                        builder.Append("\\u").Append(((int)c).ToString("x4"));
                    else
                        builder.Append(c);
                    break;
            }
        }
        builder.Append('"');
        return builder.ToString();
    }

    public static string EscapeCsStringLiteralNoQuote(string value)
    {
        if (value == null)
        {
            return null;
        }

        var builder = new StringBuilder(value.Length + 2);
        foreach (var c in value)
        {
            switch (c)
            {
                case '\\': builder.Append("\\\\"); break;
                case '"': builder.Append("\\\""); break;
                case '\n': builder.Append("\\n"); break;
                case '\r': builder.Append("\\r"); break;
                case '\t': builder.Append("\\t"); break;
                default:
                    if (char.IsControl(c))
                        builder.Append("\\u").Append(((int)c).ToString("x4"));
                    else
                        builder.Append(c);
                    break;
            }
        }
        return builder.ToString();
    }

    public static bool NeedsBackticks(string identifier)
    {
        // Defensive: null or empty identifiers cannot be used unquoted.
        if (string.IsNullOrEmpty(identifier))
            return true;

        // Rule 1: First character must be a letter or underscore.
        char first = identifier[0];
        if (!char.IsLetter(first) && first != '_')
            return true;

        // Rule 2: All characters must be letters, digits, underscore, or dollar sign.
        // Dollar sign ($) is allowed in unquoted MySQL identifiers.
        foreach (char c in identifier)
        {
            if (!char.IsLetterOrDigit(c) && c != '_' && c != '$')
                return true;
        }

        // Rule 3: Must not be a reserved keyword (case-insensitive).
        if (_mysqlReservedWords.Contains(identifier))
            return true;

        // All rules passed - safe to use without backticks.
        return false;
    }
}