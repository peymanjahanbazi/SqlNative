#nullable enable

namespace SqlNative;

public abstract class MsSqlTableStructure : TableStructure
{
    public abstract string Schema { get; }

    public override TableColumn Star { get; }

    public override TableColumn? PKey { get; }

    protected MsSqlTableStructure()
    {
        Star = new MsSqlColumn("*", false, "", typeof(object), this);
    }
}