using System;

namespace SqlNative;

public abstract partial class SqlNativeContext
{
    /// <summary>
    /// Creates an UPDATE statement for the specified table.
    /// </summary>
    /// <typeparam name="T1">The table structure type.</typeparam>
    /// <param name="func">A function that creates the update model for the table.</param>
    /// <returns>The query containing the UPDATE statement.</returns>
    public Query Update<T1>(Func<T1, UpdateModel> func)
       where T1 : TableStructure, new()
    {
        T1 t1 = new T1
        {
            DisableAlias = true
        };

        var model = func(t1);

        return BuildQuery(q => q
            .Append($"UPDATE {t1} ")
            .Append($"SET {model.Set}")
            .Append($"WHERE {model.Where}")
            .SetParameters(model.Parameters)
        );
    }
}