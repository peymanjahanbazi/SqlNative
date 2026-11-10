using System.Text.RegularExpressions;

namespace SqlNative.Core;

internal static partial class ClrTypeMapper
{
    private static readonly Regex MySqlTypeRegex = new(
        @"^\s*([a-z]+)\s*(?:\(\s*(\d+)\s*(?:,\s*\d+\s*)?\))?\s*(unsigned)?\s*$",
        RegexOptions.IgnoreCase);

    private static readonly HashSet<string> BinaryTypes = new(StringComparer.Ordinal)
    {
        "binary", "varbinary", "tinyblob", "blob", "mediumblob", "longblob",
    };

    private static readonly HashSet<string> TextTypes = new(StringComparer.Ordinal)
    {
        "char", "varchar", "tinytext", "text", "mediumtext", "longtext",
        "enum", "set", "json", "geometry", "point", "linestring", "polygon",
        "multipoint", "multilinestring", "multipolygon", "geomcollection", "geometrycollection",
    };

    private static readonly HashSet<string> ValueTypes = new(StringComparer.Ordinal)
    {
        "bool", "char", "sbyte", "byte", "short", "ushort", "int", "uint",
        "long", "ulong", "float", "double", "decimal", "DateTime", "TimeSpan",
    };

    public static string MapToClrType(string columnRawType)
    {
        var match = MySqlTypeRegex.Match(columnRawType);
        if (!match.Success)
        {
            return "string";
        }

        var baseType = match.Groups[1].Value.ToLowerInvariant();
        var width = match.Groups[2].Success ? match.Groups[2].Value : null;
        var isUnsigned = match.Groups[3].Success;

        return baseType switch
        {
            "tinyint" => width == "1" ? "bool" : isUnsigned ? "byte" : "sbyte",
            "smallint" => isUnsigned ? "ushort" : "short",
            "mediumint" or "int" or "integer" => isUnsigned ? "uint" : "int",
            "bigint" => isUnsigned ? "ulong" : "long",
            "float" or "real" => "float",
            "double" or "precision" => "double",
            "decimal" or "numeric" or "dec" or "fixed" => "decimal",
            "bit" => width == "1" ? "bool" : "ulong",
            "date" or "datetime" or "timestamp" => "DateTime",
            "time" => "TimeSpan",
            "year" => "int",
            _ when TextTypes.Contains(baseType) => "string",
            _ when BinaryTypes.Contains(baseType) => "byte[]",
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