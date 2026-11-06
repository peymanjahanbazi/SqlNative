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


    /// <summary>Deletes a row.</summary>
    /// <param name="row">The row to delete.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="DeleteAsync(Row, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int Delete(Row row)
    {
        return DeleteAsync(row).GetAwaiter().GetResult();
    }

    /// <summary>Deletes a row asynchronously.</summary>
    /// <param name="row">The row to delete.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public async Task<int> DeleteAsync(Row row, CancellationToken ct = default)
    {
        return await DeleteAsync(new List<Row> { row }, ct).ConfigureAwait(false);
    }

    /// <summary>Deletes a list of rows.</summary>
    /// <param name="rows">The rows to delete.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="DeleteAsync(List{Row}, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int Delete(List<Row> rows)
    {
        return DeleteAsync(rows).GetAwaiter().GetResult();
    }

    /// <summary>Deletes the rows matching a predicate asynchronously.</summary>
    /// <param name="predicate">A function that creates the predicate for the table.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public async Task<int> DeleteAsync(Func<Table, string> predicate, CancellationToken ct = default)
    {
        return await this.Context.BuildQuery<Table>((q, t) => q
        .Config(() => { t.DisableAlias = true; })
        .Append($"DELETE FROM {t}")
        .Append($" WHERE {predicate(t)}")
        ).ExecuteNonQueryAsync(ct).ConfigureAwait(false);
    }

    /// <summary>Deletes the rows matching a predicate.</summary>
    /// <param name="predicate">A function that creates the predicate for the table.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="DeleteAsync(Func{Table, string}, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int Delete(Func<Table, string> predicate)
    {
        return DeleteAsync(predicate).GetAwaiter().GetResult();
    }

}
