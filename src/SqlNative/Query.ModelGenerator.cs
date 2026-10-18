using SqlNative.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;

namespace SqlNative;

internal static class QueryModelGenerator
{
    public static void Model(DbDataReader reader, string className, string nameSpace, string file, DatabaseEngine engine = DatabaseEngine.MySql)
    {
        var schema = reader.GetSchemaTable();

        var columnNames = new List<string>();
        var propertyNames = new List<string>();
        var propertyTypes = new List<string>();
        var occurrence = new List<int>();
        var nameTracker = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var helper = PickIdentifierHelper(engine);

        for (var i = 0; i < reader.FieldCount; i++)
        {
            var columnName = reader.GetName(i);
            var propertyName = helper.EnsureUnique(helper.GetObjectNameInCs(columnName), usedNames);
            var propertyType = GetClrType(reader, schema, i);

            int occ;
            if (nameTracker.TryGetValue(columnName, out var existing))
            {
                occ = existing;
                nameTracker[columnName] = occ + 1;
            }
            else
            {
                occ = 0;
                nameTracker[columnName] = 1;
            }

            columnNames.Add(columnName);
            propertyNames.Add(propertyName);
            propertyTypes.Add(propertyType);
            occurrence.Add(occ);
        }

        var builder = new StringBuilder();
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Data.Common;");
        builder.AppendLine("using SqlNative;");
        builder.AppendLine();

        var hasNamespace = !string.IsNullOrWhiteSpace(nameSpace);
        if (hasNamespace)
        {
            builder.AppendLine($"namespace {nameSpace};");
            builder.AppendLine();
        }

        builder.AppendLine($"public partial class {helper.GetObjectNameInCs(className)} : QueryResult");
        builder.AppendLine("{");

        for (int i = 0; i < columnNames.Count; i++)
        {
            builder.AppendLine($"    [SqlNative.SqlNativeColumn({helper.EscapeCsStringLiteral(columnNames[i])})]");
            builder.AppendLine($"    public {propertyTypes[i]} {propertyNames[i]} {{ get; set; }}");
        }

        builder.AppendLine();
        if (columnNames.Count == 0)
        {
            builder.AppendLine("    public override int[] GetOrders(DbDataReader reader) => new int[0];");
        }
        else
        {
            builder.AppendLine("    public override int[] GetOrders(DbDataReader reader) => new[] {");
            for (int i = 0; i < columnNames.Count; i++)
            {
                string expr;
                if (occurrence[i] == 0)
                {
                    expr = $"reader.GetOrdinal({helper.EscapeCsStringLiteral(columnNames[i])})";
                }
                else
                {
                    expr = $"QueryResult.OrdinalN(reader, {helper.EscapeCsStringLiteral(columnNames[i])}, {occurrence[i]})";
                }
                builder.AppendLine($"        {expr}{(i < columnNames.Count - 1 ? "," : "")}");
            }
            builder.AppendLine("    };");
        }

        builder.AppendLine();
        builder.AppendLine("    public override void SetFields(DbDataReader r, int[] orders)");
        builder.AppendLine("    {");
        builder.AppendLine("        var o = orders;");
        for (int i = 0; i < columnNames.Count; i++)
        {
            var readerCall = GetTypedReaderCall(propertyTypes[i], i);
            builder.AppendLine($"        if (!r.IsDBNull(o[{i}])) {propertyNames[i]} = {readerCall};");
        }
        builder.AppendLine("    }");

        builder.AppendLine("}");
        File.WriteAllText(file, builder.ToString());
    }

    private static IIdentifierHelper PickIdentifierHelper(DatabaseEngine engine)
        => engine switch
        {
            DatabaseEngine.Postgres => PostgresIdentifier,
            DatabaseEngine.Oracle => OracleIdentifier,
            DatabaseEngine.MsSql => MsSqlIdentifier,
            _ => MySqlIdentifier,
        };

    private static readonly IIdentifierHelper MySqlIdentifier = new MySqlIdentifierAdapter();
    private static readonly IIdentifierHelper PostgresIdentifier = new PostgresIdentifierAdapter();
    private static readonly IIdentifierHelper OracleIdentifier = new OracleIdentifierAdapter();
    private static readonly IIdentifierHelper MsSqlIdentifier = new MsSqlIdentifierAdapter();

    private interface IIdentifierHelper
    {
        string GetObjectNameInCs(string originalname);
        string EnsureUnique(string candidate, ISet<string> usedNames);
        string EscapeCsStringLiteral(string value);
    }

    private sealed class MySqlIdentifierAdapter : IIdentifierHelper
    {
        public string GetObjectNameInCs(string n) => MySqlIdentifierHelper.GetObjectNameInCs(n);
        public string EnsureUnique(string c, ISet<string> u) => MySqlIdentifierHelper.EnsureUnique(c, u);
        public string EscapeCsStringLiteral(string v) => MySqlIdentifierHelper.EscapeCsStringLiteral(v);
    }

    private sealed class PostgresIdentifierAdapter : IIdentifierHelper
    {
        public string GetObjectNameInCs(string n) => PostgresIdentifierHelper.GetObjectNameInCs(n);
        public string EnsureUnique(string c, ISet<string> u) => PostgresIdentifierHelper.EnsureUnique(c, u);
        public string EscapeCsStringLiteral(string v) => PostgresIdentifierHelper.EscapeCsStringLiteral(v);
    }

    private sealed class OracleIdentifierAdapter : IIdentifierHelper
    {
        public string GetObjectNameInCs(string n) => OracleIdentifierHelper.GetObjectNameInCs(n);
        public string EnsureUnique(string c, ISet<string> u) => OracleIdentifierHelper.EnsureUnique(c, u);
        public string EscapeCsStringLiteral(string v) => OracleIdentifierHelper.EscapeCsStringLiteral(v);
    }

    private sealed class MsSqlIdentifierAdapter : IIdentifierHelper
    {
        public string GetObjectNameInCs(string n) => MsSqlIdentifierHelper.GetObjectNameInCs(n);
        public string EnsureUnique(string c, ISet<string> u) => MsSqlIdentifierHelper.EnsureUnique(c, u);
        public string EscapeCsStringLiteral(string v) => MsSqlIdentifierHelper.EscapeCsStringLiteral(v);
    }

    private static string GetTypedReaderCall(string propertyType, int ordinal)
    {
        var nonNull = propertyType.TrimEnd('?');
        return nonNull switch
        {
            "string" => $"r.GetString(o[{ordinal}])",
            "long" => $"r.GetInt64(o[{ordinal}])",
            "int" => $"r.GetInt32(o[{ordinal}])",
            "short" => $"r.GetInt16(o[{ordinal}])",
            "byte" => $"r.GetByte(o[{ordinal}])",
            "bool" => $"r.GetBoolean(o[{ordinal}])",
            "decimal" => $"r.GetDecimal(o[{ordinal}])",
            "double" => $"r.GetDouble(o[{ordinal}])",
            "float" => $"r.GetFloat(o[{ordinal}])",
            "DateTime" => $"r.GetDateTime(o[{ordinal}])",
            "Guid" => $"r.GetGuid(o[{ordinal}])",
            "byte[]" => $"(byte[])r.GetValue(o[{ordinal}])",
            "string[]" => $"(string[])r.GetValue(o[{ordinal}])",
            "int[]" => $"(int[])r.GetValue(o[{ordinal}])",
            "long[]" => $"(long[])r.GetValue(o[{ordinal}])",
            _ => $"({propertyType})r.GetValue(o[{ordinal}])"
        };
    }

    private static string GetClrType(DbDataReader reader, DataTable schema, int ordinal)
    {
        var fieldType = reader.GetFieldType(ordinal);
        if (fieldType == null || fieldType == typeof(DBNull))
        {
            return "object";
        }

        if (fieldType.IsValueType && IsNullableColumn(schema, ordinal))
        {
            return GetTypeName(fieldType) + "?";
        }

        return GetTypeName(fieldType);
    }

    private static bool IsNullableColumn(DataTable schema, int ordinal)
    {
        if (schema == null)
        {
            return false;
        }

        foreach (DataRow row in schema.Rows)
        {
            if ((int)row["ColumnOrdinal"] == ordinal && row["AllowDBNull"] is bool allowDBNull)
            {
                return allowDBNull;
            }
        }

        return false;
    }

    private static string GetTypeName(Type type)
    {
        if (type == typeof(byte[]))
        {
            return "byte[]";
        }

        return Type.GetTypeCode(type) switch
        {
            TypeCode.Boolean => "bool",
            TypeCode.Byte => "byte",
            TypeCode.Decimal => "decimal",
            TypeCode.Double => "double",
            TypeCode.Int16 => "short",
            TypeCode.Int32 => "int",
            TypeCode.Int64 => "long",
            TypeCode.SByte => "sbyte",
            TypeCode.Single => "float",
            TypeCode.String => "string",
            TypeCode.UInt16 => "ushort",
            TypeCode.UInt32 => "uint",
            TypeCode.UInt64 => "ulong",
            _ when type == typeof(DateTime) => "DateTime",
            _ when type == typeof(Guid) => "Guid",
            _ when type == typeof(TimeSpan) => "TimeSpan",
            _ => type.Name,
        };
    }
}