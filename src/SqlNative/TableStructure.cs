#nullable enable

namespace SqlNative;

public abstract class TableStructure
{
    public bool DisableAlias { get; set; } = false;
    public string Alias { get; set; } = string.Empty;
    public abstract string TableName { get; }
    protected abstract string QuotedTableName { get; }
    public abstract TableColumn Star { get; }
    public abstract TableColumn? PKey { get; }

    public override string ToString()
    {
        if (DisableAlias)
        {
            return QuotedTableName;
        }
        return $"{QuotedTableName} as {Alias}";
    }

    public static implicit operator string(TableStructure table)
    {
        if (table == null)
            return "";
        return table.ToString();
    }
}