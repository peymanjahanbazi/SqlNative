#nullable enable

namespace SqlNative;

public abstract class OracleTableStructure : TableStructure
{
    public abstract string Schema { get; }

    public override TableColumn Star { get; }

    public override TableColumn? PKey { get; }

    protected OracleTableStructure()
    {
        Star = new OracleColumn("*", false, "", typeof(object), this);
    }
}