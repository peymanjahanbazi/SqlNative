using SqlNative.ModelGenerator.Generation;
using SqlNative.ModelGenerator.Schema;
using SqlNative.Core;
using System;
using System.Text;

namespace SqlNative.ModelGenerator.Generation.MsSql;

internal sealed class MsSqlModelWriter : IModelWriter
{
    private const string DefaultParametersLiteral = "60_000";

    private static readonly HashSet<string> StructureReservedNames = new(StringComparer.Ordinal)
    {
        "Alias", "DisableAlias", "PKey", "Star", "TableName", "Row", "Entity",
        "Schema", "ToString", "Equals", "GetHashCode", "GetType",
    };

    private static readonly HashSet<string> RowReservedNames = new(StringComparer.Ordinal)
    {
        "Key", "TableName", "Star", "GetOrders", "SetFields", "OrdinalN",
        "ToString", "Equals", "GetHashCode", "GetType",
    };

    private readonly IModelIdentifierHelper _helper;

    internal MsSqlModelWriter(IModelIdentifierHelper helper)
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
        string entityClassName,
        string contextName,
        string packageNamespace)
    {
        var helper = _helper;

        var columns = BuildColumns(table, className, helper);
        var primaryKey = columns.FirstOrDefault(c => c.Column.IsPrimaryKey);
        var keyType = primaryKey?.ClrType ?? "object";
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

        WriteStructure(builder, table, className, columns, primaryKey, helper);
        builder.AppendLine();
        WriteRow(builder, rowClassName, columns, primaryKey, keyType, helper);
        builder.AppendLine();
        WriteEntity(builder, table, className, entityClassName, rowClassName, contextName, columns, primaryKey, keyType, helper);

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
            var clrType = MsSqlClrTypeMapper.MapToClrType(column.RawType);
            var rowType = column.IsPrimaryKey
                ? clrType
                : MsSqlClrTypeMapper.ApplyNullability(clrType, column.IsNullable);
            columns.Add(new ColumnModel(column, structureName, rowName, clrType, rowType));
        }

        return columns;
    }

    private static void WriteStructure(
        StringBuilder builder,
        TableInfo table,
        string className,
        IReadOnlyList<ColumnModel> columns,
        ColumnModel? primaryKey,
        IModelIdentifierHelper helper)
    {
        builder.AppendLine(primaryKey is null
            ? $"public partial class {className} : MsSqlTableStructure"
            : $"public partial class {className} : MsSqlTableStructure, HasPrimaryKey");

        builder.AppendLine("{");
        builder.AppendLine(
            $"    public override string TableName {{ get; }} = {helper.EscapeCsStringLiteral(table.Name)};");
        builder.AppendLine(
            $"    protected override string QuotedTableName {{ get; }} = {helper.EscapeCsStringLiteral(BuildQuatedName(table, helper))};");

        if (!string.IsNullOrEmpty(table.Schema))
        {
            builder.AppendLine(
                $"    public override string Schema {{ get; }} = {helper.EscapeCsStringLiteral(table.Schema)};");
        }

        foreach (var column in columns)
        {
            builder.AppendLine($"    public TableColumn {column.StructureName} {{ get; private set; }}");
        }

        builder.AppendLine();
        builder.AppendLine(primaryKey is null
            ? "    public override TableColumn? PKey => null;"
            : $"    public override TableColumn? PKey => {primaryKey.StructureName};");

        builder.AppendLine();
        builder.AppendLine($"    public {className}()");
        builder.AppendLine("    {");
        foreach (var column in columns)
        {
            var needsQuoting = helper.NeedsBackticks(column.Column.Name);
            var needsQuotingLiteral = needsQuoting ? "true" : "false";
            builder.AppendLine(
                $"        {column.StructureName} = new MsSqlColumn({helper.EscapeCsStringLiteral(column.Column.Name)}, {needsQuotingLiteral}, {helper.EscapeCsStringLiteral(column.Column.RawType)}, typeof({column.ClrType}), this);");
        }

        builder.AppendLine("    }");
        builder.AppendLine("}");
    }

    private static void WriteRow(
        StringBuilder builder,
        string rowClassName,
        IReadOnlyList<ColumnModel> columns,
        ColumnModel? primaryKey,
        string keyType,
        IModelIdentifierHelper helper)
    {
        builder.AppendLine($"public partial class {rowClassName} : TableRow<{keyType}>");
        builder.AppendLine("{");

        foreach (var column in columns)
        {
            var attribute = NeedsColumnAttribute(column)
                ? $"[SqlNativeColumn({helper.EscapeCsStringLiteral(column.Column.Name)})] "
                : string.Empty;
            var initializer = MsSqlClrTypeMapper.IsReferenceType(column.ClrType) ? " = null!;" : string.Empty;
            builder.AppendLine($"    {attribute}public {column.RowType} {column.RowName} {{ get; set; }}{initializer}");
        }

        builder.AppendLine(primaryKey is null
            ? $"    public override {keyType} Key => default!;"
            : $"    public override {keyType} Key => {primaryKey.RowName};");
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
            var value = MsSqlClrTypeMapper.GetReaderExpression(columns[i].ClrType, ordinal);
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
        TableInfo table,
        string className,
        string entityClassName,
        string rowClassName,
        string contextName,
        IReadOnlyList<ColumnModel> columns,
        ColumnModel? primaryKey,
        string keyType,
        IModelIdentifierHelper helper)
    {
        // Views are treated as read-only: only SELECT is implemented.
        var isView = table.IsView;
        var updateColumns = columns
            .Where(c => !c.Column.IsPrimaryKey)
            .ToArray();

        builder.AppendLine(
            $"public partial class {entityClassName} : DbEntity<{rowClassName}, {className}, {keyType}>");
        builder.AppendLine("{");
        builder.AppendLine($"    private readonly {contextName} ctx;");
        builder.AppendLine();

        builder.AppendLine($"    public {entityClassName}({contextName} ctx)");
        builder.AppendLine("    {");
        builder.AppendLine("        this.ctx = ctx;");
        builder.AppendLine("    }");
        builder.AppendLine();

        WriteDelete(builder, className, rowClassName, primaryKey, keyType, isView);
        builder.AppendLine();
        WriteInsert(builder, className, rowClassName, primaryKey, columns, isView);
        builder.AppendLine();
        WriteSelect(builder, className, rowClassName);
        builder.AppendLine();
        WriteUpdate(builder, className, rowClassName, primaryKey, updateColumns, helper, columns, isView);

        builder.AppendLine("}");
    }

    private static void WriteDelete(
            StringBuilder builder,
            string className,
            string rowClassName,
            ColumnModel? primaryKey,
            string keyType,
            bool isView = false)
    {
        var hasPrimaryKey = primaryKey is not null && !isView;
        var throwMessage = isView
            ? "throw new NotImplementedException(\"DeleteAsync is not supported: the object is a view and is read-only.\");"
            : "throw new NotImplementedException();";

        builder.AppendLine($"    public override Task<int> DeleteAsync({rowClassName} row, CancellationToken ct = default)");
        builder.AppendLine("    {");
        builder.AppendLine(hasPrimaryKey
            ? $"        return ctx.DeleteByIdAsync<{className}, {keyType}>(row.Key, ct);"
            : $"        {throwMessage}");
        builder.AppendLine("    }");
        builder.AppendLine();

        builder.AppendLine($"    public override Task<int> DeleteAsync(List<{rowClassName}> rows, CancellationToken ct = default)");
        builder.AppendLine("    {");
        if (hasPrimaryKey)
        {
            builder.AppendLine("        if (rows == null || rows.Count == 0)");
            builder.AppendLine("        {");
            builder.AppendLine("            return Task.FromResult(0);");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine(
                $"        return ctx.DeleteByIdsAsync<{className}, {keyType}>(rows.Select(item => item.Key), ct);");
        }
        else
        {
            builder.AppendLine($"        {throwMessage}");
        }

        builder.AppendLine("    }");
    }

    private static void WriteInsert(
        StringBuilder builder,
        string className,
        string rowClassName,
        ColumnModel? primaryKey,
        IReadOnlyList<ColumnModel> columns,
        bool isView = false)
    {
        var hasPrimaryKey = primaryKey is not null && !isView;

        var allColumns = columns.ToArray();
        var columnsWithoutPk = columns
            .Where(c => !c.Column.IsPrimaryKey)
            .ToArray();

        builder.AppendLine(
            isView
                ? $"    public override Task<int> InsertAsync({rowClassName} row, InsertOptions options = InsertOptions.SkipPrimaryKey, CancellationToken ct = default)"
                : $"    public override async Task<int> InsertAsync({rowClassName} row, InsertOptions options = InsertOptions.SkipPrimaryKey, CancellationToken ct = default)");
        builder.AppendLine("    {");
        if (columns.Count == 0 || isView)
        {
            builder.AppendLine(isView
                ? "        throw new NotImplementedException(\"InsertAsync is not supported: the object is a view and is read-only.\");"
                : "        throw new NotImplementedException();");
        }
        else
        {
            var allColumnsExpr = BuildColumnsExpression(allColumns);
            var withoutPkColumnsExpr = BuildColumnsExpression(columnsWithoutPk);
            var allValues = string.Join(", ", allColumns.Select(c => "@" + c.RowName));
            var withoutPkValues = string.Join(", ", columnsWithoutPk.Select(c => "@" + c.RowName));
            var allParameters = string.Join(", ", allColumns.Select(c => c.RowName + " = row." + c.RowName));
            var withoutPkParameters = string.Join(", ", columnsWithoutPk.Select(c => c.RowName + " = row." + c.RowName));

            builder.AppendLine("        if (row == null)");
            builder.AppendLine("        {");
            builder.AppendLine("            return 0;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        var skipPrimaryKey = options == InsertOptions.SkipPrimaryKey || options == InsertOptions.LoadAndSkip;");
            builder.AppendLine("        var loadRecord = options == InsertOptions.LoadRecord || options == InsertOptions.LoadAndSkip;");
            builder.AppendLine();
            if (!hasPrimaryKey)
            {
                builder.AppendLine("        if (loadRecord)");
                builder.AppendLine("        {");
                builder.AppendLine("            throw new NotImplementedException(\"Cannot load inserted row: table has no primary key.\");");
                builder.AppendLine("        }");
                builder.AppendLine();
            }
            if (hasPrimaryKey)
            {
                builder.AppendLine("        if (loadRecord)");
                builder.AppendLine("        {");
                builder.AppendLine($"            var loaded = await ctx.BuildQuery<{className}>((q, e) => q");
                builder.AppendLine("                .Config(() => e.DisableAlias = true)");
                builder.AppendLine("                .Append($\"INSERT INTO {e}\")");
                builder.AppendLine($"                .Append($\"({{(skipPrimaryKey ? {withoutPkColumnsExpr} : {allColumnsExpr})}})\")");
                builder.AppendLine("                .Append(\"OUTPUT INSERTED.*\")");
                builder.AppendLine($"                .Append(\"VALUES ( {{(skipPrimaryKey ? $\"{withoutPkValues}\" : $\"{allValues}\")}} )\")");
                builder.AppendLine("                .SetParameters(skipPrimaryKey");
                builder.AppendLine($"                    ? (object)new {{ {withoutPkParameters} }}");
                builder.AppendLine($"                    : new {{ {allParameters} }})");
                builder.AppendLine($"                ).ExecuteAsync<{rowClassName}>(ct).ConfigureAwait(false);");
                builder.AppendLine();
                builder.AppendLine("            if (loaded.Count > 0)");
                builder.AppendLine("            {");
                builder.AppendLine("                var source = loaded[0];");
                WriteFieldCopies(builder, "row", "source", columns);
                builder.AppendLine("            }");
                builder.AppendLine();
                builder.AppendLine("            return loaded.Count;");
                builder.AppendLine("        }");
                builder.AppendLine();
            }
            builder.AppendLine($"        var affected = await ctx.InsertAsync<{className}>(e => new InsertModel");
            builder.AppendLine("        {");
            builder.AppendLine($"            Columns = skipPrimaryKey ? {withoutPkColumnsExpr} : {allColumnsExpr},");
            builder.AppendLine($"            Values = skipPrimaryKey ? $\"{withoutPkValues}\" : $\"{allValues}\",");
            builder.AppendLine("            Parameters = skipPrimaryKey");
            builder.AppendLine($"                ? (object)new {{ {withoutPkParameters} }}");
            builder.AppendLine($"                    : new {{ {allParameters} }}");
            builder.AppendLine("        }, ct).ConfigureAwait(false);");
            builder.AppendLine();
            builder.AppendLine("        return affected;");
        }

        builder.AppendLine("    }");
        builder.AppendLine();

        builder.AppendLine(
            isView
                ? $"    public override Task<int> InsertAsync(List<{rowClassName}> rows, InsertOptions options = InsertOptions.SkipPrimaryKey, CancellationToken ct = default)"
                : $"    public override async Task<int> InsertAsync(List<{rowClassName}> rows, InsertOptions options = InsertOptions.SkipPrimaryKey, CancellationToken ct = default)");
        builder.AppendLine("    {");
        if (columns.Count == 0 || isView)
        {
            builder.AppendLine(isView
                ? "        throw new NotImplementedException(\"InsertAsync is not supported: the object is a view and is read-only.\");"
                : "        throw new NotImplementedException();");
        }
        else
        {
            builder.AppendLine("        if (rows == null || rows.Count == 0)");
            builder.AppendLine("        {");
            builder.AppendLine("            return 0;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        var skipPrimaryKey = options == InsertOptions.SkipPrimaryKey || options == InsertOptions.LoadAndSkip;");
            builder.AppendLine("        var loadRecord = options == InsertOptions.LoadRecord || options == InsertOptions.LoadAndSkip;");
            builder.AppendLine();
            if (!hasPrimaryKey)
            {
                builder.AppendLine("        if (loadRecord)");
                builder.AppendLine("        {");
                builder.AppendLine("            throw new NotImplementedException(\"Cannot load inserted rows: table has no primary key.\");");
                builder.AppendLine("        }");
                builder.AppendLine();
            }

            builder.AppendLine("        var parameters = new Dictionary<string, object?>();");
            builder.AppendLine("        var tuples = new List<string>();");
            builder.AppendLine("        var index = 1;");
            builder.AppendLine();
            builder.AppendLine("        foreach (var item in rows)");
            builder.AppendLine("        {");
            builder.AppendLine("            var placeholders = new List<string>();");
            builder.AppendLine();
            if (columnsWithoutPk.Length > 0)
            {
                builder.AppendLine("            if (skipPrimaryKey)");
                builder.AppendLine("            {");
                for (var i = 0; i < columnsWithoutPk.Length; i++)
                {
                    var c = columnsWithoutPk[i];
                    builder.AppendLine($"                var p{i} = \"@p\" + index++;");
                    builder.AppendLine($"                parameters[p{i}] = item.{c.RowName};");
                    builder.AppendLine($"                placeholders.Add(p{i});");
                }
                builder.AppendLine("            }");
            }

            if (allColumns.Length > 0)
            {
                builder.AppendLine(columnsWithoutPk.Length > 0 ? "            else" : "            if (!skipPrimaryKey)");
                builder.AppendLine("            {");
                for (var i = 0; i < allColumns.Length; i++)
                {
                    var c = allColumns[i];
                    builder.AppendLine($"                var p{i} = \"@p\" + index++;");
                    builder.AppendLine($"                parameters[p{i}] = item.{c.RowName};");
                    builder.AppendLine($"                placeholders.Add(p{i});");
                }
                builder.AppendLine("            }");
            }

            builder.AppendLine();
            builder.AppendLine("            tuples.Add(\"(\" + string.Join(\", \", placeholders) + \")\");");
            builder.AppendLine("        }");
            builder.AppendLine();

            var allColumnsLambdaExpr = string.Join(", ", allColumns.Select(c => "{e." + c.StructureName + "}"));
            var withoutPkColumnsLambdaExpr = string.Join(", ", columnsWithoutPk.Select(c => "{e." + c.StructureName + "}"));

            if (hasPrimaryKey)
            {
                builder.AppendLine("        if (loadRecord)");
                builder.AppendLine("        {");
                builder.AppendLine($"            var loaded = await ctx.BuildQuery<{className}>((q, e) => q");
                builder.AppendLine("                .Config(() => e.DisableAlias = true)");
                builder.AppendLine("                .Append($\"INSERT INTO {e}\")");
                builder.AppendLine($"                .Append($\"({{(skipPrimaryKey ? $\"{withoutPkColumnsLambdaExpr}\" : $\"{allColumnsLambdaExpr}\")}})\")");
                builder.AppendLine("                .Append(\"OUTPUT INSERTED.*\")");
                builder.AppendLine("                .Append(\"VALUES \" + string.Join(\", \", tuples))");
                builder.AppendLine("                .SetParameters(parameters)");
                builder.AppendLine($"                ).ExecuteAsync<{rowClassName}>(ct).ConfigureAwait(false);");
                builder.AppendLine();
                builder.AppendLine("            for (var i = 0; i < rows.Count && i < loaded.Count; i++)");
                builder.AppendLine("            {");
                builder.AppendLine("                var source = loaded[i];");
                builder.AppendLine("                var row = rows[i];");
                foreach (var column in columns)
                {
                    builder.AppendLine($"                row.{column.RowName} = source.{column.RowName};");
                }
                builder.AppendLine("            }");
                builder.AppendLine();
                builder.AppendLine("            return loaded.Count;");
                builder.AppendLine("        }");
                builder.AppendLine();
            }

            builder.AppendLine($"        return await ctx.BuildQuery<{className}>((q, e) => q");
            builder.AppendLine("            .Config(() => e.DisableAlias = true)");
            builder.AppendLine("            .Append($\"INSERT INTO {e}\")");
            builder.AppendLine($"            .Append($\"({{(skipPrimaryKey ? $\"{withoutPkColumnsLambdaExpr}\" : $\"{allColumnsLambdaExpr}\")}})\")");
            builder.AppendLine("            .Append(\"VALUES \" + string.Join(\", \", tuples))");
            builder.AppendLine("            .SetParameters(parameters)");
            builder.AppendLine("            ).ExecuteNonQueryAsync(ct).ConfigureAwait(false);");
        }

        builder.AppendLine("    }");
    }

    private static void WriteSelect(
        StringBuilder builder,
        string className,
        string rowClassName)
    {
        builder.AppendLine($"    public override Task<List<{rowClassName}>> SelectAsync(Func<{className}, string> predicate, object parameters = null, CancellationToken ct = default)");
        builder.AppendLine("    {");
        builder.AppendLine("        var condition = \"\";");
        builder.AppendLine();
        builder.AppendLine($"        return ctx.BuildQuery<{className}>((q, e) => q");
        builder.AppendLine("            .Config(() => { condition = predicate(e); })");
        builder.AppendLine("            .Append($\"SELECT {e.Star}\")");
        builder.AppendLine("            .Append($\"FROM {e}\")");
        builder.AppendLine("            .AppendIf(!string.IsNullOrWhiteSpace(condition), $\"WHERE {condition}\")");
        builder.AppendLine("            )");
        builder.AppendLine("            .SetParameters(parameters)");
        builder.AppendLine($"            .ExecuteAsync<{rowClassName}>(ct);");
        builder.AppendLine("    }");
    }

    private static void WriteUpdate(
        StringBuilder builder,
        string className,
        string rowClassName,
        ColumnModel? primaryKey,
        IReadOnlyList<ColumnModel> updateColumns,
        IModelIdentifierHelper helper,
        IReadOnlyList<ColumnModel> columns,
        bool isView = false)
    {
        var canUpdate = primaryKey is not null && updateColumns.Count > 0 && !isView;
        var throwMessage = isView
            ? "throw new NotImplementedException(\"UpdateAsync is not supported: the object is a view and is read-only.\");"
            : "throw new NotImplementedException();";
        builder.AppendLine($"    public override async Task<int> UpdateAsync({rowClassName} row, bool reload = false, CancellationToken ct = default)");
        builder.AppendLine("    {");
        if (!canUpdate)
        {
            builder.AppendLine($"        {throwMessage}");
        }
        else
        {
            var pkStructure = primaryKey!.StructureName;
            var set = string.Join(", ", updateColumns.Select(c => "{e." + c.StructureName + "} = @" + c.RowName));
            var parameters = string.Join(", ", updateColumns.Select(c => c.RowName + " = row." + c.RowName));

            builder.AppendLine("        if (row == null)");
            builder.AppendLine("        {");
            builder.AppendLine("            return 0;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        if (reload)");
            builder.AppendLine("        {");
            builder.AppendLine($"            var loaded = await ctx.BuildQuery<{className}>((q, e) => q");
            builder.AppendLine("                .Config(() => e.DisableAlias = true)");
            builder.AppendLine("                .Append($\"UPDATE {e}\")");
            builder.AppendLine("                .Append($\"SET " + set + "\")");
            builder.AppendLine($"                .Append($\"WHERE {{e.{pkStructure}}} = @Key\")");
            builder.AppendLine("                .Append(\"OUTPUT INSERTED.*\")");
            builder.AppendLine($"                .SetParameters(new {{ {parameters}, Key = row.Key }})");
            builder.AppendLine($"                ).ExecuteAsync<{rowClassName}>(ct).ConfigureAwait(false);");
            builder.AppendLine();
            builder.AppendLine("            if (loaded.Count > 0)");
            builder.AppendLine("            {");
            builder.AppendLine("                var source = loaded[0];");
            WriteFieldCopies(builder, "row", "source", columns);
            builder.AppendLine("            }");
            builder.AppendLine();
            builder.AppendLine("            return loaded.Count;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine($"        return await ctx.UpdateAsync<{className}>(e => new UpdateModel");
            builder.AppendLine("            {");
            builder.AppendLine("                Set = $\"" + set + "\",");
            builder.AppendLine($"                Where = $\"{{e.{pkStructure}}} = @Key\",");
            builder.AppendLine($"                Parameters = new {{ {parameters}, Key = row.Key }}");
            builder.AppendLine("            }, ct).ConfigureAwait(false);");
        }

        builder.AppendLine("    }");
        builder.AppendLine();

        builder.AppendLine($"    public override async Task<int> UpdateAsync(List<{rowClassName}> rows, bool reload = false, CancellationToken ct = default)");
        builder.AppendLine("    {");
        if (!canUpdate)
        {
            builder.AppendLine($"        {throwMessage}");
        }
        else
        {
            var parametersPerRow = SourceColumns(primaryKey!, updateColumns).Count;

            builder.AppendLine("        if (rows == null || rows.Count == 0)");
            builder.AppendLine("        {");
            builder.AppendLine("            return 0;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine($"        var parametersPerStatement = ctx.Config?.MaxParametersPerStatement ?? {DefaultParametersLiteral};");
            builder.AppendLine($"        if (parametersPerStatement <= 0) parametersPerStatement = {DefaultParametersLiteral};");
            builder.AppendLine($"        var batchSize = Math.Max(1, parametersPerStatement / {parametersPerRow});");
            builder.AppendLine();
            builder.AppendLine("        var affected = 0;");
            builder.AppendLine("        for (var offset = 0; offset < rows.Count; offset += batchSize)");
            builder.AppendLine("        {");
            builder.AppendLine("            affected += await UpdateBatchAsync(rows, offset, Math.Min(batchSize, rows.Count - offset), reload, ct).ConfigureAwait(false);");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        return affected;");
        }

        builder.AppendLine("    }");

        if (canUpdate)
        {
            builder.AppendLine();
            WriteUpdateBatch(builder, className, rowClassName, primaryKey!, updateColumns, helper, columns);
        }
    }

    private static List<ColumnModel> SourceColumns(
        ColumnModel primaryKey,
        IReadOnlyList<ColumnModel> updateColumns)
    {
        var columns = new List<ColumnModel>(updateColumns.Count + 1) { primaryKey };
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

    private static void WriteUpdateBatch(
        StringBuilder builder,
        string className,
        string rowClassName,
        ColumnModel primaryKey,
        IReadOnlyList<ColumnModel> updateColumns,
        IModelIdentifierHelper helper,
        IReadOnlyList<ColumnModel> columns)
    {
        var sourceColumns = SourceColumns(primaryKey, updateColumns);
        var firstSelectParts = new List<string>(sourceColumns.Count);
        var nextSelectParts = new List<string>(sourceColumns.Count);
        var setParts = new List<string>(updateColumns.Count);
        for (var i = 0; i < sourceColumns.Count; i++)
        {
            var placeholder = "{" + "p" + i + "}";
            firstSelectParts.Add(placeholder + " AS " + helper.EscapeCsStringLiteralNoQuote(helper.WrapObjectName(sourceColumns[i].Column.Name)));
            nextSelectParts.Add(placeholder);
        }

        foreach (var column in updateColumns)
        {
            setParts.Add("{" + "e." + column.StructureName + "} = t2." + helper.EscapeCsStringLiteralNoQuote(helper.WrapObjectName(column.Column.Name)));
        }

        var onClause = "{" + "e." + primaryKey.StructureName + "} = t2." + helper.EscapeCsStringLiteralNoQuote(helper.WrapObjectName(primaryKey.Column.Name));

        builder.AppendLine($"    private async Task<int> UpdateBatchAsync(List<{rowClassName}> rows, int offset, int count, bool reload, CancellationToken ct)");
        builder.AppendLine("    {");
        builder.AppendLine("        var parameters = new Dictionary<string, object?>();");
        builder.AppendLine("        var selects = new List<string>();");
        builder.AppendLine("        var index = 1;");
        builder.AppendLine();
        builder.AppendLine("        for (var i = 0; i < count; i++)");
        builder.AppendLine("        {");
        builder.AppendLine("            var item = rows[offset + i];");
        for (var i = 0; i < sourceColumns.Count; i++)
        {
            builder.AppendLine($"            var p{i} = \"@p\" + (index++).ToString();");
        }

        builder.AppendLine();
        for (var i = 0; i < sourceColumns.Count; i++)
        {
            builder.AppendLine($"            parameters[p{i}] = item.{sourceColumns[i].RowName};");
        }

        builder.AppendLine();
        builder.AppendLine("            selects.Add(i == 0");
        builder.AppendLine("                ? $\"SELECT " + string.Join(", ", firstSelectParts) + "\"");
        builder.AppendLine("                : $\"SELECT " + string.Join(", ", nextSelectParts) + "\");");
        builder.AppendLine("        }");
        builder.AppendLine();
        builder.AppendLine("        var source = string.Join(\" UNION ALL \", selects);");
        builder.AppendLine();
        builder.AppendLine("        var query = ctx.BuildQuery<" + className + ">((q, e) => q");
        builder.AppendLine("            .Append($\"UPDATE {e}\")");
        builder.AppendLine("            .Append($\"JOIN ({source}) AS t2 ON " + onClause + "\")");
        builder.AppendLine("            .Append($\"SET " + string.Join(", ", setParts) + "\")");
        builder.AppendLine("            .SetParameters(parameters)");
        builder.AppendLine("            );");
        builder.AppendLine();
        builder.AppendLine("        if (reload)");
        builder.AppendLine("        {");
        builder.AppendLine("            var loaded = await query.Append(\"OUTPUT INSERTED.*\").ExecuteAsync<" + rowClassName + ">(ct).ConfigureAwait(false);");
        builder.AppendLine("            for (var i = 0; i < count && i < loaded.Count; i++)");
        builder.AppendLine("            {");
        builder.AppendLine("                var source2 = loaded[i];");
        builder.AppendLine("                var item = rows[offset + i];");
        WriteFieldCopies(builder, "item", "source2", columns);
        builder.AppendLine("            }");
        builder.AppendLine("            return loaded.Count;");
        builder.AppendLine("        }");
        builder.AppendLine();
        builder.AppendLine("        return await query.ExecuteNonQueryAsync(ct).ConfigureAwait(false);");
        builder.AppendLine("    }");
    }

    private static string BuildQuatedName(TableInfo table, IModelIdentifierHelper helper)
    {
        var quotedTable = helper.WrapObjectName(table.Name);
        var quotedSchema = string.IsNullOrEmpty(table.Schema)
            ? string.Empty
            : helper.WrapObjectName(table.Schema) + ".";

        return quotedSchema + quotedTable;
    }

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
}