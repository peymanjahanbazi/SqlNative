namespace SqlNative;

/// <summary>
/// Provides common persistence operations for a table row and its table structure.
/// </summary>
/// <remarks>
/// The asynchronous members are the primary implementation and must be overridden. The
/// synchronous members are concrete wrappers that call the asynchronous ones, so both paths
/// run the same code. The only exceptions are <see cref="AllAsync(CancellationToken)"/>,
/// <see cref="InsertOrUpdateAsync(List{Row}, OperationOptions, CancellationToken)"/> and
/// <see cref="IsPrimaryKeySet(Row)"/>, which are concrete because they can be expressed in
/// terms of the members above them.
/// </remarks>
public abstract partial class TableRepository<Row, Table>
    where Row : TableRow, new()
    where Table : TableStructure, new()
{
    /// <summary>Returns all rows from the table.</summary>
    /// <returns>All rows in the table.</returns>
    /// <remarks>
    /// This method blocks on <see cref="AllAsync(CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public long Count()
    {
        return CountAsync().GetAwaiter().GetResult();
    }

    /// <summary>Returns all rows from the table asynchronously.</summary>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>All rows in the table.</returns>
    /// <remarks>
    /// The default implementation selects everything through <see cref="SelectAsync"/>, so
    /// entities do not have to override it. Override it when the table needs a dedicated
    /// query for the "all rows" case.
    /// </remarks>
    public virtual Task<long> CountAsync(CancellationToken ct = default)
    {
        return CountAsync(t => "", null, ct);
    }

    /// <summary>Selects rows using a predicate over the table structure.</summary>
    /// <param name="predicate">A function that creates the predicate for the table.</param>
    /// <param name="parameters">Optional parameter values used by the predicate.</param>
    /// <returns>The selected rows.</returns>
    /// <remarks>
    /// This method blocks on <see cref="SelectAsync(Func{Table, string}, object, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public long Count(Func<Table, string> predicate, object? parameters = null)
    {
        return CountAsync(predicate, parameters).GetAwaiter().GetResult();
    }

    /// <summary>Selects rows asynchronously using a predicate over the table structure.</summary>
    /// <param name="predicate">A function that creates the predicate for the table.</param>
    /// <param name="parameters">Optional parameter values used by the predicate.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The selected rows.</returns>
    public async Task<long> CountAsync(Func<Table, string> predicate, object? parameters = null, CancellationToken ct = default)
    {
        var condition = "";

        var result = await Context.BuildQuery<Table>((q, e) => q
            .Config(() => { condition = predicate(e); })
            .Append($"SELECT COUNT(*)")
            .Append($"FROM {e}")
            .AppendIf(!string.IsNullOrWhiteSpace(condition), $"WHERE {condition}")
            )
            .AddParameters(parameters)
            .ExecuteScalarAsync(ct).ConfigureAwait(false);
        return Convert.ToInt64(result);
    }
}