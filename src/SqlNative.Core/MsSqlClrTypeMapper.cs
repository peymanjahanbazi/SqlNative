using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SqlNative.Core;

public static class MsSqlClrTypeMapper
{
    private static readonly Regex MsSqlTypeRegex = new(
        @"^\s*([A-Za-z][A-Za-z0-9_]*)\s*(?:\(\s*(\d+(?:\s*,\s*\d+)?)\s*\))?\s*(max)?\s*$",
        RegexOptions.IgnoreCase);

    private static readonly HashSet<string> BinaryTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "binary", "varbinary", "timestamp", "rowversion",
    };

    private static readonly HashSet<string> TextTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "char", "varchar", "text", "nchar", "nvarchar", "ntext",
        "uniqueidentifier",
        "xml",
    };

    private static readonly HashSet<string> ValueTypes = new(StringComparer.Ordinal)
    {
        "bool", "char", "sbyte", "byte", "short", "ushort", "int", "uint",
        "long", "ulong", "float", "double", "decimal", "DateTime", "TimeSpan",
    };

    public static string MapToClrType(string columnRawType)
    {
        var match = MsSqlTypeRegex.Match(columnRawType ?? string.Empty);
        if (!match.Success)
        {
            return "string";
        }

        var baseType = match.Groups[1].Value.ToLowerInvariant();

        return baseType switch
        {
            "bit" => "bool",
            "tinyint" => "byte",
            "smallint" => "short",
            "int" or "integer" => "int",
            "bigint" => "long",
            "real" => "float",
            "float" => "double",
            "decimal" or "numeric" => "decimal",
            "money" or "smallmoney" => "decimal",
            "date" or "datetime" or "datetime2" or "smalldatetime" => "DateTime",
            "datetimeoffset" => "DateTime",
            "time" => "TimeSpan",
            "uniqueidentifier" => "Guid",
            _ when BinaryTypes.Contains(baseType) => "byte[]",
            _ when TextTypes.Contains(baseType) => "string",
            _ => "string",
        };
    }

    public static string ApplyNullability(string clrType, bool isNullable)
        => isNullable && (ValueTypes.Contains(clrType) || IsReferenceType(clrType)) ? clrType + "?" : clrType;

    public static bool IsReferenceType(string clrType)
        => clrType is "string" or "byte[]" or "object";

    public static string GetReaderExpression(string clrType, string ordinal)
    {
        var type = clrType.TrimEnd('?');
        return type switch
        {
            "string" => $"reader.GetString({ordinal})",
            "bool" => $"reader.GetBoolean({ordinal})",
            "byte" => $"reader.GetByte({ordinal})",
            "short" => $"reader.GetInt16({ordinal})",
            "int" => $"reader.GetInt32({ordinal})",
            "long" => $"reader.GetInt64({ordinal})",
            "float" => $"reader.GetFloat({ordinal})",
            "double" => $"reader.GetDouble({ordinal})",
            "decimal" => $"reader.GetDecimal({ordinal})",
            "DateTime" => $"reader.GetDateTime({ordinal})",
            "Guid" => $"reader.GetGuid({ordinal})",
            "byte[]" => $"(byte[])reader.GetValue({ordinal})",
            _ => $"({type})reader.GetValue({ordinal})",
        };
    }
}