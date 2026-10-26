using System;

namespace SqlNative;

/// <summary>
/// Represents an Oracle table column.
/// </summary>
public class OracleColumn : TableColumn
{
    /// <summary>Initializes a new Oracle column.</summary>
    /// <param name="name">The column name.</param>
    /// <param name="needsDoubleQuotes">Whether the name must be enclosed in double quotes.</param>
    /// <param name="sqlType">The SQL data type of the column.</param>
    /// <param name="dotnetType">The CLR type mapped to the column.</param>
    /// <param name="table">The table that owns the column.</param>
    public OracleColumn(string name, bool needsDoubleQuotes, string sqlType, Type dotnetType, OracleTableStructure table)
        : base(name, sqlType, dotnetType, table)
    {
        NeedsDoubleQuotes = needsDoubleQuotes;
    }

    /// <summary>Gets a value indicating whether the column name is enclosed in double quotes.</summary>
    public bool NeedsDoubleQuotes { get; }

    /// <summary>Returns the Oracle representation of this column.</summary>
    /// <returns>The SQL text identifying this column.</returns>
    public override string ToString()
    {
        if (Table.DisableAlias)
        {
            if (NeedsDoubleQuotes)
            {
                return $"\"{Name}\"";
            }
            return Name;
        }
        if (NeedsDoubleQuotes)
        {
            return $"{Table.Alias}.\"{Name}\"";
        }

        return $"{Table.Alias}.{Name}";
    }
}
