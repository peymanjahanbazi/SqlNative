namespace SqlNative;

/// <summary>
/// Represents a database column belonging to a table structure.
/// </summary>
public abstract class TableColumn
{
    /// <summary>Initializes a new table column.</summary>
    /// <param name="name">The column name.</param>
    /// <param name="sqlType">The SQL data type of the column.</param>
    /// <param name="dotnetType">The CLR type mapped to the column.</param>
    /// <param name="table">The table that owns the column.</param>
    protected TableColumn(string name, string sqlType, Type dotnetType, TableStructure table)
    {
        Table = table;
        Name = name;
        SqlType = sqlType;
        DotnetType = dotnetType;
    }

    /// <summary>Gets the table that owns this column.</summary>
    public TableStructure Table { get; private set; }

    /// <summary>Gets the column name.</summary>
    public string Name { get; }

    /// <summary>Gets the CLR type mapped to this column.</summary>
    public Type DotnetType { get; }

    /// <summary>Gets the SQL data type of this column.</summary>
    public string SqlType { get; }

    /// <summary>Converts a column to its SQL representation.</summary>
    /// <param name="column">The column to convert.</param>
    /// <returns>The SQL representation of the column.</returns>
    public static implicit operator string(TableColumn column)
    {
        if (column == null)
            return "";
        return column.ToString();
    }

    /// <summary>Concatenates two columns with a space.</summary>
    /// <param name="a">The first column.</param>
    /// <param name="b">The second column.</param>
    /// <returns>The concatenated SQL text.</returns>
    public static string operator +(TableColumn a, TableColumn b)
    {
        return $"{a} {b}";
    }

    /// <summary>Concatenates a column and text with a space.</summary>
    /// <param name="a">The column.</param>
    /// <param name="b">The text to append.</param>
    /// <returns>The concatenated SQL text.</returns>
    public static string operator +(TableColumn a, string b)
    {
        return $"{a} {b}";
    }

    /// <summary>Concatenates text and a column with a space.</summary>
    /// <param name="a">The text to prepend.</param>
    /// <param name="b">The column.</param>
    /// <returns>The concatenated SQL text.</returns>
    public static string operator +(string a, TableColumn b)
    {
        return $"{a} {b}";
    }
}
