using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SqlNative;

/// <summary>
/// Specifies how an inserted record is handled and whether its primary key is included.
/// </summary>
public enum InsertOptions
{
    /// <summary>Loads the inserted record and includes its generated primary key.</summary>
    LoadRecord = 1,
    /// <summary>Skips the primary key when creating the insert statement.</summary>
    SkipPrimaryKey = 2,
    /// <summary>Loads the inserted record while skipping the primary key in the insert statement.</summary>
    LoadAndSkip = 3
}

/// <summary>
/// Provides common persistence operations for a table row and its table structure.
/// </summary>
/// <typeparam name="Row">The row type persisted by this entity.</typeparam>
/// <typeparam name="Structure">The table structure type used by this entity.</typeparam>
/// <typeparam name="Pk">The primary key type used by this entity.</typeparam>
/// <remarks>
/// The asynchronous members are the primary implementation and must be overridden. The
/// synchronous members are concrete wrappers that call the asynchronous ones, so both paths
/// run the same code. The only exceptions are <see cref="AllAsync(CancellationToken)"/> and
/// the <c>Store</c> members, which are concrete because they can be expressed in terms of
/// the members above them.
/// </remarks>
public abstract class DbEntity<Row, Structure, Pk>
    where Row : TableRow<Pk>, new()
    where Structure : TableStructure, new()
{
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
    public abstract Task<int> InsertAsync(Row row, InsertOptions options = InsertOptions.SkipPrimaryKey, CancellationToken ct = default);

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
    public abstract Task<int> UpdateAsync(Row row, bool reload = false, CancellationToken ct = default);

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
    public abstract Task<int> DeleteAsync(Row row, CancellationToken ct = default);

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

    /// <summary>Deletes a list of rows asynchronously.</summary>
    /// <param name="rows">The rows to delete.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract Task<int> DeleteAsync(List<Row> rows, CancellationToken ct = default);

    /// <summary>
    /// Determines whether a primary key value is <see langword="null"/> or the default value for its type.
    /// </summary>
    /// <param name="value">The primary key value to inspect.</param>
    /// <returns><see langword="true"/> when the value is null or default; otherwise, <see langword="false"/>.</returns>
    protected bool IsNullOrDefault(Pk value)
    {
        if (value == null) return true;
        return EqualityComparer<Pk>.Default.Equals(value, default(Pk));
    }

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
    public abstract Task<List<Row>> SelectAsync(Func<Structure, string> predicate, object parameters = null, CancellationToken ct = default);

    /// <summary>Inserts a row when its key is empty, or updates it otherwise.</summary>
    /// <param name="row">The row to store.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="StoreAsync(Row, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int Store(Row row)
    {
        return StoreAsync(row).GetAwaiter().GetResult();
    }

    /// <summary>Inserts a row when its key is empty, or updates it otherwise.</summary>
    /// <param name="row">The row to store.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public async Task<int> StoreAsync(Row row, CancellationToken ct = default)
    {
        if (IsNullOrDefault(row.Key))
        {
            return await InsertAsync(row, InsertOptions.SkipPrimaryKey, ct).ConfigureAwait(false);
        }
        return await UpdateAsync(row, false, ct).ConfigureAwait(false);
    }

    /// <summary>Inserts rows with empty keys and updates rows with populated keys.</summary>
    /// <param name="rows">The rows to store.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// This method blocks on <see cref="StoreAsync(List{Row}, CancellationToken)"/>.
    /// Prefer the asynchronous version in asynchronous code.
    /// </remarks>
    public int Store(List<Row> rows)
    {
        return StoreAsync(rows).GetAwaiter().GetResult();
    }

    /// <summary>Inserts rows with empty keys and updates rows with populated keys.</summary>
    /// <param name="rows">The rows to store.</param>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public async Task<int> StoreAsync(List<Row> rows, CancellationToken ct = default)
    {
        var updates = rows.Where(r => !IsNullOrDefault(r.Key)).ToList();
        var inserts = rows.Where(r => IsNullOrDefault(r.Key)).ToList();
        var inserted = await InsertAsync(inserts, InsertOptions.SkipPrimaryKey, ct).ConfigureAwait(false);
        var updated = await UpdateAsync(updates, false, ct).ConfigureAwait(false);
        return inserted + updated;
    }
}
