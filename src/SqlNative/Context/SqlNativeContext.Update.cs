using System.Threading;
using System.Threading.Tasks;

namespace SqlNative;

public abstract partial class SqlNativeContext
{
    /// <summary>
    /// Creates an UPDATE statement for the specified table.
    /// </summary>
    /// <typeparam name="T1">The table structure type.</typeparam>
    /// <param name="func">A function that creates the update model for the table.</param>
    /// <returns>The number of rows affected by the UPDATE statement.</returns>
    public int Update<T1>(Func<T1, UpdateModel> func)
       where T1 : TableStructure, new()
    {
        return UpdateAsync<T1>(func).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Creates an UPDATE statement for the specified table.
    /// </summary>
    /// <typeparam name="T1">The table structure type.</typeparam>
    /// <param name="func">A function that creates the update model for the table.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected by the UPDATE statement.</returns>
    public async Task<int> UpdateAsync<T1>(Func<T1, UpdateModel> func, CancellationToken ct = default)
       where T1 : TableStructure, new()
    {
        T1 t1 = new T1
        {
            DisableAlias = true
        };

        var model = func(t1);

        return await BuildQuery(q => q
            .Append($"UPDATE {t1} ")
            .Append($"SET {model.Set}")
            .Append($"WHERE {model.Where}")
            .SetParameters(model.Parameters)
        ).ExecuteNonQueryAsync(ct).ConfigureAwait(false);
    }
}
