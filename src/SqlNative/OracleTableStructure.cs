#nullable enable

namespace SqlNative;

/// <summary>
/// Base class for Oracle table structures.
/// </summary>
public abstract class OracleTableStructure : TableStructure
{
    /// <summary>Gets the schema containing the table.</summary>
    public abstract string Schema { get; }

    /// <summary>Gets the wildcard column representing all columns in the table.</summary>
    public override TableColumn Star { get; }

    /// <summary>Initializes the table structure and its wildcard column.</summary>
    protected OracleTableStructure()
    {
        Star = new OracleColumn("*", false, "", typeof(object), 0, this);
    }
}