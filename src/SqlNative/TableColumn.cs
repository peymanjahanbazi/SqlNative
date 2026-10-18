using System;

namespace SqlNative;

public abstract class TableColumn
{
    protected TableColumn(string name, string sqlType, Type dotnetType, TableStructure table)
    {
        Table = table;
        Name = name;
        SqlType = sqlType;
        DotnetType = dotnetType;
    }

    public TableStructure Table { get; private set; }
    public string Name { get; }
    public string BaseParameter { get; }
    private ushort idx = 0;
    public string parameter => $"{BaseParameter}{++idx}";
    public Type DotnetType { get; }
    public string SqlType { get; }

    public static implicit operator string(TableColumn column)
    {
        if (column == null)
            return null;
        return column.ToString();
    }

    public static string operator +(TableColumn a, TableColumn b)
    {
        return $"{a}, {b}";
    }

    public static string operator +(TableColumn a, string b)
    {
        return $"{a}, {b}";
    }

    public static string operator +(string a, TableColumn b)
    {
        return $"{a}, {b}";
    }

    public static string operator -(TableColumn a, TableColumn b)
    {
        return $"{a} {b}";
    }
}