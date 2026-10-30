using System.Text.RegularExpressions;
using SqlNative.ModelGenerator.Schema;

namespace SqlNative.ModelGenerator.Cli;

/// <summary>
/// Filters the database tables according to the <c>--include-tables</c> and <c>--exclude-tables</c> options.
/// Patterns support the <c>_</c> wildcard, which matches any sequence of characters
/// (e.g. <c>e_</c> matches every table whose name starts with "e").
/// A pattern of the form <c>schema.table</c> matches the schema and table name together.
/// </summary>
internal sealed class TableFilter
{
    private readonly List<Regex> _includes;
    private readonly List<Regex> _excludes;

    private TableFilter(List<Regex> includes, List<Regex> excludes)
    {
        _includes = includes;
        _excludes = excludes;
    }

    public bool IsActive => _includes.Count > 0 || _excludes.Count > 0;

    public static TableFilter FromOptions(GeneratorOptions options)
    {
        return new TableFilter(
            [.. options.IncludeTables.Select(ToMatcher)],
            [.. options.ExcludeTables.Select(ToMatcher)]);
    }

    public DatabaseSchema Apply(DatabaseSchema schema)
    {
        if (!IsActive)
        {
            return schema;
        }

        var tables = schema.Tables
            .Where(keep)
            .ToArray();

        return schema with { Tables = tables };

        bool keep(TableInfo table)
        {
            if (_includes.Count > 0 && !_includes.Any(m => m.IsMatch(table.Name) || MatchesSchemaQualified(m, table)))
            {
                return false;
            }

            return !_excludes.Any(m => m.IsMatch(table.Name) || MatchesSchemaQualified(m, table));
        }
    }

    private static bool MatchesSchemaQualified(Regex matcher, TableInfo table)
    {
        return table.Schema is not null && matcher.IsMatch($"{table.Schema}.{table.Name}");
    }

    private static Regex ToMatcher(string pattern)
    {
        // The '_' wildcard matches any sequence of characters; everything else is taken literally.
        var regexPattern = Regex.Escape(pattern).Replace("_", ".*");
        return new Regex($"^{regexPattern}$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }
}
