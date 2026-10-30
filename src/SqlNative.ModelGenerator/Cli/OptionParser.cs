using SqlNative.ModelGenerator.Engines;

namespace SqlNative.ModelGenerator.Cli;

internal static class OptionParser
{
    private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["cs"] = "cs",
        ["connectionstring"] = "cs",
        ["conn"] = "cs",
        ["engine"] = "engine",
        ["database"] = "engine",
        ["db"] = "engine",
        ["pg"] = "engine",
        ["postgres"] = "engine",
        ["folder"] = "folder",
        ["out"] = "folder",
        ["output"] = "folder",
        ["package"] = "package",
        ["namespace"] = "package",
        ["ns"] = "package",
        ["context"] = "context",
        ["ctx"] = "context",
        ["include-tables"] = "include-tables",
        ["include"] = "include-tables",
        ["exclude-tables"] = "exclude-tables",
        ["exclude"] = "exclude-tables",
        ["help"] = "help",
        ["h"] = "help",
        ["?"] = "help",
    };

    private static readonly HashSet<string> ValuelessOptions = new(StringComparer.OrdinalIgnoreCase) { "help" };

    // Options that may repeat and accumulate values, e.g. --include-tables emp --include-tables dept.
    private static readonly HashSet<string> RepeatableOptions = new(StringComparer.OrdinalIgnoreCase) { "include-tables", "exclude-tables" };

    private static readonly HashSet<string> RequiredOptions = new(StringComparer.Ordinal) { "cs", "engine", "folder", "package" };

    public static ParseResult Parse(string[] args)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var repeatableValues = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (!arg.StartsWith("--", StringComparison.Ordinal))
            {
                return Fail($"Unexpected argument '{arg}'. All options must start with '--'. Run with --help to see usage.");
            }

            var body = arg[2..];
            var separatorIndex = body.IndexOf('=');
            string key;
            string value;
            if (separatorIndex >= 0)
            {
                key = body[..separatorIndex].Trim();
                value = body[(separatorIndex + 1)..].Trim();
                if (value.Length == 0 && !ValuelessOptions.Contains(key))
                {
                    return Fail($"Option '--{key}' has an empty value.");
                }
            }
            else
            {
                key = body.Trim();
                if (ValuelessOptions.Contains(key))
                {
                    value = "true";
                }
                else if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
                {
                    value = args[++i];
                }
                else
                {
                    return Fail($"Option '--{key}' is missing a value. Example: --{key} <value>.");
                }
            }

            if (!Aliases.TryGetValue(key, out var canonical))
            {
                return Fail($"Unknown option '--{key}'. Run with --help to see the available options.");
            }

            if (RepeatableOptions.Contains(canonical))
            {
                if (!repeatableValues.TryGetValue(canonical, out var collected))
                {
                    collected = new List<string>();
                    repeatableValues[canonical] = collected;
                }

                collected.AddRange(SplitTablePatterns(value));
                if (collected.Count == 0)
                {
                    return Fail($"Option '--{key}' has an empty value.");
                }
            }
            else
            {
                values[canonical] = value;
            }
        }

        if (values.ContainsKey("help"))
        {
            return new ParseResult(ParseStatus.Help);
        }

        foreach (var required in RequiredOptions)
        {
            if (!values.ContainsKey(required))
            {
                return Fail($"Missing required option '--{required}'. Run with --help to see usage.");
            }
        }

        var engine = EngineFactory.Normalize(values["engine"]);
        if (!EngineFactory.SupportedEngines.Contains(engine))
        {
            return Fail(
                $"'{values["engine"]}' is not a valid engine. Valid values are: {string.Join(", ", EngineFactory.SupportedEngines)}.");
        }

        return new ParseResult(
            ParseStatus.Ok,
            new GeneratorOptions
            {
                ConnectionString = values["cs"],
                Engine = engine,
                Folder = values["folder"],
                Package = values["package"],
                ContextName = values.TryGetValue("context", out var context) && context.Length > 0 ? context : null,
                IncludeTables = repeatableValues.TryGetValue("include-tables", out var includeTables)
                    ? includeTables
                    : [],
                ExcludeTables = repeatableValues.TryGetValue("exclude-tables", out var excludeTables)
                    ? excludeTables
                    : [],
            });
    }

    private static ParseResult Fail(string message) => new(ParseStatus.Error, Error: message);

    private static List<string> SplitTablePatterns(string value)
    {
        return value
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Where(p => p.Length > 0)
            .ToList();
    }
}
