namespace SqlNative.ModelGenerator.Cli;

internal enum ParseStatus
{
    Ok,
    Help,
    Error,
}

internal sealed record ParseResult(ParseStatus Status, GeneratorOptions? Options = null, string? Error = null);

internal sealed record GeneratorOptions
{
    public string ConnectionString { get; init; } = string.Empty;

    public string Engine { get; init; } = string.Empty;

    public string Folder { get; init; } = string.Empty;

    public string Package { get; init; } = string.Empty;

    public string? ContextName { get; init; }

    /// <summary>Table name patterns to generate models for. Empty means "all tables".</summary>
    public IReadOnlyList<string> IncludeTables { get; init; } = [];

    /// <summary>Table name patterns to skip. Applied after <see cref="IncludeTables"/>.</summary>
    public IReadOnlyList<string> ExcludeTables { get; init; } = [];
}
