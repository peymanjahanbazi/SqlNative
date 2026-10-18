using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlNative.Core;

public static class OracleIdentifierHelper
{
    private static readonly Regex _nonAlphanumericRegex = new Regex(@"[^A-Za-z0-9_]+");

    // Oracle reserved words (Oracle Database 19c/21c Reserved and Keywords).
    // These are commonly used identifiers in Oracle that require quoting.
    private static readonly HashSet<string> _oracleReservedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "access", "add", "all", "alter", "and", "any", "array", "as", "asc", "audit",
        "between", "by", "check", "cluster", "column", "comment", "compress", "connect", "create",
        "current", "date", "decimal", "default", "delete", "desc", "distinct", "drop", "else",
        "exclusive", "exists", "file", "float", "for", "from", "grant", "group", "having", "identified",
        "immediate", "in", "initial", "insert", "integer", "intersect", "into", "is", "level", "like",
        "lock", "long", "maxextents", "merge", "mlslabel", "mode", "modify", "nested_table_id", "noaudit",
        "nocompress", "not", "nowait", "null", "number", "of", "offline", "option", "or", "order",
        "pctfree", "pctused", "prior", "privileges", "public", "raw", "read", "rename", "resource",
        "return", "revoke", "row", "select", "session", "set", "share", "size", "smallint", "start",
        "successful", "synonym", "sysdate", "table", "then", "to", "trigger", "uid", "union", "unique",
        "update", "user", "validate", "values", "view", "whenever", "where", "with",
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

        if (NeedsDoubleQuotes(objectName))
        {
            return "\"" + objectName.Replace("\"", "\"\"") + "\"";
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

        var builder = new StringBuilder(value.Length);
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

    public static bool NeedsDoubleQuotes(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            return true;
        }

        char first = identifier[0];
        if (!char.IsLetter(first) && first != '_')
        {
            return true;
        }

        foreach (char c in identifier)
        {
            if (!char.IsLetterOrDigit(c) && c != '_' && c != '$' && c != '#')
            {
                return true;
            }
        }

        if (_oracleReservedWords.Contains(identifier))
        {
            return true;
        }

        return false;
    }
}