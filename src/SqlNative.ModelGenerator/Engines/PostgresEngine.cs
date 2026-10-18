using SqlNative.ModelGenerator.Schema;
using Npgsql;

namespace SqlNative.ModelGenerator.Engines;

internal sealed class PostgresEngine : IDatabaseEngine
{
    public string Name => "postgresql";

    public async Task<DatabaseSchema> ReadSchemaAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var databaseName = ReadDatabaseFromConnectionString(connection.ConnectionString)
            ?? connection.Database;

        var tableEntries = new List<(string Schema, string Name)>();
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = """
                SELECT table_schema, table_name
                FROM information_schema.tables
                WHERE table_type = 'BASE TABLE'
                  AND table_schema NOT IN ('pg_catalog', 'information_schema')
                ORDER BY table_schema, table_name;
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
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        return string.IsNullOrWhiteSpace(builder.Database) ? null : builder.Database;
    }

    private static async Task<TableInfo> DescribeTableAsync(NpgsqlConnection connection, string schema, string tableName, CancellationToken cancellationToken)
    {
        var columns = new List<ColumnInfo>();
        HashSet<string> primaryKeys = new(StringComparer.OrdinalIgnoreCase);
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = """
                SELECT c.column_name,
                       c.udt_name,
                       c.is_nullable,
                       c.is_identity,
                       EXISTS (
                           SELECT 1
                           FROM information_schema.table_constraints tc
                           JOIN information_schema.key_column_usage kcu
                             ON tc.constraint_name = kcu.constraint_name
                            AND tc.table_schema = kcu.table_schema
                           WHERE tc.constraint_type = 'PRIMARY KEY'
                             AND tc.table_schema = @schema
                             AND tc.table_name = @table
                             AND kcu.column_name = c.column_name
                       ) AS is_primary_key
                FROM information_schema.columns c
                WHERE c.table_schema = @schema
                  AND c.table_name = @table
                ORDER BY c.ordinal_position;
                """;
            AddParameter(command, "@schema", schema);
            AddParameter(command, "@table", tableName);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var columnName = reader.GetString(0);
                var rawType = reader.GetString(1);
                var isNullable = string.Equals(reader.GetString(2), "YES", StringComparison.OrdinalIgnoreCase);
                var isIdentity = string.Equals(reader.GetString(3), "YES", StringComparison.OrdinalIgnoreCase);
                var isPrimaryKey = reader.GetBoolean(4);

                columns.Add(new ColumnInfo(
                    Name: columnName,
                    RawType: rawType,
                    IsNullable: isNullable,
                    IsPrimaryKey: isPrimaryKey,
                    IsAutoIncrement: isIdentity));

                if (isPrimaryKey)
                {
                    primaryKeys.Add(columnName);
                }
            }
        }

        return new TableInfo(Schema: schema, Name: tableName, Columns: columns);
    }

    private static void AddParameter(NpgsqlCommand command, string name, string value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}