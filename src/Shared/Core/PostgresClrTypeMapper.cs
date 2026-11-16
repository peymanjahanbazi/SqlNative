using System.Text.RegularExpressions;

namespace SqlNative.Core;

internal static class PostgresClrTypeMapper
{
    private static readonly Regex PostgresTypeRegex = new(
        @"^\s*([a-z][a-z0-9_]*)\s*(?:\(\s*(\d+)\s*(?:,\s*\d+)?\s*\))?\s*(\[\s*\])?\s*$",
        RegexOptions.IgnoreCase);

    private static readonly HashSet<string> BinaryTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "bytea",
    };

    private static readonly HashSet<string> TextTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "char", "varchar", "character", "character varying", "text", "name",
        "json", "jsonb", "jsonpath", "citext", "xml",
        "inet", "cidr", "macaddr", "macaddr8", "pg_lsn",
    };

    private static readonly HashSet<string> ValueTypes = new(StringComparer.Ordinal)
    {
        "bool", "char", "sbyte", "byte", "short", "ushort", "int", "uint",
        "long", "ulong", "float", "double", "decimal", "DateTime", "TimeSpan",
    };

    public static string MapToClrType(string columnRawType)
    {
        var match = PostgresTypeRegex.Match(columnRawType ?? string.Empty);
        if (!match.Success)
        {
            return "string";
        }

        var baseType = match.Groups[1].Value.ToLowerInvariant();
        var isArray = match.Groups[3].Success;
        var width = match.Groups[2].Success ? match.Groups[2].Value : null;

        var clr = baseType switch
        {
            "smallint" or "int2" => "short",
            "int" or "int4" or "integer" => "int",
            "bigint" or "int8" => "long",
            "smallserial" or "serial2" => "short",
            "serial" or "serial4" => "int",
            "bigserial" or "serial8" => "long",
            "real" or "float4" => "float",
            "float" or "float8" or "double precision" => "double",
            "numeric" or "decimal" => "decimal",
            "money" => "decimal",
            "boolean" or "bool" => "bool",
            "bit" => width == "1" ? "bool" : "ulong",
            "date" => "DateTime",
            "timestamp" or "timestamptz" => "DateTime",
            "time" or "timetz" => "TimeSpan",
            "uuid" => "Guid",
            "xml" or "json" or "jsonb" or "citext" => "string",
            _ when BinaryTypes.Contains(baseType) => "byte[]",
            _ when TextTypes.Contains(baseType) => "string",
            _ => "string",
        };

        return isArray ? clr + "[]" : clr;
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
            "string[]" => $"(string[])reader.GetValue({ordinal})",
            "int[]" => $"(int[])reader.GetValue({ordinal})",
            "long[]" => $"(long[])reader.GetValue({ordinal})",
            _ => $"({type})reader.GetValue({ordinal})",
        };
    }
}