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
    /// <summary>Gets the context used to run the commands of this repository.</summary>

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
}
