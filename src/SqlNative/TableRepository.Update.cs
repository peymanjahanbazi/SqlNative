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

    /// <summary>Updates the rows matched by a model describing the statement.</summary>
    /// <param name="model">
    /// A function that creates the <see cref="UpdateModel"/> from the table structure.
    /// The model's <see cref="UpdateModel.Set"/> clause is required; the
    /// <see cref="UpdateModel.Where"/> clause is optional.
    /// </param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// The generated statement is <c>UPDATE {table} SET {Set} [WHERE {Where}]</c>, where the
    /// table alias is omitted. The values referenced by <see cref="UpdateModel.Parameters"/>
    /// are bound as command parameters, never inlined.
    /// </remarks>
    /// <exception cref="InvalidDataException">
    /// The model is <see langword="null"/> or its <see cref="UpdateModel.Set"/> clause is empty.
    /// </exception>
    public async Task<int> UpdateAsync(Func<Table, UpdateModel> model)
    {
        UpdateModel? modelObject = null;
        return await Context.BuildQuery<Table>((q, t) => q.Config(() =>
        {
            t.DisableAlias = true;
            modelObject = model(t);
            if (modelObject == null)
            {
                throw new InvalidDataException("Update model is null");
            }
            if (string.IsNullOrWhiteSpace(modelObject.Set))
            {
                throw new InvalidDataException("Set claude is empty");
            }
        })
        .Append($"UPDATE {t}")
        .Append($"SET {modelObject?.Set}")
        .AppendIf(!string.IsNullOrWhiteSpace(modelObject?.Where), $"WHERE {modelObject?.Where}")
        .SetParameters(modelObject?.Parameters))
        .ExecuteNonQueryAsync().ConfigureAwait(false);

    }

    /// <summary>Updates the rows matched by a model describing the statement.</summary>
    /// <param name="model">
    /// A function that creates the <see cref="UpdateModel"/> from the table structure.
    /// The model's <see cref="UpdateModel.Set"/> clause is required; the
    /// <see cref="UpdateModel.Where"/> clause is optional.
    /// </param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="UpdateAsync(Func{Table, UpdateModel})"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    /// <exception cref="InvalidDataException">
    /// The model is <see langword="null"/> or its <see cref="UpdateModel.Set"/> clause is empty.
    /// </exception>
    public int Update(Func<Table, UpdateModel> model)
    {
        return UpdateAsync(model).GetAwaiter().GetResult();

    }

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
    /// <param name="option">The update options. <see cref="OperationOptions.LoadRecord"/> (or <see cref="OperationOptions.LoadAndSkip"/>) reloads the affected rows from the database.</param>
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

}
