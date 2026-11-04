namespace SqlNative;

/// <summary>
/// Provides common persistence operations for a table row and its table structure.
/// </summary>
/// <typeparam name="Row">The row type persisted by this repository.</typeparam>
/// <typeparam name="Table">The table structure type used by this repository.</typeparam>
/// <remarks>
/// The asynchronous members are the primary implementation and must be overridden. The
/// synchronous members are concrete wrappers that call the asynchronous ones, so both paths
/// run the same code. The only exceptions are <see cref="AllAsync(CancellationToken)"/>,
/// <see cref="InsertOrUpdateAsync(List{Row}, OperationOptions, CancellationToken)"/> and
/// <see cref="IsPrimaryKeySet(Row)"/>, which are concrete because they can be expressed in
/// terms of the members above them.
/// </remarks>
public abstract class TableRepository<Row, Table>
    where Row : TableRow, new()
    where Table : TableStructure, new()
{
    /// <summary>Gets the context used to run the commands of this repository.</summary>
    public abstract SqlNativeContext Context { get; }

    /// <summary>Inserts a row.</summary>
    /// <param name="row">The row to insert.</param>
    /// <param name="options">The insert options.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="InsertAsync(Row, OperationOptions, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int Insert(Row row, OperationOptions options = OperationOptions.SkipPrimaryKey)
    {
        return InsertAsync(row, options).GetAwaiter().GetResult();
    }

    /// <summary>Inserts a row asynchronously.</summary>
    /// <param name="row">The row to insert.</param>
    /// <param name="options">The insert options.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public async Task<int> InsertAsync(Row row, OperationOptions options = OperationOptions.SkipPrimaryKey, CancellationToken ct = default)
    {
        return await InsertAsync(new List<Row> { row }, options, ct).ConfigureAwait(false);
    }

    /// <summary>Inserts a list of rows.</summary>
    /// <param name="rows">The rows to insert.</param>
    /// <param name="options">The insert options.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="InsertAsync(List{Row}, OperationOptions, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int Insert(List<Row> rows, OperationOptions options = OperationOptions.SkipPrimaryKey)
    {
        return InsertAsync(rows, options).GetAwaiter().GetResult();
    }

    /// <summary>Inserts a list of rows asynchronously.</summary>
    /// <param name="rows">The rows to insert.</param>
    /// <param name="options">The insert options.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract Task<int> InsertAsync(List<Row> rows, OperationOptions options = OperationOptions.SkipPrimaryKey, CancellationToken ct = default);

    /// <summary>Updates a row.</summary>
    /// <param name="row">The row to update.</param>
    /// <param name="reload">The update options. <see cref="OperationOptions.LoadRecord"/> (or <see cref="OperationOptions.LoadAndSkip"/>) reloads the updated row(s) from the database.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="UpdateAsync(Row, OperationOptions, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int Update(Row row, OperationOptions reload = OperationOptions.None)
    {
        return UpdateAsync(row, reload).GetAwaiter().GetResult();
    }

    /// <summary>Updates a row asynchronously.</summary>
    /// <param name="row">The row to update.</param>
    /// <param name="reload">The update options. <see cref="OperationOptions.LoadRecord"/> (or <see cref="OperationOptions.LoadAndSkip"/>) reloads the updated row(s) from the database.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public async Task<int> UpdateAsync(Row row, OperationOptions reload = OperationOptions.None, CancellationToken ct = default)
    {
        return await UpdateAsync(new List<Row> { row }, reload, ct).ConfigureAwait(false);
    }

    /// <summary>Updates a list of rows.</summary>
    /// <param name="rows">The rows to update.</param>
    /// <param name="reload">The update options. <see cref="OperationOptions.LoadRecord"/> (or <see cref="OperationOptions.LoadAndSkip"/>) reloads the updated rows from the database.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="UpdateAsync(List{Row}, OperationOptions, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int Update(List<Row> rows, OperationOptions reload = OperationOptions.None)
    {
        return UpdateAsync(rows, reload).GetAwaiter().GetResult();
    }

    /// <summary>Updates a list of rows asynchronously.</summary>
    /// <param name="rows">The rows to update.</param>
    /// <param name="reload">The update options. <see cref="OperationOptions.LoadRecord"/> (or <see cref="OperationOptions.LoadAndSkip"/>) reloads the updated rows from the database.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract Task<int> UpdateAsync(List<Row> rows, OperationOptions reload = OperationOptions.None, CancellationToken ct = default);

    /// <summary>Inserts or updates a list of rows.</summary>
    /// <param name="rows">The rows to insert or update.</param>
    /// <param name="reload">The update options. <see cref="OperationOptions.LoadRecord"/> (or <see cref="OperationOptions.LoadAndSkip"/>) reloads the affected rows from the database.</param>
    /// <returns>The total number of rows inserted or updated.</returns>
    /// <remarks>
    /// This method blocks on <see cref="InsertOrUpdateAsync(List{Row}, OperationOptions, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int InsertOrUpdate(List<Row> rows, OperationOptions reload = OperationOptions.LoadRecord)
    {
        return InsertOrUpdateAsync(rows, reload).GetAwaiter().GetResult();
    }

    /// <summary>Inserts or updates a list of rows asynchronously.</summary>
    /// <param name="rows">The rows to insert or update.</param>
    /// <param name="reload">The update options. <see cref="OperationOptions.LoadRecord"/> (or <see cref="OperationOptions.LoadAndSkip"/>) reloads the affected rows from the database.</param>
    /// <param name="ct">The token used to cancel the commands.</param>
    /// <returns>The total number of rows inserted or updated.</returns>
    /// <remarks>
    /// Rows whose primary key is not set (see <see cref="IsPrimaryKeySet(Row)"/>) are inserted;
    /// all remaining rows are updated, preserving the original order in each list. The option is
    /// forwarded to both operations: for the insert, <see cref="OperationOptions.SkipPrimaryKey"/>
    /// also omits the primary key; <see cref="OperationOptions.LoadRecord"/> (or
    /// <see cref="OperationOptions.LoadAndSkip"/>) reloads the affected rows from the database.
    /// Tables without a primary key are not supported: the repository's insert and update
    /// operations then throw <see cref="NotImplementedException"/>.
    /// </remarks>
    public async Task<int> InsertOrUpdateAsync(List<Row> rows, OperationOptions option = OperationOptions.SkipPrimaryKey, CancellationToken ct = default)
    {
        var insertList = rows.Where(x => !IsPrimaryKeySet(x)).ToList();
        var updateList = rows.Except(insertList).ToList();

        int insertCount = await InsertAsync(insertList, option, ct);
        int updateCount = await UpdateAsync(updateList, option, ct);
        return insertCount + updateCount;
    }

    /// <summary>Determines whether the primary key of a row is set.</summary>
    /// <param name="row">The row to inspect.</param>
    /// <returns>
    /// <see langword="true"/> if every primary key column holds a value that is neither
    /// <see langword="null"/> nor the CLR default value; otherwise <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// Composite (multi-column) primary keys are supported: the key counts as set only when
    /// every key column is set. Used by <see cref="InsertOrUpdateAsync(List{Row}, OperationOptions, CancellationToken)"/>
    /// to split rows into insert and update batches.
    /// </remarks>
    protected abstract bool IsPrimaryKeySet(Row row);

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
    /// This method blocks on <see cref="SelectAsync(Func{Table, string}, object, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public List<Row> Select(Func<Table, string> predicate, object parameters = null)
    {
        return SelectAsync(predicate, parameters).GetAwaiter().GetResult();
    }

    /// <summary>Selects rows asynchronously using a predicate over the table structure.</summary>
    /// <param name="predicate">A function that creates the predicate for the table.</param>
    /// <param name="parameters">Optional parameter values used by the predicate.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The selected rows.</returns>
    public async Task<List<Row>> SelectAsync(Func<Table, string> predicate, object parameters = null, CancellationToken ct = default)
    {
        var condition = "";

        return await Context.BuildQuery<Table>((q, e) => q
            .Config(() => { condition = predicate(e); })
            .Append($"SELECT {e.Star}")
            .Append($"FROM {e}")
            .AppendIf(!string.IsNullOrWhiteSpace(condition), $"WHERE {condition}")
            )
            .SetParameters(parameters)
            .ExecuteAsync<Row>(ct).ConfigureAwait(false);
    }
}