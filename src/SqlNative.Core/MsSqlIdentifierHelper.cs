using System.Text;

namespace SqlNative.Core;

internal static class MsSqlIdentifierHelper
{
    private static readonly HashSet<string> _tsqlReservedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "add", "all", "alter", "and", "any", "as", "asc", "authorization", "backup",
        "begin", "between", "break", "browse", "bulk", "by", "cascade", "case", "check",
        "checkpoint", "close", "clustered", "coalesce", "collate", "column", "commit",
        "compute", "constraint", "contains", "containstable", "convert", "create", "cross",
        "current", "current_date", "current_time", "current_timestamp", "current_user",
        "cursor", "database", "dbcc", "deallocate", "declare", "default", "delete", "deny",
        "desc", "distinct", "distributed", "double", "drop", "dump", "else", "end",
        "errlvl", "escape", "except", "exec", "execute", "exists", "exit", "external",
        "fetch", "file", "fillfactor", "for", "foreign", "freetext", "freetexttable", "from",
        "full", "function", "goto", "grant", "group", "having", "holdlock", "identity",
        "identity_insert", "identitycol", "if", "in", "index", "inner", "insert",
        "intersect", "into", "is", "join", "key", "kill", "left", "like", "lineno",
        "load", "merge", "national", "nocheck", "nonclustered", "not", "null", "nullif",
        "of", "off", "offsets", "on", "open", "opendatasource", "openquery", "openrowset",
        "openxml", "option", "or", "order", "outer", "over", "percent", "pivot", "plan",
        "precision", "primary", "print", "proc", "procedure", "public", "raiserror",
        "read", "readtext", "reconfigure", "references", "replication", "restore",
        "restrict", "return", "revert", "revoke", "right", "rollback", "rowcount",
        "rowguidcol", "rule", "save", "schema", "securityaudit", "select", "semantickeyphrasetable",
        "semanticsimilaritydetailstable", "semanticsimilaritytable", "session_user", "set",
        "setuser", "shutdown", "some", "statistics", "system_user", "table", "tablesample",
        "textsize", "then", "to", "top", "tran", "transaction", "trigger", "truncate",
        "try_convert", "tsequal", "union", "unique", "unpivot", "update", "updatetext",
        "use", "user", "values", "varying", "view", "waitfor", "when", "where", "while",
        "with", "within", "writetext",
    };

    public static string GetObjectNameInCs(string originalname)
    {
        var name = IdentifierNameConverter.ToPascalCase(originalname);

        if (char.IsDigit(name[0]) || name[0] == '_')
        {
            name = "_" + name;
        }

        return name;
    }

    public static string WrapObjectName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            return objectName;
        }

        if (NeedsBrackets(objectName))
        {
            return "[" + objectName.Replace("]", "]]") + "]";
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

    public static bool NeedsBrackets(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            return true;
        }

        char first = identifier[0];
        if (!char.IsLetter(first) && first != '_' && first != '@')
        {
            return true;
        }

        foreach (char c in identifier)
        {
            if (!char.IsLetterOrDigit(c) && c != '_' && c != '@' && c != '$' && c != '#')
            {
                return true;
            }
        }

        if (_tsqlReservedWords.Contains(identifier))
        {
            return true;
        }

        return false;
    }
}