using System.Text.RegularExpressions;

namespace SqlNative.Core;

internal static class OracleClrTypeMapper
{
    private static readonly Regex OracleTypeRegex = new(
        @"^\s*([a-z][a-z0-9_]*)\s*(?:\(\s*(\d+)\s*(?:,\s*\d+)?\s*\))?\s*$",
        RegexOptions.IgnoreCase);

    private static readonly HashSet<string> BinaryTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "blob", "bfile", "raw", "long_raw",
    };

    private static readonly HashSet<string> TextTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "char", "varchar2", "nchar", "nvarchar2", "long",
        "clob", "nclob",
        "xmltype", "xml",
    };

    private static readonly HashSet<string> ValueTypes = new(StringComparer.Ordinal)
    {
        "bool", "char", "sbyte", "byte", "short", "ushort", "int", "uint",
        "long", "ulong", "float", "double", "decimal", "DateTime", "TimeSpan",
    };

    public static string MapToClrType(string columnRawType)
    {
        var match = OracleTypeRegex.Match(columnRawType ?? string.Empty);
        if (!match.Success)
        {
            return "string";
        }

        var baseType = match.Groups[1].Value.ToLowerInvariant();
        var width = match.Groups[2].Success ? match.Groups[2].Value : null;

        var clr = baseType switch
        {
            "number" or "binary_float" or "binary_double" => ResolveNumberType(width),
            "varchar2" or "nvarchar2" or "char" or "nchar" or "clob" or "nclob" or "long" or "xmltype" or "xml" => "string",
            "date" or "timestamp" => "DateTime",
            "interval year" or "interval day" => "TimeSpan",
            "interval" => "TimeSpan",
            "raw" or "long_raw" => "byte[]",
            "blob" or "bfile" => "byte[]",
            "rowid" or "urowid" => "string",
            _ when BinaryTypes.Contains(baseType) => "byte[]",
            _ when TextTypes.Contains(baseType) => "string",
            _ => "string",
        };

        return clr;
    }

    public static string ApplyNullability(string clrType, bool isNullable)
        => isNullable && (ValueTypes.Contains(clrType) || IsReferenceType(clrType) || IsArrayType(clrType)) ? clrType + "?" : clrType;

    public static bool IsReferenceType(string clrType)
        => clrType is "string" or "byte[]" or "object";

    public static bool IsArrayType(string clrType)
        => clrType != null && clrType.EndsWith("[]", StringComparison.Ordinal);

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

    private static string ResolveNumberType(string width)
    {
        // NUMBER(p) / NUMBER(p, s) / BINARY_FLOAT / BINARY_DOUBLE
        if (width is null)
        {
            // NUMBER (no precision) - representable as decimal
            return "decimal";
        }

        if (!int.TryParse(width, out var p))
        {
            return "decimal";
        }

        if (p == 1)
        {
            // Single-bit NUMBER(1) is commonly used as boolean
            return "bool";
        }

        if (p <= 4)
        {
            return "short";
        }

        if (p <= 9)
        {
            return "int";
        }

        if (p <= 18)
        {
            return "long";
        }

        if (p <= 28)
        {
            return "decimal";
        }

        // NUMBER(p, s) with precision > 28: fall back to decimal (may lose precision)
        return "decimal";
    }
}