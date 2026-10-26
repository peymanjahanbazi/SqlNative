using System;

namespace SqlNative;

public abstract partial class SqlNativeContext
{
    /// <summary>
    /// Creates an INSERT statement for the specified table.
    /// </summary>
    /// <typeparam name="T1">The table structure type.</typeparam>
    /// <param name="func">A function that creates the insert model for the table.</param>
    /// <returns>The query containing the INSERT statement.</returns>
    public Query Insert<T1>(Func<T1, InsertModel> func)
       where T1 : TableStructure, new()
    {
        T1 t1 = new()
        {
            DisableAlias = true
        };
        var model = func(t1);
        ValidateInsertModel(model);
        return BuildQuery(q => q
            .Append($"INSERT INTO {t1}")
            .Append($"({model.Columns})"))
            .Append($"VALUES ( {model.Values} )")
            .SetParameters(model.Parameters);
    }

    private static void ValidateInsertModel(InsertModel model)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        if (string.IsNullOrWhiteSpace(model.Columns))
        {
            throw new InvalidOperationException("InsertModel.Columns cannot be null or empty.");
        }
        if (string.IsNullOrWhiteSpace(model.Values))
        {
            throw new InvalidOperationException("InsertModel.Values cannot be null or empty.");
        }
    }
}