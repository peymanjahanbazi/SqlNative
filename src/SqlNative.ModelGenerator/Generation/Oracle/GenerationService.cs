using SqlNative.ModelGenerator.Cli;
using SqlNative.ModelGenerator.Generation;
using SqlNative.ModelGenerator.Schema;
using SqlNative.Core;

namespace SqlNative.ModelGenerator.Generation.Oracle;

internal sealed class OracleGenerationService : IGenerationService
{
    private readonly IModelIdentifierHelper _helper = new OracleIdentifierAdapter();
    private readonly IContextWriter _contextWriter = new OracleContextWriter();
    private readonly IModelWriter _modelWriter;

    public OracleGenerationService()
    {
        _modelWriter = new OracleModelWriter(_helper);
    }

    public async Task<GenerationResult> GenerateAsync(GeneratorOptions options, DatabaseSchema schema)
    {
        var outputFolder = Path.GetFullPath(options.Folder);
        Directory.CreateDirectory(outputFolder);

        var usedClassNames = new HashSet<string>(StringComparer.Ordinal);
        var tables = schema.Tables
            .OrderBy(t => t.Schema ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ThenBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
            .Select(table => (Table: table, ClassName: _helper.EnsureUnique(_helper.GetObjectNameInCs(GetClassNameSource(table)), usedClassNames)))
            .ToArray();

        var contextName = options.ContextName is null
            ? _helper.GetObjectNameInCs(schema.DatabaseName) + "Context"
            : _helper.GetObjectNameInCs(options.ContextName);

        if (usedClassNames.Contains(contextName))
        {
            throw new InvalidOperationException(
                $"The context name '{contextName}' conflicts with a generated table class name in namespace '{options.Package}'. Choose another name with --context.");
        }

        var usedEntityClassNames = new HashSet<string>(usedClassNames, StringComparer.Ordinal);
        var generated = new (string ClassName, string EntityClassName)[tables.Length];
        for (var i = 0; i < tables.Length; i++)
        {
            var className = tables[i].ClassName;
            var entityClassName = _helper.EnsureUnique(className + "Entity", usedEntityClassNames);
            generated[i] = (className, entityClassName);
        }

        var writtenFiles = new List<string>(tables.Length + 1);
        for (var i = 0; i < tables.Length; i++)
        {
            var filePath = Path.Combine(outputFolder, generated[i].ClassName + ".cs");
            await File.WriteAllTextAsync(
                filePath,
                _modelWriter.Generate(
                    tables[i].Table,
                    generated[i].ClassName,
                    generated[i].EntityClassName,
                    contextName,
                    options.Package));
            writtenFiles.Add(filePath);
        }

        var contextFilePath = Path.Combine(outputFolder, contextName + ".cs");
        await File.WriteAllTextAsync(
            contextFilePath,
            _contextWriter.Generate(generated, contextName, options.Package));
        writtenFiles.Add(contextFilePath);

        return new GenerationResult(tables.Length, contextName, outputFolder, writtenFiles);
    }

    private static string GetClassNameSource(TableInfo table)
    {
        // Oracle has schemas, but the table name is sufficient for the class name.
        return table.Name;
    }

    private sealed class OracleIdentifierAdapter : IModelIdentifierHelper
    {
        public string GetObjectNameInCs(string n) => OracleIdentifierHelper.GetObjectNameInCs(n);
        public string EnsureUnique(string c, ISet<string> u) => OracleIdentifierHelper.EnsureUnique(c, u);
        public string EscapeCsStringLiteral(string v) => OracleIdentifierHelper.EscapeCsStringLiteral(v);
        public string EscapeCsStringLiteralNoQuote(string v) => OracleIdentifierHelper.EscapeCsStringLiteralNoQuote(v);
        public bool NeedsBackticks(string n) => OracleIdentifierHelper.NeedsDoubleQuotes(n);
        public string WrapObjectName(string n) => OracleIdentifierHelper.WrapObjectName(n);
    }
}