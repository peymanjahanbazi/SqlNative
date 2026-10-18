#nullable enable

namespace SqlNative;

public abstract class MySqlTableStructure : TableStructure
{
    public override TableColumn Star { get; }

    public override TableColumn? PKey { get; }

    protected MySqlTableStructure()
    {
        Star = new MySqlColumn("*", false, "", typeof(object), this);
    }
}