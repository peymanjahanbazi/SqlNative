using SqlNative.ModelGenerator.Generation.MsSql;
using SqlNative.ModelGenerator.Generation.MySql;
using SqlNative.ModelGenerator.Generation.Oracle;
using SqlNative.ModelGenerator.Generation.Postgres;

namespace SqlNative.ModelGenerator.Generation;

internal static class GenerationFactory
{
    public static readonly IReadOnlyList<string> SupportedEngines = ["mysql", "postgresql", "mssql", "oracle"];

    public static IGenerationService Create(string engine)
    {
        var normalized = engine.Trim().ToLowerInvariant();
        return normalized switch
        {
            "mysql" => new MySqlGenerationService(),
            "postgresql" or "postgres" or "pg" => new PostgresGenerationService(),
            "mssql" or "sqlserver" or "sql-server" => new MsSqlGenerationService(),
            "oracle" or "ora" or "oradb" => new OracleGenerationService(),
            _ => throw new InvalidOperationException(
                $"Unsupported engine '{engine}'. Use --engine mysql|mssql|postgresql|oracle."),
        };
    }
}