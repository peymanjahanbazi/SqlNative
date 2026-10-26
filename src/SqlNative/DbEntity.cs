using System;
using System.Collections.Generic;
using System.Linq;

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
public abstract class DbEntity<Row, Structure, Pk>
    where Row : TableRow<Pk>, new()
    where Structure : TableStructure, new()
{
    /// <summary>Inserts a row.</summary>
    /// <param name="row">The row to insert.</param>
    /// <param name="options">The insert options.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract int Insert(Row row, InsertOptions options = InsertOptions.SkipPrimaryKey);

    /// <summary>Inserts a list of rows.</summary>
    /// <param name="rows">The rows to insert.</param>
    /// <param name="options">The insert options.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract int Insert(List<Row> rows, InsertOptions options = InsertOptions.SkipPrimaryKey);

    /// <summary>Updates a row.</summary>
    /// <param name="row">The row to update.</param>
    /// <param name="reload">Whether the updated row should be reloaded from the database.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract int Update(Row row, bool reload = false);

    /// <summary>Updates a list of rows.</summary>
    /// <param name="rows">The rows to update.</param>
    /// <param name="reload">Whether the updated rows should be reloaded from the database.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract int Update(List<Row> rows, bool reload = false);

    /// <summary>Deletes a row.</summary>
    /// <param name="row">The row to delete.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract int Delete(Row row);

    /// <summary>Deletes a list of rows.</summary>
    /// <param name="rows">The rows to delete.</param>
    /// <returns>The number of rows affected.</returns>
    public abstract int Delete(List<Row> rows);

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
    public List<Row> All()
    {
        return Select(t => "");
    }

    /// <summary>Selects rows using a predicate over the table structure.</summary>
    /// <param name="predicate">A function that creates the predicate for the table.</param>
    /// <param name="parameters">Optional parameter values used by the predicate.</param>
    /// <returns>The selected rows.</returns>
    public abstract List<Row> Select(Func<Structure, string> predicate, object parameters = null);

    /// <summary>Inserts a row when its key is empty, or updates it otherwise.</summary>
    /// <param name="row">The row to store.</param>
    /// <returns>The number of rows affected.</returns>
    public int Store(Row row)
    {
        if (IsNullOrDefault(row.Key))
        {
            return Insert(row);
        }
        return Update(row);
    }

    /// <summary>Inserts rows with empty keys and updates rows with populated keys.</summary>
    /// <param name="rows">The rows to store.</param>
    /// <returns>The number of rows affected.</returns>
    public int Store(List<Row> rows)
    {
        var updates = rows.Where(r => !IsNullOrDefault(r.Key)).ToList();
        var inserts = rows.Where(r => IsNullOrDefault(r.Key)).ToList();
        return Insert(inserts) + Update(updates);
    }
}
