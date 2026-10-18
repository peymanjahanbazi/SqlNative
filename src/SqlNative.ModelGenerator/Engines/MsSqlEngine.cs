using Microsoft.Data.SqlClient;
using SqlNative.ModelGenerator.Schema;

namespace SqlNative.ModelGenerator.Engines;

internal sealed class MsSqlEngine : IDatabaseEngine
{
    public string Name => "mssql";

    public async Task<DatabaseSchema> ReadSchemaAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var databaseName = ReadDatabaseFromConnectionString(connection.ConnectionString)
            ?? connection.Database;

        var tableEntries = new List<(string Schema, string Name)>();
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = """
                SELECT s.name AS schema_name, t.name AS table_name
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE t.type = 'U'
                  AND t.is_ms_shipped = 0
                ORDER BY s.name, t.name;
                """;
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                tableEntries.Add((reader.GetString(0), reader.GetString(1)));
            }
        }

        var tables = new List<TableInfo>(tableEntries.Count);
        foreach (var (schema, tableName) in tableEntries)
        {
            tables.Add(await DescribeTableAsync(connection, schema, tableName, cancellationToken));
        }

        return new DatabaseSchema(databaseName, tables);
    }

    private static string? ReadDatabaseFromConnectionString(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        return string.IsNullOrWhiteSpace(builder.InitialCatalog) ? null : builder.InitialCatalog;
    }

    private static async Task<TableInfo> DescribeTableAsync(
        SqlConnection connection,
        string schema,
        string tableName,
        CancellationToken cancellationToken)
    {
        var columns = new List<ColumnInfo>();
        var primaryKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        await using (var pkCommand = connection.CreateCommand())
        {
            pkCommand.CommandText = """
                SELECT c.name AS column_name
                FROM sys.indexes i
                INNER JOIN sys.index_columns ic
                    ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                INNER JOIN sys.columns c
                    ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                WHERE i.is_primary_key = 1
                  AND i.object_id = OBJECT_ID(@fullName);
                """;
            var schemaQualified = $"[{schema}].[{tableName}]";
            AddParameter(pkCommand, "@fullName", schemaQualified);

            await using var pkReader = await pkCommand.ExecuteReaderAsync(cancellationToken);
            while (await pkReader.ReadAsync(cancellationToken))
            {
                primaryKeys.Add(pkReader.GetString(0));
            }
        }

        await using (var command = connection.CreateCommand())
        {
            command.CommandText = """
                SELECT c.name AS column_name,
                       t.name AS type_name,
                       c.is_nullable,
                       c.is_identity,
                       c.is_computed
                FROM sys.columns c
                INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
                WHERE c.object_id = OBJECT_ID(@fullName)
                ORDER BY c.column_id;
                """;
            AddParameter(command, "@fullName", $"[{schema}].[{tableName}]");

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var columnName = reader.GetString(0);
                var rawType = reader.GetString(1);
                var isNullable = reader.GetBoolean(2);
                var isIdentity = reader.GetBoolean(3);
                var isComputed = reader.GetBoolean(4);
                var isPrimaryKey = primaryKeys.Contains(columnName);

                columns.Add(new ColumnInfo(
                    Name: columnName,
                    RawType: rawType,
                    IsNullable: isNullable && !isComputed,
                    IsPrimaryKey: isPrimaryKey,
                    IsAutoIncrement: isIdentity));
            }
        }

        return new TableInfo(Schema: schema, Name: tableName, Columns: columns);
    }

    private static void AddParameter(SqlCommand command, string name, string value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}