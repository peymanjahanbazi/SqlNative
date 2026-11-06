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
public abstract partial class TableRepository<Row, Table>
    where Row : TableRow, new()
    where Table : TableStructure, new()
{
    /// <summary>Gets the context used to run the commands of this repository.</summary>
    public abstract SqlNativeContext Context { get; }

    /// <summary>Inserts a list of rows asynchronously.</summary>
    /// <param name="rows">The rows to insert.</param>
    /// <param name="options">The insert options.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract Task<int> InsertAsync(List<Row> rows, OperationOptions options = OperationOptions.SkipPrimaryKey, CancellationToken ct = default);

    /// <summary>Updates a list of rows asynchronously.</summary>
    /// <param name="rows">The rows to update.</param>
    /// <param name="reload">The update options. <see cref="OperationOptions.LoadRecord"/> (or <see cref="OperationOptions.LoadAndSkip"/>) reloads the updated rows from the database.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract Task<int> UpdateAsync(List<Row> rows, OperationOptions reload = OperationOptions.None, CancellationToken ct = default);

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

    /// <summary>Deletes a list of rows asynchronously.</summary>
    /// <param name="rows">The rows to delete.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract Task<int> DeleteAsync(List<Row> rows, CancellationToken ct = default);

}