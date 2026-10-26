#nullable enable

namespace SqlNative;

/// <summary>
/// Base class for MySQL table structures.
/// </summary>
public abstract class MySqlTableStructure : TableStructure
{
    /// <summary>Gets the wildcard column representing all columns in the table.</summary>
    public override TableColumn Star { get; }

    /// <summary>Gets the primary key column, if the table has one.</summary>
    public override TableColumn? PKey { get; }

    /// <summary>Initializes the table structure and its wildcard column.</summary>
    protected MySqlTableStructure()
    {
        Star = new MySqlColumn("*", false, "", typeof(object), this);
    }
}
