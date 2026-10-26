using System;

namespace SqlNative;

/// <summary>
/// Represents a MySQL table column.
/// </summary>
public class MySqlColumn : TableColumn
{
    /// <summary>Initializes a new MySQL column.</summary>
    /// <param name="name">The column name.</param>
    /// <param name="needsBackticks">Whether the name must be enclosed in backticks.</param>
    /// <param name="sqlType">The SQL data type of the column.</param>
    /// <param name="dotnetType">The CLR type mapped to the column.</param>
    /// <param name="table">The table that owns the column.</param>
    public MySqlColumn(string name, bool needsBackticks, string sqlType, Type dotnetType, MySqlTableStructure table)
        : base(name, sqlType, dotnetType, table)
    {
        NeedsBackticks = needsBackticks;
    }

    /// <summary>Gets a value indicating whether the column name is enclosed in backticks.</summary>
    public bool NeedsBackticks { get; }

    /// <summary>Returns the MySQL representation of this column.</summary>
    /// <returns>The SQL text identifying this column.</returns>
    public override string ToString()
    {
        if (Table.DisableAlias)
        {
            if (NeedsBackticks)
            {
                return $"`{Name}`";
            }
            return Name;
        }
        if (NeedsBackticks)
        {
            return $"{Table.Alias}.`{Name}`";
        }

        return $"{Table.Alias}.{Name}";
    }
}
