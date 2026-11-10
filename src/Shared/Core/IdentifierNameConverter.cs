using System.Text;
using System.Text.RegularExpressions;

namespace SqlNative.Core;

/// <summary>
/// Turns a database identifier into a PascalCase C# identifier. Shared by every per-engine
/// identifier helper so all four dialects agree on the result.
/// </summary>
internal static class IdentifierNameConverter
{
    private static readonly Regex _nonAlphanumericRegex = new Regex(@"[^A-Za-z0-9]+");

    /// <summary>
    /// Splits the identifier on every non-alphanumeric run and capitalises each part.
    /// A part that already mixes upper and lower case carries its own word boundaries and
    /// keeps them (<c>CountryCode</c> stays <c>CountryCode</c>); a part that is entirely upper
    /// or entirely lower case is title cased (<c>COUNTRY</c> becomes <c>Country</c>).
    /// </summary>
    public static string ToPascalCase(string originalName)
    {
        if (string.IsNullOrWhiteSpace(originalName))
        {
            return "Item";
        }

        var parts = _nonAlphanumericRegex.Split(originalName.Trim());
        var builder = new StringBuilder();

        foreach (var part in parts)
        {
            if (part.Length == 0)
            {
                continue;
            }

            builder.Append(Capitalize(part));
        }

        return builder.Length == 0 ? "Item" : builder.ToString();
    }

    private static string Capitalize(string part)
    {
        var head = char.ToUpperInvariant(part[0]);

        return HasMixedCase(part)
            ? head + part.Substring(1)
            : head + part.Substring(1).ToLowerInvariant();
    }

    /// <summary>
    /// True when the word holds at least one upper and one lower case letter, which means it
    /// already marks its own word boundaries (<c>CountryCode</c>).
    /// </summary>
    private static bool HasMixedCase(string part)
    {
        var hasUpper = false;
        var hasLower = false;

        foreach (var c in part)
        {
            if (char.IsUpper(c))
            {
                hasUpper = true;
            }
            else if (char.IsLower(c))
            {
                hasLower = true;
            }

            if (hasUpper && hasLower)
            {
                return true;
            }
        }

        return false;
    }
}
