using System;

namespace SqlNative;

public class MsSqlColumn : TableColumn
{
    public MsSqlColumn(string name, bool needsBrackets, string sqlType, Type dotnetType, MsSqlTableStructure table)
        : base(name, sqlType, dotnetType, table)
    {
        NeedsBrackets = needsBrackets;
    }

    public bool NeedsBrackets { get; }

    public override string ToString()
    {
        if (Table.DisableAlias)
        {
            if (NeedsBrackets)
            {
                return $"[{Name}]";
            }
            return Name;
        }
        if (NeedsBrackets)
        {
            return $"{Table.Alias}.[{Name}]";
        }

        return $"{Table.Alias}.{Name}";
    }
}