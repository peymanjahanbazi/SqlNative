using SqlNative.ModelGenerator.Cli;
using SqlNative.ModelGenerator.Schema;

namespace SqlNative.ModelGenerator.Generation;

internal sealed record GenerationResult(
    int TableCount,
    string ContextName,
    string OutputFolder,
    IReadOnlyList<string> WrittenFiles);

internal interface IGenerationService
{
    Task<GenerationResult> GenerateAsync(GeneratorOptions options, DatabaseSchema schema);
}