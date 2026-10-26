#nullable enable

namespace SqlNative;

/// <summary>
/// Represents the structure and columns of a database table.
/// </summary>
public abstract class TableStructure
{
    /// <summary>Gets or sets whether the table alias is omitted from generated SQL.</summary>
    public bool DisableAlias { get; set; } = false;

    /// <summary>Gets or sets the alias used for the table in generated SQL.</summary>
    public string Alias { get; set; } = string.Empty;

    /// <summary>Gets the table name.</summary>
    public abstract string TableName { get; }

    /// <summary>Gets the quoted table name used in generated SQL.</summary>
    protected abstract string QuotedTableName { get; }

    /// <summary>Gets the wildcard column representing all columns in the table.</summary>
    public abstract TableColumn Star { get; }

    /// <summary>Gets the primary key column, if the table has one.</summary>
    public abstract TableColumn? PKey { get; }

    /// <summary>Returns the SQL representation of this table.</summary>
    /// <returns>The quoted table name, optionally followed by its alias.</returns>
    public override string ToString()
    {
        if (DisableAlias)
        {
            return QuotedTableName;
        }
        return $"{QuotedTableName} as {Alias}";
    }

    /// <summary>Converts a table structure to its SQL representation.</summary>
    /// <param name="table">The table structure to convert.</param>
    /// <returns>The SQL representation of the table.</returns>
    public static implicit operator string(TableStructure table)
    {
        if (table == null)
            return "";
        return table.ToString();
    }
}
