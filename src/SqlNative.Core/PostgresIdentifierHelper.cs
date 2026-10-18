using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlNative.Core;

public static class PostgresIdentifierHelper
{
    private static readonly Regex _nonAlphanumericRegex = new Regex(@"[^A-Za-z0-9]+");

    private static readonly HashSet<string> _postgresReservedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "all", "analyse", "analyze", "and", "any", "array", "as", "asc", "asymmetric",
        "both", "case", "cast", "check", "collate", "column", "constraint", "create",
        "current_catalog", "current_date", "current_role", "current_time", "current_timestamp",
        "current_user", "default", "deferrable", "desc", "distinct", "do", "else", "end",
        "except", "false", "fetch", "for", "foreign", "from", "grant", "group", "having",
        "in", "initially", "intersect", "into", "lateral", "leading", "limit", "localtime",
        "localtimestamp", "not", "null", "offset", "on", "only", "or", "order", "placing",
        "primary", "references", "returning", "select", "session_user", "some", "symmetric",
        "table", "then", "to", "trailing", "true", "union", "unique", "user", "using",
        "variadic", "when", "where", "window", "with",
        "bigint", "bigserial", "bit", "boolean", "box", "bytea", "char", "character", "cidr",
        "circle", "date", "decimal", "double", "float", "inet", "integer", "interval", "json",
        "jsonb", "line", "lseg", "macaddr", "macaddr8", "money", "name", "numeric", "path",
        "pg_lsn", "point", "polygon", "real", "serial", "smallint", "serial4", "serial8",
        "smallserial", "text", "time", "timestamp", "timestamptz", "timetz", "tsquery",
        "tsvector", "txid_snapshot", "uuid", "xml", "varchar", "xmltype",
        "authorization", "between", "cross", "full", "inner", "join", "left", "natural",
        "outer", "right", "exists",
        "ilike", "like", "similar", "is", "overlap", "overlaps",
        "add", "alter", "always", "by", "cascade", "comment", "convert", "copy", "database",
        "deferred", "delete", "drop", "encoding", "extension", "family", "first", "generated",
        "if", "immutable", "index", "insert", "listen", "load", "match", "moves", "no",
        "notify", "owner", "parallel", "password", "procedure", "publication", "rename",
        "replace", "reset", "restrict", "rule", "schema", "security", "sequence", "set",
        "stable", "statistics", "storage", "trigger", "type", "unlogged", "update", "validate",
        "volatile",
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
            return true;

        char first = identifier[0];
        if (!char.IsLetter(first) && first != '_')
            return true;

        foreach (char c in identifier)
        {
            if (!char.IsLetterOrDigit(c) && c != '_')
                return true;
        }

        if (_postgresReservedWords.Contains(identifier))
            return true;

        return false;
    }
}