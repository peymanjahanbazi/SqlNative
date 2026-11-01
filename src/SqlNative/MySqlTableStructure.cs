#nullable enable

namespace SqlNative;

/// <summary>
/// Base class for MySQL table structures.
/// </summary>
public abstract class MySqlTableStructure : TableStructure
{
    /// <summary>Gets the wildcard column representing all columns in the table.</summary>
    public override TableColumn Star { get; }

    /// <summary>Initializes the table structure and its wildcard column.</summary>
    protected MySqlTableStructure()
    {
        Star = new MySqlColumn("*", false, "", typeof(object), 0, this);
    }
}