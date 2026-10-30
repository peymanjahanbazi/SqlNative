using System.Text;

namespace SqlNative.ModelGenerator.Cli;

/// <summary>
/// Expands the <c>-i &lt;file&gt;</c> option ("read parameters from file"), producing a single
/// flat argument list for <see cref="OptionParser"/>. File content is tokenized the same way a
/// shell would: newlines and other whitespace separate options, and double or single quotes group
/// a value that contains spaces. e.g.
///
/// <code>
/// --cs "connection string"
/// --include-tables "ab,c"
/// </code>
///
/// is equivalent to <c>--cs "connection string" --include-tables "ab,c"</c>.
/// Command-line arguments are appended after the file ones, so they override file values for
/// single-occurrence options (connection string, engine, ...) while table filters from both
/// sources are combined.
/// </summary>
internal static class InputFileLoader
{
    private enum InputFlagKind
    {
        None,
        NextArg,  // -i params.txt
        Inline,   // -i=params.txt
    }

    private const int MaxNestingDepth = 10;

    public static (string[] Args, string? Error) Load(string[] args)
    {
        return Split(args, depth: 0);
    }

    private static (string[] Args, string? Error) Split(string[] args, int depth)
    {
        if (depth > MaxNestingDepth)
        {
            return (args, $"Input files are nested more than {MaxNestingDepth} levels deep.");
        }

        var fromFiles = new List<string>();
        var fromCommandLine = new List<string>();

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var flag = GetInputFlag(arg);
            if (flag == InputFlagKind.None)
            {
                fromCommandLine.Add(arg);
                continue;
            }

            string path;
            if (flag == InputFlagKind.Inline)
            {
                path = arg[(arg.IndexOf('=') + 1)..].Trim();
                if (path.Length == 0)
                {
                    return (args, "Option '-i' has an empty value.");
                }
            }
            else if (i + 1 < args.Length)
            {
                path = args[++i];
            }
            else
            {
                return (args, "Option '-i' is missing a file name. Example: -i params.txt.");
            }

            var (tokens, error) = ReadFile(path, depth);
            if (error is not null)
            {
                return (args, error);
            }

            fromFiles.AddRange(tokens);
        }

        // File arguments first; later occurrences win in OptionParser, so the command line
        // keeps the higher priority for single-occurrence options.
        fromFiles.AddRange(fromCommandLine);
        return (fromFiles.ToArray(), null);
    }

    private static InputFlagKind GetInputFlag(string arg)
    {
        if (arg.Equals("-i", StringComparison.Ordinal) || arg.Equals("--input-file", StringComparison.Ordinal))
        {
            return InputFlagKind.NextArg;
        }

        if (arg.StartsWith("-i=", StringComparison.Ordinal) || arg.StartsWith("--input-file=", StringComparison.Ordinal))
        {
            return InputFlagKind.Inline;
        }

        return InputFlagKind.None;
    }

    private static (string[] Tokens, string? Error) ReadFile(string path, int depth)
    {
        string content;
        try
        {
            content = File.ReadAllText(path);
        }
        catch (Exception ex)
        {
            return (Array.Empty<string>(), $"Could not read input file '{path}': {ex.Message}");
        }

        var (tokens, error) = Tokenize(content);
        if (error is not null)
        {
            return (Array.Empty<string>(), $"Input file '{path}': {error}");
        }

        // A file may itself contain '-i other-file.txt'; expand recursively.
        var (expanded, expansionError) = Split(tokens, depth + 1);
        return expansionError is null ? (expanded, null) : (Array.Empty<string>(), expansionError);
    }

    private static (string[] Tokens, string? Error) Tokenize(string content)
    {
        var tokens = new List<string>();
        var current = new StringBuilder();
        var hasCurrent = false;
        var inQuote = false;
        var quote = '"';

        foreach (var c in content)
        {
            if (inQuote)
            {
                if (c == quote)
                {
                    inQuote = false;
                }
                else
                {
                    current.Append(c);
                }
            }
            else if (c is '"' or '\'')
            {
                inQuote = true;
                quote = c;
                hasCurrent = true;
            }
            else if (char.IsWhiteSpace(c))
            {
                if (hasCurrent)
                {
                    tokens.Add(current.ToString());
                    current.Clear();
                    hasCurrent = false;
                }
            }
            else
            {
                current.Append(c);
                hasCurrent = true;
            }
        }

        if (inQuote)
        {
            return (Array.Empty<string>(), "unterminated quote.");
        }

        if (hasCurrent)
        {
            tokens.Add(current.ToString());
        }

        return (tokens.ToArray(), null);
    }
}
