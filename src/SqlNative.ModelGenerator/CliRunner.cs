using SqlNative.ModelGenerator.Cli;
using SqlNative.ModelGenerator.Engines;
using SqlNative.ModelGenerator.Generation;

namespace SqlNative.ModelGenerator;

internal static class CliRunner
{
    public static async Task<int> RunAsync(string[] args)
    {
        var (inputArgs, inputFileError) = InputFileLoader.Load(args);
        if (inputFileError is not null)
        {
            Console.Error.WriteLine($"Error: {inputFileError}");
            Console.Error.WriteLine("Run 'SqlNative.ModelGenerator --help' for usage.");
            return 1;
        }

        var parseResult = OptionParser.Parse(inputArgs);
        switch (parseResult.Status)
        {
            case ParseStatus.Help:
                Console.WriteLine(HelpText.Content);
                return 0;
            case ParseStatus.Error:
                Console.Error.WriteLine($"Error: {parseResult.Error}");
                Console.Error.WriteLine("Run 'SqlNative.ModelGenerator --help' for usage.");
                return 1;
        }

        var options = parseResult.Options!;

        try
        {
            var engine = EngineFactory.Create(options.Engine);
            Console.WriteLine($"Connecting to {engine.Name} database...");
            var schema = await engine.ReadSchemaAsync(options.ConnectionString);

            var tableFilter = TableFilter.FromOptions(options);
            if (tableFilter.IsActive)
            {
                var before = schema.Tables.Count;
                schema = tableFilter.Apply(schema);
                Console.WriteLine($"Table filter: {before} table(s) found, {schema.Tables.Count} match(es) after include/exclude.");
            }

            if (schema.Tables.Count == 0)
            {
                Console.WriteLine(tableFilter.IsActive
                    ? "Warning: no tables match the include/exclude options."
                    : "Warning: no tables were found in the database.");
            }

            var generator = GenerationFactory.Create(options.Engine);
            var result = await generator.GenerateAsync(options, schema);

            Console.WriteLine($"Database '{schema.DatabaseName}': generated {result.TableCount} table model(s) and context '{result.ContextName}'.");
            foreach (var file in result.WrittenFiles)
            {
                Console.WriteLine($"  {file}");
            }

            return 0;
        }
        catch (NotSupportedException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 2;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Generation failed: {ex.Message}");
            if (ex.InnerException is not null)
            {
                Console.Error.WriteLine(ex.InnerException.Message);
            }

            return 1;
        }
    }
}
