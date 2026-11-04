using SqlNative.Core;
using SqlNative.ModelGenerator.Schema;
using System.Text;

namespace SqlNative.ModelGenerator.Generation.MySql;

internal sealed class MySqlModelWriter : IModelWriter
{
    private static readonly HashSet<string> StructureReservedNames = new(StringComparer.Ordinal)
    {
        "Alias", "DisableAlias", "Star", "TableName", "Row", "Entity", "Repository",
        "ToString", "Equals", "GetHashCode", "GetType",
    };

    private static readonly HashSet<string> RowReservedNames = new(StringComparer.Ordinal)
    {
        "TableName", "Star", "GetOrders", "SetFields", "OrdinalN",
        "ToString", "Equals", "GetHashCode", "GetType",
    };

    private readonly IModelIdentifierHelper _helper;

    internal MySqlModelWriter(IModelIdentifierHelper helper)
    {
        _helper = helper;
    }

    private sealed record ColumnModel(
        ColumnInfo Column,
        string StructureName,
        string RowName,
        string ClrType,
        string RowType);

    public string Generate(
        TableInfo table,
        string className,
        string repositoryClassName,
        string contextName,
        string packageNamespace)
    {
        var helper = _helper;

        var columns = BuildColumns(table, className, helper);
        var primaryKeys = columns
            .Where(c => c.Column.IsPrimaryKey)
            .OrderBy(c => c.Column.PrimaryKeyOrdinal)
            .ToArray();
        var rowClassName = className + "Row";

        var builder = new StringBuilder();
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Data.Common;");
        builder.AppendLine("using System.Linq;");
        builder.AppendLine("using System.Threading;");
        builder.AppendLine("using System.Threading.Tasks;");
        builder.AppendLine("using SqlNative;");
        builder.AppendLine();
        builder.AppendLine($"namespace {packageNamespace};");
        builder.AppendLine();

        WriteStructure(builder, table, className, columns, helper);
        builder.AppendLine();
        WriteRow(builder, rowClassName, columns, helper);
        builder.AppendLine();
        WriteEntity(builder, className, repositoryClassName, rowClassName, contextName, columns, primaryKeys, helper);

        return builder.ToString();
    }

    private static List<ColumnModel> BuildColumns(
        TableInfo table,
        string className,
        IModelIdentifierHelper helper)
    {
        var structureNames = new HashSet<string>(StructureReservedNames, StringComparer.Ordinal) { className };
        var rowNames = new HashSet<string>(RowReservedNames, StringComparer.Ordinal);
        var columns = new List<ColumnModel>(table.Columns.Count);

        foreach (var column in table.Columns)
        {
            var candidate = helper.GetObjectNameInCs(column.Name);
            var structureName = helper.EnsureUnique(candidate, structureNames);
            var rowName = helper.EnsureUnique(candidate, rowNames);
            var clrType = ClrTypeMapper.MapToClrType(column.RawType);
            var rowType = column.IsPrimaryKey
                ? clrType
                : ClrTypeMapper.ApplyNullability(clrType, column.IsNullable);
            columns.Add(new ColumnModel(column, structureName, rowName, clrType, rowType));
        }

        return columns;
    }

    private static void WriteStructure(
        StringBuilder builder,
        TableInfo table,
        string className,
        IReadOnlyList<ColumnModel> columns,
        IModelIdentifierHelper helper)
    {
        builder.AppendLine($"public partial class {className} : MySqlTableStructure");

        builder.AppendLine("{");
        builder.AppendLine(
            $"    public override string TableName {{ get; }} = {helper.EscapeCsStringLiteral(table.Name)};");
        builder.AppendLine(
            $"    public override string QuotedTableName {{ get; }} = {helper.EscapeCsStringLiteral(BuildQuatedName(table, helper))};");

        foreach (var column in columns)
        {
            builder.AppendLine($"    public TableColumn {column.StructureName} {{ get; private set; }}");
        }

        builder.AppendLine($"    public {className}()");
        builder.AppendLine("    {");
        foreach (var column in columns)
        {
            var needsQuoting = helper.NeedsBackticks(column.Column.Name);
            var needsQuotingLiteral = needsQuoting ? "true" : "false";
            builder.AppendLine(
                $"        {column.StructureName} = new MySqlColumn({helper.EscapeCsStringLiteral(column.Column.Name)}, {needsQuotingLiteral}, {helper.EscapeCsStringLiteral(column.Column.RawType)}, typeof({column.ClrType}), {column.Column.Length}, this);");
        }

        builder.AppendLine("    }");
        builder.AppendLine("}");
    }

    private static void WriteRow(
        StringBuilder builder,
        string rowClassName,
        IReadOnlyList<ColumnModel> columns,
        IModelIdentifierHelper helper)
    {
        builder.AppendLine($"public partial class {rowClassName} : TableRow");
        builder.AppendLine("{");

        foreach (var column in columns)
        {
            var attribute = NeedsColumnAttribute(column)
                ? $"[SqlNativeColumn({helper.EscapeCsStringLiteral(column.Column.Name)})] "
                : string.Empty;
            var initializer = ClrTypeMapper.IsReferenceType(column.ClrType) ? " = null!;" : string.Empty;
            builder.AppendLine($"    {attribute}public {column.RowType} {column.RowName} {{ get; set; }}{initializer}");
        }

        builder.AppendLine();
        WriteGetOrders(builder, columns, helper);
        builder.AppendLine();
        WriteSetFields(builder, columns);
        builder.AppendLine("}");
    }

    private static void WriteGetOrders(
        StringBuilder builder,
        IReadOnlyList<ColumnModel> columns,
        IModelIdentifierHelper helper)
    {
        if (columns.Count == 0)
        {
            builder.AppendLine("    public override int[] GetOrders(DbDataReader reader) => Array.Empty<int>();");
            return;
        }

        builder.AppendLine("    public override int[] GetOrders(DbDataReader reader) => new[]");
        builder.AppendLine("    {");

        var occurrences = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < columns.Count; i++)
        {
            var columnName = columns[i].Column.Name;
            var name = helper.EscapeCsStringLiteral(columnName);
            occurrences.TryGetValue(columnName, out var occurrence);
            occurrences[columnName] = occurrence + 1;

            var expression = occurrence == 0
                ? $"reader.GetOrdinal({name})"
                : $"QueryResult.OrdinalN(reader, {name}, {occurrence})";
            builder.AppendLine($"        {expression}{(i < columns.Count - 1 ? "," : string.Empty)}");
        }

        builder.AppendLine("    };");
    }

    private static void WriteSetFields(StringBuilder builder, IReadOnlyList<ColumnModel> columns)
    {
        builder.AppendLine("    public override void SetFields(DbDataReader reader, int[] orders)");
        builder.AppendLine("    {");
        for (var i = 0; i < columns.Count; i++)
        {
            var ordinal = "orders[" + i + "]";
            var value = ClrTypeMapper.GetReaderExpression(columns[i].ClrType, ordinal);
            builder.AppendLine($"        if (!reader.IsDBNull({ordinal})) {columns[i].RowName} = {value};");
        }

        builder.AppendLine("    }");
    }

    private static bool NeedsColumnAttribute(ColumnModel column)
        => !string.Equals(
            Canonicalize(column.RowName),
            Canonicalize(column.Column.Name),
            StringComparison.OrdinalIgnoreCase);

    private static string Canonicalize(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            if (char.IsLetterOrDigit(c))
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }

    private static void WriteEntity(
        StringBuilder builder,
        string className,
        string repositoryClassName,
        string rowClassName,
        string contextName,
        IReadOnlyList<ColumnModel> columns,
        IReadOnlyList<ColumnModel> primaryKeys,
        IModelIdentifierHelper helper)
    {
        var updateColumns = columns
            .Where(c => !c.Column.IsPrimaryKey)
            .ToArray();

        builder.AppendLine(
            $"public partial class {repositoryClassName} : TableRepository<{rowClassName}, {className}>");
        builder.AppendLine("{");
        builder.AppendLine($"    private readonly {contextName} ctx;");
        builder.AppendLine();
        builder.AppendLine($"    public {repositoryClassName}({contextName} ctx)");
        builder.AppendLine("    {");
        builder.AppendLine("        this.ctx = ctx;");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    public override SqlNativeContext Context => ctx;");
        builder.AppendLine();

        WriteDelete(builder, className, rowClassName, primaryKeys, helper);
        builder.AppendLine();
        WriteInsert(builder, className, rowClassName, primaryKeys, columns, helper);
        builder.AppendLine();
        WriteUpdate(builder, className, rowClassName, primaryKeys, updateColumns, helper, columns);
        builder.AppendLine();
        WriteIsPrimaryKeySet(builder, rowClassName, primaryKeys);

        builder.AppendLine("}");
    }

    private static void WriteDelete(
            StringBuilder builder,
            string className,
            string rowClassName,
            IReadOnlyList<ColumnModel> primaryKeys,
            IModelIdentifierHelper helper)
    {
        var hasPrimaryKey = primaryKeys.Count > 0;


        builder.AppendLine($"    public override Task<int> DeleteAsync(List<{rowClassName}> rows, CancellationToken ct = default)");
        builder.AppendLine("    {");
        if (hasPrimaryKey)
        {
            builder.AppendLine("        if (rows == null || rows.Count == 0)");
            builder.AppendLine("        {");
            builder.AppendLine("            return Task.FromResult(0);");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        var keys = new List<string>();");
            builder.AppendLine("        var parameters = new Dictionary<string, object?>();");
            builder.AppendLine("        var index = 1;");
            builder.AppendLine("        foreach (var item in rows)");
            builder.AppendLine("        {");
            if (primaryKeys.Count == 1)
            {
                builder.AppendLine("            var p = \"@p\" + index++;");
                builder.AppendLine($"            parameters[p] = item.{primaryKeys[0].RowName};");
                builder.AppendLine("            keys.Add(p);");
            }
            else
            {
                builder.AppendLine("            var conditions = new List<string>();");
                builder.AppendLine("            var conditionIndex = 0;");
                foreach (var key in primaryKeys)
                {
                    builder.AppendLine($"            var p{key.StructureName} = \"@p\" + (index + conditionIndex++);");
                    builder.AppendLine($"            parameters[p{key.StructureName}] = item.{key.RowName};");
                    var quotedKeyName = helper.WrapObjectName(key.Column.Name);
                    builder.AppendLine($"            conditions.Add(\"{quotedKeyName.Replace("\"", "\\\"")}\" + \" = \" + p{key.StructureName});");
                }
                builder.AppendLine("            index += conditionIndex;");
                builder.AppendLine("            keys.Add(\"(\" + string.Join(\" AND \", conditions) + \")\");");
            }
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine($"        return Context.BuildQuery<{className}>((q, e) => q");
            builder.AppendLine("            .Config(() => e.DisableAlias = true)");
            builder.AppendLine("            .Append($\"DELETE FROM {e}\")");
            if (primaryKeys.Count == 1)
            {
                builder.AppendLine($"            .Append($\"WHERE {{e.{primaryKeys[0].StructureName}}} IN (\" + string.Join(\", \", keys) + \")\")");
            }
            else
            {
                builder.AppendLine("            .Append($\"WHERE \" + string.Join(\" OR \", keys))");
            }
            builder.AppendLine("            .SetParameters(parameters)");
            builder.AppendLine("            ).ExecuteNonQueryAsync(ct);");
        }
        else
        {
            builder.AppendLine("        throw new NotImplementedException();");
        }

        builder.AppendLine("    }");
    }

    private static void WriteInsert(
        StringBuilder builder,
        string className,
        string rowClassName,
        IReadOnlyList<ColumnModel> primaryKeys,
        IReadOnlyList<ColumnModel> columns,
        IModelIdentifierHelper helper)
    {
        var hasPrimaryKey = primaryKeys.Count > 0;

        var allColumns = columns.ToArray();
        var columnsWithoutPk = columns
            .Where(c => !c.Column.IsPrimaryKey)
            .ToArray();


        builder.AppendLine($"    public override async Task<int> InsertAsync(List<{rowClassName}> rows, OperationOptions options = OperationOptions.SkipPrimaryKey, CancellationToken ct = default)");
        builder.AppendLine("    {");
        if (columns.Count == 0)
        {
            builder.AppendLine("        throw new NotImplementedException();");
        }
        else
        {
            builder.AppendLine("        if (rows == null || rows.Count == 0)");
            builder.AppendLine("        {");
            builder.AppendLine("            return 0;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        var skipPrimaryKey = options == OperationOptions.SkipPrimaryKey || options == OperationOptions.LoadAndSkip;");
            builder.AppendLine("        var loadRecord = options == OperationOptions.LoadRecord || options == OperationOptions.LoadAndSkip;");
            builder.AppendLine();
            if (!hasPrimaryKey)
            {
                builder.AppendLine("        if (loadRecord)");
                builder.AppendLine("        {");
                builder.AppendLine("            throw new NotImplementedException(\"Cannot load inserted rows: table has no primary key.\");");
                builder.AppendLine("        }");
                builder.AppendLine();
            }
            var autoIncrementKeyCount = primaryKeys.Count(c => c.Column.IsAutoIncrement);
            if (hasPrimaryKey && autoIncrementKeyCount > 0 && autoIncrementKeyCount != primaryKeys.Count)
            {
                // A composite key cannot mix generated and supplied columns.
                builder.AppendLine("        throw new NotImplementedException(\"Cannot insert rows: the composite primary key mixes auto-generated and supplied columns.\");");
            }
            else if (hasPrimaryKey && autoIncrementKeyCount == primaryKeys.Count)
            {
                // The primary key is generated by the database, so every row is inserted on its
                // own and its key is read back with LAST_INSERT_ID() right after the insert.
                var pkStructure = primaryKeys[0].StructureName;
                var withoutPkColumnsText = string.Join(", ", columnsWithoutPk.Select(c => "{e." + c.StructureName + "}"));
                var withoutPkValues = string.Join(", ", columnsWithoutPk.Select(c => "@" + c.RowName));
                var withoutPkParameters = string.Join(", ", columnsWithoutPk.Select(c => c.RowName + " = item." + c.RowName));
                builder.AppendLine("        foreach (var item in rows)");
                builder.AppendLine("        {");
                builder.AppendLine($"            await Context.BuildQuery<{className}>((q, e) => q");
                builder.AppendLine("                .Config(() => e.DisableAlias = true)");
                builder.AppendLine("                .Append($\"INSERT INTO {e}\")");
                builder.AppendLine($"                .Append($\"({withoutPkColumnsText})\")");
                builder.AppendLine($"                .Append($\"VALUES ({withoutPkValues})\")");
                builder.AppendLine($"                .SetParameters(new {{ {withoutPkParameters} }})");
                builder.AppendLine("                ).ExecuteNonQueryAsync(ct).ConfigureAwait(false);");
                builder.AppendLine();
                builder.AppendLine("            if (loadRecord)");
                builder.AppendLine("            {");
                builder.AppendLine($"                var loaded = await SelectAsync(t => $\"{{t.{pkStructure}}} = LAST_INSERT_ID()\", null, ct).ConfigureAwait(false);");
                builder.AppendLine("                if (loaded.Count > 0)");
                builder.AppendLine("                {");
                builder.AppendLine("                    var source = loaded[0];");
                WriteFieldCopies(builder, "item", "source", columns);
                builder.AppendLine("                }");
                builder.AppendLine("            }");
                builder.AppendLine("        }");
                builder.AppendLine();
                builder.AppendLine("        return rows.Count;");
            }
            else
            {
            builder.AppendLine("        var parameters = new Dictionary<string, object?>();");
            builder.AppendLine("        var tuples = new List<string>();");
            builder.AppendLine("        var index = 1;");
            builder.AppendLine();
            builder.AppendLine("        foreach (var item in rows)");
            builder.AppendLine("        {");
            builder.AppendLine("            var placeholders = new List<string>();");
            builder.AppendLine();
            var pkColumns = allColumns.Where(x => x.Column.IsPrimaryKey).ToArray();
            var pIndex = 0;
            if (pkColumns.Length > 0)
            {
                builder.AppendLine("            if (!skipPrimaryKey)");
                builder.AppendLine("            {");
                foreach (var pk in pkColumns)
                {
                    builder.AppendLine($"                var p{pIndex} = \"@p\" + index++;");
                    builder.AppendLine($"                parameters[p{pIndex}] = item.{pk.RowName};");
                    builder.AppendLine($"                placeholders.Add(p{pIndex});");
                    pIndex++;
                }

                builder.AppendLine("            }");
            }

            for (var i = 0; i < columnsWithoutPk.Length; i++)
            {
                var c = columnsWithoutPk[i];
                builder.AppendLine($"            var p{pIndex} = \"@p\" + index++;");
                builder.AppendLine($"            parameters[p{pIndex}] = item.{c.RowName};");
                builder.AppendLine($"            placeholders.Add(p{pIndex});");
                pIndex++;
            }

            builder.AppendLine();
            builder.AppendLine("            tuples.Add(\"(\" + string.Join(\", \", placeholders) + \")\");");
            builder.AppendLine("        }");
            builder.AppendLine();

            var allColumnsLambdaExpr = string.Join(", ", allColumns.Select(c => "{e." + c.StructureName + "}"));
            var withoutPkColumnsLambdaExpr = string.Join(", ", columnsWithoutPk.Select(c => "{e." + c.StructureName + "}"));

            builder.AppendLine($"        var affected = await Context.BuildQuery<{className}>((q, e) => q");
            builder.AppendLine("            .Config(() => e.DisableAlias = true)");
            builder.AppendLine("            .Append($\"INSERT INTO {e}\")");
            builder.AppendLine($"            .Append($\"({{(skipPrimaryKey ? $\"{withoutPkColumnsLambdaExpr}\" : $\"{allColumnsLambdaExpr}\")}})\")");
            builder.AppendLine("            .Append(\"VALUES \" + string.Join(\", \", tuples))");
            builder.AppendLine("            .SetParameters(parameters)");
            builder.AppendLine("            ).ExecuteNonQueryAsync(ct).ConfigureAwait(false);");
            builder.AppendLine();

            if (hasPrimaryKey)
            {
                var pkStructure = primaryKeys[0].StructureName;
                builder.AppendLine("        if (loadRecord)");
                builder.AppendLine("        {");
                builder.AppendLine("            var reloadParameters = new Dictionary<string, object?>();");
                builder.AppendLine("            var reloadParts = new List<string>();");
                builder.AppendLine("            var reloadIndex = 1;");
                builder.AppendLine("            foreach (var item in rows)");
                builder.AppendLine("            {");
                if (primaryKeys.Count == 1)
                {
                    builder.AppendLine("                var p = \"@p\" + reloadIndex++;");
                    builder.AppendLine($"                reloadParameters[p] = item.{primaryKeys[0].RowName};");
                    builder.AppendLine("                reloadParts.Add(p);");
                }
                else
                {
                    builder.AppendLine("                var conditions = new List<string>();");
                    builder.AppendLine("                var conditionIndex = 0;");
                    foreach (var key in primaryKeys)
                    {
                        builder.AppendLine($"                var p{key.StructureName} = \"@p\" + (reloadIndex + conditionIndex++);");
                        builder.AppendLine($"                parameters[p{key.StructureName}] = item.{key.RowName};");
                        var quotedKeyName = helper.WrapObjectName(key.Column.Name);
                        builder.AppendLine($"                conditions.Add(\"{quotedKeyName.Replace("\"", "\\\"")}\" + \" = \" + p{key.StructureName});");
                    }
                    builder.AppendLine("                reloadIndex += conditionIndex;");
                    builder.AppendLine("                reloadParts.Add(\"(\" + string.Join(\" AND \", conditions) + \")\");");
                }
                builder.AppendLine("            }");
                builder.AppendLine();
                builder.AppendLine($"            var loaded = await Context.BuildQuery<{className}>((q, e) => q");
                builder.AppendLine("                .Append($\"SELECT {e.Star}\")");
                builder.AppendLine("                .Append($\"FROM {e}\")");
                if (primaryKeys.Count == 1)
                {
                    builder.AppendLine($"                .Append($\"WHERE {{e.{pkStructure}}} IN (\" + string.Join(\", \", reloadParts) + \")\")");
                }
                else
                {
                    builder.AppendLine("                .Append($\"WHERE \" + string.Join(\" OR \", reloadParts))");
                }
                builder.AppendLine("                .SetParameters(reloadParameters)");
                builder.AppendLine($"            ).ExecuteAsync<{rowClassName}>(ct).ConfigureAwait(false);");
                builder.AppendLine();
                var isSingleKey = primaryKeys.Count == 1;
                var keyType = isSingleKey ? primaryKeys[0].ClrType : string.Join(", ", primaryKeys.Select(k => k.ClrType));
                var itemKey = string.Join(", ", primaryKeys.Select(k => "item." + k.RowName));
                var srcKey = string.Join(", ", primaryKeys.Select(k => "src." + k.RowName));
                if (isSingleKey)
                {
                    builder.AppendLine($"            var byKey = new Dictionary<{keyType}, {rowClassName}>(rows.Count);");
                    builder.AppendLine("            foreach (var src in loaded)");
                    builder.AppendLine("            {");
                    builder.AppendLine($"                byKey[src.{primaryKeys[0].RowName}] = src;");
                    builder.AppendLine("            }");
                    builder.AppendLine();
                    builder.AppendLine("            foreach (var item in rows)");
                    builder.AppendLine("            {");
                    builder.AppendLine($"                if (byKey.TryGetValue(item.{primaryKeys[0].RowName}, out var source))");
                }
                else
                {
                    builder.AppendLine($"            var byKey = new Dictionary<({keyType}), {rowClassName}>(rows.Count);");
                    builder.AppendLine("            foreach (var src in loaded)");
                    builder.AppendLine("            {");
                    builder.AppendLine($"                byKey[({srcKey})] = src;");
                    builder.AppendLine("            }");
                    builder.AppendLine();
                    builder.AppendLine("            foreach (var item in rows)");
                    builder.AppendLine("            {");
                    builder.AppendLine($"                if (byKey.TryGetValue(({itemKey}), out var source))");
                }
                builder.AppendLine("                {");
                WriteFieldCopies(builder, "item", "source", columns);
                builder.AppendLine("                }");
                builder.AppendLine("            }");
                builder.AppendLine("        }");
                builder.AppendLine();
            }

            builder.AppendLine("        return affected;");
            }
        }

        builder.AppendLine("    }");
    }

    private static void WriteUpdate(
        StringBuilder builder,
        string className,
        string rowClassName,
        IReadOnlyList<ColumnModel> primaryKeys,
        IReadOnlyList<ColumnModel> updateColumns,
        IModelIdentifierHelper helper,
        IReadOnlyList<ColumnModel> columns)
    {
        var canUpdate = primaryKeys.Count > 0 && updateColumns.Count > 0;


        builder.AppendLine($"    public override async Task<int> UpdateAsync(List<{rowClassName}> rows, OperationOptions options = OperationOptions.None, CancellationToken ct = default)");
        builder.AppendLine("    {");
        if (!canUpdate)
        {
            builder.AppendLine("        throw new NotImplementedException();");
        }
        else
        {
            var pkStructure = primaryKeys[0].StructureName;

            builder.AppendLine("        if (rows == null || rows.Count == 0)");
            builder.AppendLine("        {");
            builder.AppendLine("            return 0;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        var reload = options == OperationOptions.LoadRecord || options == OperationOptions.LoadAndSkip;");
            WriteUpdateRowsBody(builder, className, rowClassName, primaryKeys, updateColumns, helper);
            builder.AppendLine();

            builder.AppendLine("        if (reload)");
            builder.AppendLine("        {");
            builder.AppendLine("            var reloadParameters = new Dictionary<string, object?>();");
            builder.AppendLine("            var reloadParts = new List<string>();");
            builder.AppendLine("            var reloadIndex = 1;");
            builder.AppendLine("            foreach (var item in rows)");
            builder.AppendLine("            {");
            if (primaryKeys.Count == 1)
            {
                builder.AppendLine("                var p = \"@p\" + reloadIndex++;");
                builder.AppendLine($"                reloadParameters[p] = item.{primaryKeys[0].RowName};");
                builder.AppendLine("                reloadParts.Add(p);");
            }
            else
            {
                builder.AppendLine("                var conditions = new List<string>();");
                builder.AppendLine("                var conditionIndex = 0;");
                foreach (var key in primaryKeys)
                {
                    builder.AppendLine($"                var p{key.StructureName} = \"@p\" + (reloadIndex + conditionIndex++);");
                    builder.AppendLine($"                parameters[p{key.StructureName}] = item.{key.RowName};");
                    var quotedKeyName = helper.WrapObjectName(key.Column.Name);
                    builder.AppendLine($"                conditions.Add(\"{quotedKeyName.Replace("\"", "\\\"")}\" + \" = \" + p{key.StructureName});");
                }
                builder.AppendLine("                reloadIndex += conditionIndex;");
                builder.AppendLine("                reloadParts.Add(\"(\" + string.Join(\" AND \", conditions) + \")\");");
            }
            builder.AppendLine("            }");
            builder.AppendLine();
            builder.AppendLine($"            var loaded = await Context.BuildQuery<{className}>((q, e) => q");
            builder.AppendLine("                .Append($\"SELECT {e.Star}\")");
            builder.AppendLine("                .Append($\"FROM {e}\")");
            if (primaryKeys.Count == 1)
            {
                builder.AppendLine($"                .Append($\"WHERE {{e.{pkStructure}}} IN (\" + string.Join(\", \", reloadParts) + \")\")");
            }
            else
            {
                builder.AppendLine("                .Append($\"WHERE \" + string.Join(\" OR \", reloadParts))");
            }
            builder.AppendLine("                .SetParameters(reloadParameters)");
            builder.AppendLine($"            ).ExecuteAsync<{rowClassName}>(ct).ConfigureAwait(false);");
            builder.AppendLine();
            var isSingleKey = primaryKeys.Count == 1;
            var keyType = isSingleKey ? primaryKeys[0].ClrType : string.Join(", ", primaryKeys.Select(k => k.ClrType));
            var itemKey = string.Join(", ", primaryKeys.Select(k => "item." + k.RowName));
            var srcKey = string.Join(", ", primaryKeys.Select(k => "src." + k.RowName));
            if (isSingleKey)
            {
                builder.AppendLine($"            var byKey = new Dictionary<{keyType}, {rowClassName}>(rows.Count);");
                builder.AppendLine("            foreach (var src in loaded)");
                builder.AppendLine("            {");
                builder.AppendLine($"                byKey[src.{primaryKeys[0].RowName}] = src;");
                builder.AppendLine("            }");
                builder.AppendLine();
                builder.AppendLine("            foreach (var item in rows)");
                builder.AppendLine("            {");
                builder.AppendLine($"                if (byKey.TryGetValue(item.{primaryKeys[0].RowName}, out var source))");
            }
            else
            {
                builder.AppendLine($"            var byKey = new Dictionary<({keyType}), {rowClassName}>(rows.Count);");
                builder.AppendLine("            foreach (var src in loaded)");
                builder.AppendLine("            {");
                builder.AppendLine($"                byKey[({srcKey})] = src;");
                builder.AppendLine("            }");
                builder.AppendLine();
                builder.AppendLine("            foreach (var item in rows)");
                builder.AppendLine("            {");
                builder.AppendLine($"                if (byKey.TryGetValue(({itemKey}), out var source))");
            }
            builder.AppendLine("                {");
            WriteFieldCopies(builder, "item", "source", columns);
            builder.AppendLine("                }");
            builder.AppendLine("            }");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        return affected;");
        }

        builder.AppendLine("    }");
    }

    private static List<ColumnModel> SourceColumns(
        IReadOnlyList<ColumnModel> primaryKeys,
        IReadOnlyList<ColumnModel> updateColumns)
    {
        var columns = new List<ColumnModel>(primaryKeys.Count + updateColumns.Count);
        columns.AddRange(primaryKeys);
        columns.AddRange(updateColumns);
        return columns;
    }

    private static void WriteFieldCopies(
        StringBuilder builder,
        string rowVar,
        string srcVar,
        IReadOnlyList<ColumnModel> columns)
    {
        foreach (var column in columns)
        {
            builder.AppendLine($"            {rowVar}.{column.RowName} = {srcVar}.{column.RowName};");
        }
    }

    /// <summary>
    /// Writes the body of the multi-row update used by
    /// <c>UpdateAsync(List&lt;Row&gt;)</c>. The whole list is sent in a single statement,
    /// so the caller is responsible for splitting it into chunks the database accepts.
    /// </summary>
    private static void WriteUpdateRowsBody(
        StringBuilder builder,
        string className,
        string rowClassName,
        IReadOnlyList<ColumnModel> primaryKeys,
        IReadOnlyList<ColumnModel> updateColumns,
        IModelIdentifierHelper helper)
    {
        var sourceColumns = SourceColumns(primaryKeys, updateColumns);
        var setParts = new List<string>(updateColumns.Count);
        foreach (var column in updateColumns)
        {
            setParts.Add("{e." + column.StructureName + "} = t2." + helper.WrapObjectName(column.Column.Name));
        }

        var onClause = string.Join(" AND ", primaryKeys.Select(k => "{e." + k.StructureName + "} = t2." + helper.WrapObjectName(k.Column.Name)));

        builder.AppendLine("        var parameters = new Dictionary<string, object?>();");
        builder.AppendLine("        var tuples = new List<string>();");
        builder.AppendLine("        var index = 1;");
        builder.AppendLine();
        builder.AppendLine("        for (var i = 0; i < rows.Count; i++)");
        builder.AppendLine("        {");
        builder.AppendLine("            var item = rows[i];");
        for (var i = 0; i < sourceColumns.Count; i++)
        {
            builder.AppendLine($"            var p{i} = \"@p\" + index++;");
        }

        builder.AppendLine();
        for (var i = 0; i < sourceColumns.Count; i++)
        {
            builder.AppendLine($"            parameters[p{i}] = item.{sourceColumns[i].RowName};");
        }

        builder.AppendLine();
        var rowPlaceholders = string.Join(", ", Enumerable.Range(0, sourceColumns.Count).Select(i => "{p" + i + "}"));
        // MySQL 5.7 has no ROW(...) values constructor, so the derived table is built
        // from UNION ALL'ed SELECTs. The first SELECT (re)declares the column names,
        // the rest just select the parameter values in order.
        var firstRow = string.Join(", ", sourceColumns.Select((c, i) => "{p" + i + "} AS " + helper.WrapObjectName(c.Column.Name)));
        builder.AppendLine("            if (i == 0)");
        builder.AppendLine("                tuples.Add($\"SELECT " + firstRow + "\");");
        builder.AppendLine("            else");
        builder.AppendLine("                tuples.Add($\"SELECT " + rowPlaceholders + "\");");
        builder.AppendLine("        }");
        builder.AppendLine();
        builder.AppendLine("        var valuesClause = string.Join(\" UNION ALL \", tuples);");
        builder.AppendLine();
        builder.AppendLine($"        var affected = await Context.BuildQuery<{className}>((q, e) => q");
        builder.AppendLine("            .Config(() => e.Alias = \"t1\")");
        builder.AppendLine("            .Append($\"UPDATE {e}\")");
        builder.AppendLine($"            .Append($\"INNER JOIN (\" + valuesClause + $\") AS t2 ON " + onClause + "\")");
        builder.AppendLine("            .Append($\"SET " + string.Join(", ", setParts) + "\")");
        builder.AppendLine("            .SetParameters(parameters)");
        builder.AppendLine("            ).ExecuteNonQueryAsync(ct).ConfigureAwait(false);");
    }

    private static string BuildQuatedName(TableInfo table, IModelIdentifierHelper helper)
        => helper.WrapObjectName(table.Name);

    /// <summary>
    /// Builds a C# interpolated string literal holding a comma separated list of table column
    /// references (e.g. "$"{e.Id}, {e.Name}"").
    /// The <c>+</c> operator on <see cref="SqlNative.TableColumn"/> joins with a space, which would
    /// produce an invalid column list, so string interpolation is used instead.
    /// </summary>
    private static string BuildColumnsExpression(IReadOnlyList<ColumnModel> columns)
    {
        if (columns.Count == 0)
        {
            return "string.Empty";
        }

        return "$\"" + string.Join(", ", columns.Select(c => "{e." + c.StructureName + "}")) + "\"";
    }

    /// <summary>
    /// Writes the <c>IsPrimaryKeySet</c> override for the generated repository. The primary key
    /// counts as set when every key column holds a value that is neither <c>null</c> nor the
    /// CLR default value, so single-column and composite (multi-column) keys are both supported.
    /// </summary>
    private static void WriteIsPrimaryKeySet(
        StringBuilder builder,
        string rowClassName,
        IReadOnlyList<ColumnModel> primaryKeys)
    {
        builder.AppendLine($"    protected override bool IsPrimaryKeySet({rowClassName} item)");
        builder.AppendLine("    {");

        if (primaryKeys.Count == 0)
        {
            builder.AppendLine("        // The table has no primary key, so the key is never considered set.");
            builder.AppendLine("        return false;");
        }
        else
        {
            for (var i = 0; i < primaryKeys.Count; i++)
            {
                var key = primaryKeys[i];
                var condition = ClrTypeMapper.IsReferenceType(key.ClrType)
                    ? $"item.{key.RowName} != null"
                    : $"item.{key.RowName} != default";
                var end = i == primaryKeys.Count - 1 ? ";" : string.Empty;
                builder.AppendLine(i == 0
                    ? $"        return {condition}{end}"
                    : $"            && {condition}{end}");
            }
        }

        builder.AppendLine("    }");
    }
}
