using System;

namespace SqlNative;

public class PostgresColumn : TableColumn
{
    public PostgresColumn(string name, bool needsDoubleQuotes, string sqlType, Type dotnetType, PostgresTableStructure table)
        : base(name, sqlType, dotnetType, table)
    {
        NeedsDoubleQuotes = needsDoubleQuotes;
    }

    public bool NeedsDoubleQuotes { get; }

    public override string ToString()
    {
        if (Table.DisableAlias)
        {
            if (NeedsDoubleQuotes)
            {
                return $"\"{Name}\"";
            }
            return Name;
        }
        if (NeedsDoubleQuotes)
        {
            return $"{Table.Alias}.\"{Name}\"";
        }

        return $"{Table.Alias}.{Name}";
    }
}