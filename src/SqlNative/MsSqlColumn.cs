using System;

namespace SqlNative;

/// <summary>
/// Represents a Microsoft SQL Server table column.
/// </summary>
public class MsSqlColumn : TableColumn
{
    /// <summary>Initializes a new Microsoft SQL Server column.</summary>
    /// <param name="name">The column name.</param>
    /// <param name="needsBrackets">Whether the name must be enclosed in square brackets.</param>
    /// <param name="sqlType">The SQL data type of the column.</param>
    /// <param name="dotnetType">The CLR type mapped to the column.</param>
    /// <param name="table">The table that owns the column.</param>
    public MsSqlColumn(string name, bool needsBrackets, string sqlType, Type dotnetType, MsSqlTableStructure table)
        : base(name, sqlType, dotnetType, table)
    {
        NeedsBrackets = needsBrackets;
    }

    /// <summary>Gets a value indicating whether the column name is enclosed in square brackets.</summary>
    public bool NeedsBrackets { get; }

    /// <summary>Returns the Microsoft SQL Server representation of this column.</summary>
    /// <returns>The SQL text identifying this column.</returns>
    public override string ToString()
    {
        if (Table.DisableAlias)
        {
            if (NeedsBrackets)
            {
                return $"[{Name}]";
            }
            return Name;
        }
        if (NeedsBrackets)
        {
            return $"{Table.Alias}.[{Name}]";
        }

        return $"{Table.Alias}.{Name}";
    }
}
