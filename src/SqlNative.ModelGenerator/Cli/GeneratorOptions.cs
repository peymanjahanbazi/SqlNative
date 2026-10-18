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
    public required string ConnectionString { get; init; }

    public required string Engine { get; init; }

    public required string Folder { get; init; }

    public required string Package { get; init; }

    public string? ContextName { get; init; }
}
