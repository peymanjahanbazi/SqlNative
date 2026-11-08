namespace SqlNative;

/// <summary>
/// Provides common persistence operations for a MySQL table row and its table structure.
/// </summary>
/// <typeparam name="Row">The row type persisted by this repository.</typeparam>
/// <typeparam name="Table">The table structure type used by this repository.</typeparam>
public abstract partial class TableRepositoryMySql<Row, Table> : TableRepository<Row, Table>
    where Row : TableRow, new()
    where Table : TableStructure, new()
{
    /// <summary>Renders a join clause as SQL text.</summary>
    /// <param name="join">The join to render.</param>
    /// <returns>The SQL text of the join clause.</returns>
    /// <exception cref="NotSupportedException">The join type is not supported by this dialect.</exception>
    private string GenerateJoin(JoinClause join)
    {
        if (join is LeftJoin)
        {
            return $"LEFT JOIN {join.Table} ON {join.On}";
        }
        else if (join is RightJoin)
        {
            return $"RIGHT JOIN {join.Table} ON {join.On}";
        }
        else if (join is InnerJoin)
        {
            return $"INNER JOIN {join.Table} ON {join.On}";
        }
        else if (join is FullJoin)
        {
            return $"FULL JOIN {join.Table} ON {join.On}";
        }
        else if (join is CrossJoin)
        {
            return $"CROSS JOIN {join.Table}";
        }
        else
        {
            throw new NotSupportedException($"Join type '{join?.GetType().Name ?? "null"}' is not supported.");
        }
    }

    /// <summary>Generates the SELECT statement described by a <see cref="SelectModel"/>.</summary>
    /// <param name="model">The model describing the select statement.</param>
    /// <param name="q">The query being built.</param>
    /// <param name="t1">The main table of the statement.</param>
    /// <returns>The query containing the generated statement.</returns>
    /// <remarks>
    /// The clauses are emitted in the order <c>SELECT</c>, <c>FROM</c>, joins, <c>WHERE</c>,
    /// <c>GROUP BY</c>, <c>HAVING</c>, <c>ORDER BY</c> and the paging clause. Empty clauses are
    /// skipped. When the model has no <see cref="SelectModel.Select"/> expression, all columns of
    /// the table are selected.
    /// </remarks>
    /// <remarks>
    /// MySQL has no standalone <c>OFFSET</c> clause: the offset is part of the <c>LIMIT</c> clause,
    /// which the server spells <c>LIMIT row_count OFFSET offset</c>. An offset on its own is a
    /// syntax error, so it is only emitted together with a row count.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="model"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The limit or the offset is negative.</exception>
    protected internal override Query GenerateQueryByModel(SelectModel model, Query q, TableStructure t1)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        if (string.IsNullOrWhiteSpace(model.Select))
        {
            model.Select = t1.Star;
        }
        model.From = t1;

        return q
        .Append($"SELECT {model.Select}")
        .Append($"FROM {model.From}")
        .AppendFor(model.Joins, GenerateJoin)
        .AppendIf(!string.IsNullOrWhiteSpace(model.Where), $"WHERE {model.Where}")
        .AppendIf(!string.IsNullOrWhiteSpace(model.GroupBy), $"GROUP BY {model.GroupBy}")
        .AppendIf(!string.IsNullOrWhiteSpace(model.Having), $"HAVING {model.Having}")
        .AppendIf(!string.IsNullOrWhiteSpace(model.OrderBy), $"ORDER BY {model.OrderBy}")
        .AppendIf(model.Limit.HasValue || (model.Offset ?? 0) > 0, GenerateLimit(model))
        .AddParameters(model.Parameters);
    }

    /// <summary>Renders the MySQL paging clause.</summary>
    /// <param name="model">The model describing the select statement.</param>
    /// <returns>The SQL text of the paging clause.</returns>
    /// <remarks>
    /// MySQL spells the row count and the offset as a single clause, <c>LIMIT row_count OFFSET
    /// offset</c>, and it rejects a negative value for either of them. An offset on its own is not
    /// valid MySQL, so when the model carries an offset but no row count, the largest row count
    /// MySQL accepts is used: the offset still skips the rows and the count limits nothing. An
    /// offset of zero skips nothing, so it needs no clause of its own.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">The limit or the offset is negative.</exception>
    private static string GenerateLimit(SelectModel model)
    {
        if (model.Limit < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(model), model.Limit, "The limit must not be negative.");
        }
        if (model.Offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(model), model.Offset, "The offset must not be negative.");
        }

        // The offset cannot be sent on its own, so it is paired with a row count. A row count of
        // zero would return no rows at all, so the largest one MySQL accepts is used instead.
        // The value does not fit in a long, so the row count is kept as text.
        string limit = model.Limit?.ToString() ?? "18446744073709551615";

        return model.Offset > 0
            ? $"LIMIT {limit} OFFSET {model.Offset}"
            : $"LIMIT {limit}";
    }
}