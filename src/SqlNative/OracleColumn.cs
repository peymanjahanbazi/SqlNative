using System;

namespace SqlNative;

public class OracleColumn : TableColumn
{
    public OracleColumn(string name, bool needsDoubleQuotes, string sqlType, Type dotnetType, OracleTableStructure table)
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