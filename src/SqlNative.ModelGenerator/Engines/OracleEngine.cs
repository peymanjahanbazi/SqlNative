using SqlNative.ModelGenerator.Schema;
using Oracle.ManagedDataAccess.Client;

namespace SqlNative.ModelGenerator.Engines;

internal sealed class OracleEngine : IDatabaseEngine
{
    public string Name => "oracle";

    public async Task<DatabaseSchema> ReadSchemaAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Oracle connection string is empty. Provide a value like 'User Id=scott;Password=tiger;Data Source=host:1521/service;' via --cs.");
        }

        OracleConnection connection;
        try
        {
            connection = new OracleConnection(connectionString);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException(
                $"Could not parse the Oracle connection string: {ex.Message}. " +
                "Expected keys include 'User Id' (or 'UserID'), 'Password', and 'Data Source'. " +
                "Example: 'User Id=scott;Password=tiger;Data Source=host:1521/service;'.", ex);
        }

        await using (connection)
        {
            try
            {
                await connection.OpenAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is OracleException
                                       || ex is System.Net.Sockets.SocketException
                                       || ex is TimeoutException)
            {
                throw new InvalidOperationException(
                    $"Could not open Oracle connection: {ex.Message}. " +
                    "Verify network reachability, the Data Source TNS entry / host:port/service, " +
                    "credentials, and that the database is accepting connections.", ex);
            }

            var schemaOwner = ResolveSchemaOwner(connection);
            if (string.IsNullOrWhiteSpace(schemaOwner))
            {
                throw new InvalidOperationException(
                    "Could not infer the Oracle schema owner from the connection string or session. " +
                    "Specify the schema explicitly via 'User Id=...' or connect as the target user.");
            }

            var tableEntries = new List<(string Schema, string Name)>();
            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT owner, table_name
                    FROM all_tables
                    WHERE owner = :owner
                      AND secondary = 'N'
                      AND nested = 'NO'
                      AND (iot_type IS NULL OR iot_type != 'IOT_OVERFLOW')
                    ORDER BY owner, table_name
                    """;
                AddBindParameter(command, "owner", schemaOwner);

                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    tableEntries.Add((reader.GetString(0), reader.GetString(1)));
                }
            }
            catch (OracleException ex)
            {
                throw new InvalidOperationException(
                    $"Oracle error while reading tables from schema '{schemaOwner}': {ex.Message}. " +
                    "Make sure the user has SELECT privilege on ALL_TABLES (or use the schema's own user).", ex);
            }

            var tables = new List<TableInfo>(tableEntries.Count);
            foreach (var (schema, tableName) in tableEntries)
            {
                tables.Add(await DescribeTableAsync(connection, schema, tableName, cancellationToken));
            }

            return new DatabaseSchema(schemaOwner, tables);
        }
    }

    private static string? ResolveSchemaOwner(OracleConnection connection)
    {
        try
        {
            var builder = new OracleConnectionStringBuilder(connection.ConnectionString);
            var userId = builder.UserID;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                return userId.ToUpperInvariant();
            }
        }
        catch
        {
            // Fall through to session lookup.
        }

        try
        {
            using var command = connection.CreateCommand();
            // USERENV('SCHEMAID') returns the original schema name; SYS_CONTEXT('USERENV','CURRENT_SCHEMA')
            // is the more portable alternative.
            command.CommandText = "SELECT SYS_CONTEXT('USERENV','CURRENT_SCHEMA') FROM dual";
            var result = command.ExecuteScalar();
            return (result as string ?? string.Empty).ToUpperInvariant();
        }
        catch
        {
            return null;
        }
    }

    private static async Task<TableInfo> DescribeTableAsync(
        OracleConnection connection,
        string schema,
        string tableName,
        CancellationToken cancellationToken)
    {
        var columns = new List<ColumnInfo>();
        var primaryKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            await using var pkCommand = connection.CreateCommand();
            pkCommand.CommandText = """
                SELECT acc.column_name
                FROM all_cons_columns acc
                JOIN all_constraints ac
                  ON acc.owner = ac.owner
                 AND acc.constraint_name = ac.constraint_name
                 AND acc.table_name = ac.table_name
                WHERE ac.owner = :owner
                  AND ac.table_name = :table_name
                  AND ac.constraint_type = 'P'
                """;
            AddBindParameter(pkCommand, "owner", schema);
            AddBindParameter(pkCommand, "table_name", tableName);

            await using var pkReader = await pkCommand.ExecuteReaderAsync(cancellationToken);
            while (await pkReader.ReadAsync(cancellationToken))
            {
                primaryKeys.Add(pkReader.GetString(0));
            }
        }
        catch (OracleException ex)
        {
            throw new InvalidOperationException(
                $"Oracle error while reading primary keys for {schema}.{tableName}: {ex.Message}.", ex);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT column_name,
                       data_type,
                       nullable
                FROM all_tab_columns
                WHERE owner = :owner
                  AND table_name = :table_name
                ORDER BY column_id
                """;
            AddBindParameter(command, "owner", schema);
            AddBindParameter(command, "table_name", tableName);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var columnName = reader.GetString(0);
                var rawType = reader.GetString(1);
                var nullableText = reader.GetString(2);

                var isNullable = string.Equals(nullableText, "Y", StringComparison.OrdinalIgnoreCase);
                var isPrimaryKey = primaryKeys.Contains(columnName);

                columns.Add(new ColumnInfo(
                    Name: columnName,
                    RawType: rawType,
                    IsNullable: isNullable,
                    IsPrimaryKey: isPrimaryKey,
                    IsAutoIncrement: false));
            }
        }
        catch (OracleException ex)
        {
            throw new InvalidOperationException(
                $"Oracle error while reading columns for {schema}.{tableName}: {ex.Message}.", ex);
        }

        return new TableInfo(Schema: schema, Name: tableName, Columns: columns);
    }

    private static void AddBindParameter(OracleCommand command, string name, string value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}