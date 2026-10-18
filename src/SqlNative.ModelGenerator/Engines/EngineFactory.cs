namespace SqlNative.ModelGenerator.Engines;

internal static class EngineFactory
{
    public static readonly IReadOnlyList<string> SupportedEngines = ["mysql", "mssql", "postgresql", "oracle"];

    public static IDatabaseEngine Create(string engine)
    {
        var normalized = Normalize(engine);

        return normalized switch
        {
            "mysql" => new MySqlEngine(),
            "postgresql" => new PostgresEngine(),
            "mssql" => new MsSqlEngine(),
            "oracle" => new OracleEngine(),
            _ => throw new ArgumentException(
                $"Unknown engine '{engine}'. Valid values: {string.Join(", ", SupportedEngines)}."),
        };
    }

    public static string Normalize(string engine)
        => engine.Trim().ToLowerInvariant() switch
        {
            "pg" or "postgres" => "postgresql",
            "sqlserver" or "sql-server" => "mssql",
            "ora" or "oradb" => "oracle",
            _ => engine.Trim().ToLowerInvariant(),
        };
}