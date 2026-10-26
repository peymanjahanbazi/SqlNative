#nullable enable

namespace SqlNative;

/// <summary>
/// Base class for Microsoft SQL Server table structures.
/// </summary>
public abstract class MsSqlTableStructure : TableStructure
{
    /// <summary>Gets the schema containing the table.</summary>
    public abstract string Schema { get; }

    /// <summary>Gets the wildcard column representing all columns in the table.</summary>
    public override TableColumn Star { get; }

    /// <summary>Gets the primary key column, if the table has one.</summary>
    public override TableColumn? PKey { get; }

    /// <summary>Initializes the table structure and its wildcard column.</summary>
    protected MsSqlTableStructure()
    {
        Star = new MsSqlColumn("*", false, "", typeof(object), this);
    }
}
