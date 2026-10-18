using System;

namespace SqlNative;

public class MySqlColumn : TableColumn
{
    public MySqlColumn(string name, bool needsBackticks, string sqlType, Type dotnetType, MySqlTableStructure table)
        : base(name, sqlType, dotnetType, table)
    {
        NeedsBackticks = needsBackticks;
    }

    public bool NeedsBackticks { get; }

    public override string ToString()
    {
        if (Table.DisableAlias)
        {
            if (NeedsBackticks)
            {
                return $"`{Name}`";
            }
            return Name;
        }
        if (NeedsBackticks)
        {
            return $"{Table.Alias}.`{Name}`";
        }

        return $"{Table.Alias}.{Name}";
    }
}