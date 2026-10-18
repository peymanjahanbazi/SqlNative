using SqlNative.ModelGenerator.Schema;
using MySqlConnector;

namespace SqlNative.ModelGenerator.Engines;

internal sealed partial class MySqlEngine : IDatabaseEngine
{
    public string Name => "mysql";

    public async Task<DatabaseSchema> ReadSchemaAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var databaseName = await ReadDatabaseNameAsync(connection, cancellationToken);

        var tableNames = new List<string>();
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = "SHOW TABLES;";
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                tableNames.Add(reader.GetString(0));
            }
        }

        var tables = new List<TableInfo>(tableNames.Count);
        foreach (var tableName in tableNames)
        {
            tables.Add(await DescribeTableAsync(connection, tableName, cancellationToken));
        }

        return new DatabaseSchema(databaseName, tables);
    }

    private static async Task<string> ReadDatabaseNameAsync(MySqlConnection connection, CancellationToken cancellationToken)
    {
        if (TryGetDatabaseFromConnectionString(connection.ConnectionString) is { } fromConnectionString)
        {
            return fromConnectionString;
        }

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT DATABASE();";
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result as string ?? string.Empty;
    }

    private static string? TryGetDatabaseFromConnectionString(string connectionString)
    {
        var builder = new MySqlConnectionStringBuilder(connectionString);
        return string.IsNullOrWhiteSpace(builder.Database) ? null : builder.Database;
    }

    private static async Task<TableInfo> DescribeTableAsync(MySqlConnection connection, string tableName, CancellationToken cancellationToken)
    {
        var columns = new List<ColumnInfo>();

        await using var command = connection.CreateCommand();
        command.CommandText = $"DESCRIBE `{tableName.Replace("`", "``")}`;";
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            columns.Add(new ColumnInfo(
                Name: reader.GetString(0),
                RawType: reader.GetString(1),
                IsNullable: string.Equals(reader.GetString(2), "YES", StringComparison.OrdinalIgnoreCase),
                IsPrimaryKey: string.Equals(reader.GetString(3), "PRI", StringComparison.OrdinalIgnoreCase),
                IsAutoIncrement: GetSafeString(reader, 5).Contains("auto_increment", StringComparison.OrdinalIgnoreCase)));
        }

        return new TableInfo(Schema: null, Name: tableName, Columns: columns);
    }

    private static string GetSafeString(System.Data.Common.DbDataReader reader, int ordinal)
        => reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
}
