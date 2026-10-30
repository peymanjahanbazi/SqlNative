namespace SqlNative.ModelGenerator.Schema;

internal sealed record ColumnInfo(string Name, string RawType, bool IsNullable, bool IsPrimaryKey, bool IsAutoIncrement);

internal sealed record TableInfo(string? Schema, string Name, IReadOnlyList<ColumnInfo> Columns, bool IsView = false);

internal sealed record DatabaseSchema(string DatabaseName, IReadOnlyList<TableInfo> Tables);