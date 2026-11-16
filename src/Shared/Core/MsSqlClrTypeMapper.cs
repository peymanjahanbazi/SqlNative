using System.Text.RegularExpressions;

namespace SqlNative.Core;

internal static class MsSqlClrTypeMapper
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
            "bool" => $"Convert.ToBoolean(reader.GetValue({ordinal}))",
            "byte" => $"Convert.ToByte(reader.GetValue({ordinal}))",
            "sbyte" => $"Convert.ToSByte(reader.GetValue({ordinal}))",
            "short" => $"Convert.ToInt16(reader.GetValue({ordinal}))",
            "ushort" => $"Convert.ToUInt16(reader.GetValue({ordinal}))",
            "int" => $"Convert.ToInt32(reader.GetValue({ordinal}))",
            "uint" => $"Convert.ToUInt32(reader.GetValue({ordinal}))",
            "long" => $"Convert.ToInt64(reader.GetValue({ordinal}))",
            "ulong" => $"Convert.ToUInt64(reader.GetValue({ordinal}))",
            "float" => $"Convert.ToSingle(reader.GetValue({ordinal}))",
            "double" => $"Convert.ToDouble(reader.GetValue({ordinal}))",
            "decimal" => $"Convert.ToDecimal(reader.GetValue({ordinal}))",
            "DateTime" => $"Convert.ToDateTime(reader.GetValue({ordinal}))",
            "Guid" => $"reader.GetGuid({ordinal})",
            "byte[]" => $"(byte[])reader.GetValue({ordinal})",
            _ => $"({type})reader.GetValue({ordinal})",
        };
    }
}