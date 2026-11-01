namespace SqlNative;

/// <summary>
/// Provides common persistence operations for a table row and its table structure.
/// </summary>
/// <typeparam name="Row">The row type persisted by this entity.</typeparam>
/// <typeparam name="Structure">The table structure type used by this entity.</typeparam>
/// <remarks>
/// The asynchronous members are the primary implementation and must be overridden. The
/// synchronous members are concrete wrappers that call the asynchronous ones, so both paths
/// run the same code. The only exceptions are <see cref="AllAsync(CancellationToken)"/> and
/// the <c>Store</c> members, which are concrete because they can be expressed in terms of
/// the members above them.
/// </remarks>
public abstract class DbEntity<Row, Structure>
    where Row : TableRow, new()
    where Structure : TableStructure, new()
{
    /// <summary>Gets the context used to run the commands of this entity.</summary>
    public abstract SqlNativeContext Context { get; }

    /// <summary>Inserts a row.</summary>
    /// <param name="row">The row to insert.</param>
    /// <param name="options">The insert options.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="InsertAsync(Row, InsertOptions, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int Insert(Row row, InsertOptions options = InsertOptions.SkipPrimaryKey)
    {
        return InsertAsync(row, options).GetAwaiter().GetResult();
    }

    /// <summary>Inserts a row asynchronously.</summary>
    /// <param name="row">The row to insert.</param>
    /// <param name="options">The insert options.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public async Task<int> InsertAsync(Row row, InsertOptions options = InsertOptions.SkipPrimaryKey, CancellationToken ct = default)
    {
        return await InsertAsync(new List<Row> { row }, options, ct).ConfigureAwait(false);
    }

    /// <summary>Inserts a list of rows.</summary>
    /// <param name="rows">The rows to insert.</param>
    /// <param name="options">The insert options.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="InsertAsync(List{Row}, InsertOptions, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int Insert(List<Row> rows, InsertOptions options = InsertOptions.SkipPrimaryKey)
    {
        return InsertAsync(rows, options).GetAwaiter().GetResult();
    }

    /// <summary>Inserts a list of rows asynchronously.</summary>
    /// <param name="rows">The rows to insert.</param>
    /// <param name="options">The insert options.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract Task<int> InsertAsync(List<Row> rows, InsertOptions options = InsertOptions.SkipPrimaryKey, CancellationToken ct = default);

    /// <summary>Updates a row.</summary>
    /// <param name="row">The row to update.</param>
    /// <param name="reload">Whether the updated row should be reloaded from the database.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="UpdateAsync(Row, bool, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int Update(Row row, bool reload = false)
    {
        return UpdateAsync(row, reload).GetAwaiter().GetResult();
    }

    /// <summary>Updates a row asynchronously.</summary>
    /// <param name="row">The row to update.</param>
    /// <param name="reload">Whether the updated row should be reloaded from the database.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public async Task<int> UpdateAsync(Row row, bool reload = false, CancellationToken ct = default)
    {
        return await UpdateAsync(new List<Row> { row }, reload, ct).ConfigureAwait(false);
    }

    /// <summary>Updates a list of rows.</summary>
    /// <param name="rows">The rows to update.</param>
    /// <param name="reload">Whether the updated rows should be reloaded from the database.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="UpdateAsync(List{Row}, bool, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int Update(List<Row> rows, bool reload = false)
    {
        return UpdateAsync(rows, reload).GetAwaiter().GetResult();
    }

    /// <summary>Updates a list of rows asynchronously.</summary>
    /// <param name="rows">The rows to update.</param>
    /// <param name="reload">Whether the updated rows should be reloaded from the database.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract Task<int> UpdateAsync(List<Row> rows, bool reload = false, CancellationToken ct = default);

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
    public async Task<int> DeleteAsync(Func<Structure, string> predicate, CancellationToken ct = default)
    {
        return await this.Context.BuildQuery<Structure>((q, t) => q
        .Config(() => { t.DisableAlias = true; })
        .Append($"DELETE FROM {t}")
        .Append($" WHERE {predicate(t)}")
        ).ExecuteNonQueryAsync(ct).ConfigureAwait(false);
    }

    /// <summary>Deletes the rows matching a predicate.</summary>
    /// <param name="predicate">A function that creates the predicate for the table.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="DeleteAsync(Func{Structure, string}, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int Delete(Func<Structure, string> predicate)
    {
        return DeleteAsync(predicate).GetAwaiter().GetResult();
    }

    /// <summary>Deletes a list of rows asynchronously.</summary>
    /// <param name="rows">The rows to delete.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract Task<int> DeleteAsync(List<Row> rows, CancellationToken ct = default);

    /// <summary>Returns all rows from the table.</summary>
    /// <returns>All rows in the table.</returns>
    /// <remarks>
    /// This method blocks on <see cref="AllAsync(CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public List<Row> All()
    {
        return AllAsync().GetAwaiter().GetResult();
    }

    /// <summary>Returns all rows from the table asynchronously.</summary>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>All rows in the table.</returns>
    /// <remarks>
    /// The default implementation selects everything through <see cref="SelectAsync"/>, so
    /// entities do not have to override it. Override it when the table needs a dedicated
    /// query for the "all rows" case.
    /// </remarks>
    public virtual Task<List<Row>> AllAsync(CancellationToken ct = default)
    {
        return SelectAsync(t => "", null, ct);
    }

    /// <summary>Selects rows using a predicate over the table structure.</summary>
    /// <param name="predicate">A function that creates the predicate for the table.</param>
    /// <param name="parameters">Optional parameter values used by the predicate.</param>
    /// <returns>The selected rows.</returns>
    /// <remarks>
    /// This method blocks on <see cref="SelectAsync(Func{Structure, string}, object, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public List<Row> Select(Func<Structure, string> predicate, object parameters = null)
    {
        return SelectAsync(predicate, parameters).GetAwaiter().GetResult();
    }

    /// <summary>Selects rows asynchronously using a predicate over the table structure.</summary>
    /// <param name="predicate">A function that creates the predicate for the table.</param>
    /// <param name="parameters">Optional parameter values used by the predicate.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The selected rows.</returns>
    public async Task<List<Row>> SelectAsync(Func<Structure, string> predicate, object parameters = null, CancellationToken ct = default)
    {
        var condition = "";

        return await Context.BuildQuery<Structure>((q, e) => q
            .Config(() => { condition = predicate(e); })
            .Append($"SELECT {e.Star}")
            .Append($"FROM {e}")
            .AppendIf(!string.IsNullOrWhiteSpace(condition), $"WHERE {condition}")
            )
            .SetParameters(parameters)
            .ExecuteAsync<Row>(ct).ConfigureAwait(false);
    }
}