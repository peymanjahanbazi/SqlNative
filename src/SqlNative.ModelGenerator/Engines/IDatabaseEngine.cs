using SqlNative.ModelGenerator.Schema;

namespace SqlNative.ModelGenerator.Engines;

internal interface IDatabaseEngine
{
    string Name { get; }

    Task<DatabaseSchema> ReadSchemaAsync(string connectionString, CancellationToken cancellationToken = default);
}
