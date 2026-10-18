#nullable enable

namespace SqlNative;

public abstract class PostgresTableStructure : TableStructure
{
    public abstract string Schema { get; }

    public override TableColumn Star { get; }

    public override TableColumn? PKey { get; }

    protected PostgresTableStructure()
    {
        Star = new PostgresColumn("*", false, "", typeof(object), this);
    }
}