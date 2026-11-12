namespace SqlNative;

/// <summary>
/// Provides common persistence operations for a Microsoft SQL Server table row and its table structure.
/// </summary>
/// <typeparam name="Row">The row type persisted by this repository.</typeparam>
/// <typeparam name="Table">The table structure type used by this repository.</typeparam>
public abstract partial class TableRepositoryMsSql<Row, Table> : TableRepository<Row, Table>
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
    /// <param name="IsCountQuery"></param>
    /// <returns>The query containing the generated statement.</returns>
    /// <remarks>
    /// The clauses are emitted in the order <c>SELECT</c>, <c>FROM</c>, joins, <c>WHERE</c>,
    /// <c>GROUP BY</c>, <c>HAVING</c>, <c>ORDER BY</c> and the paging clause. Empty clauses are
    /// skipped. When the model has no <see cref="SelectModel.Select"/> expression, all columns of
    /// the table are selected.
    /// </remarks>
    /// <remarks>
    /// SQL Server has no <c>LIMIT</c>. The rows are paged with
    /// <c>OFFSET n ROWS FETCH NEXT m ROWS ONLY</c>, and <c>OFFSET</c> is only allowed together
    /// with an <c>ORDER BY</c>, so the offset is emitted only when the model orders the rows. The
    /// row count alone is expressed with <c>TOP (m)</c> on the <c>SELECT</c> clause.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="model"/> is <see langword="null"/>.</exception>
    protected internal override Query GenerateQueryByModel(SelectModel model, Query q, TableStructure t1, bool IsCountQuery = false)
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

        var hasOrderBy = !string.IsNullOrWhiteSpace(model.OrderBy);
        var hasLimit = !string.IsNullOrEmpty(model.Limit) && !IsCountQuery;
        var hasOffset = !string.IsNullOrEmpty(model.Offset) && !IsCountQuery;

        // A row count without an ORDER BY is expressed with TOP, which needs no paging clause.
        var useTop = hasLimit && !hasOffset;

        if (useTop)
        {
            return q
            .AppendIf(!IsCountQuery, $"SELECT TOP ({model.Limit}) {model.Select}")
            .AppendIf(IsCountQuery, $"SELECT COUNT(*)")
            .Append($"FROM {model.From}")
            .AppendFor(model.Joins, GenerateJoin)
            .AppendIf(!string.IsNullOrWhiteSpace(model.Where), $"WHERE {model.Where}")
            .AppendIf(!string.IsNullOrWhiteSpace(model.GroupBy), $"GROUP BY {model.GroupBy}")
            .AppendIf(!string.IsNullOrWhiteSpace(model.Having), $"HAVING {model.Having}")
            .AppendIf(hasOrderBy, $"ORDER BY {model.OrderBy}")
            .AddParameters(model.Parameters);
        }

        return q
        .AppendIf(!IsCountQuery, $"SELECT {model.Select}")
        .AppendIf(IsCountQuery, $"SELECT COUNT(*)")
        .Append($"FROM {model.From}")
        .AppendFor(model.Joins, GenerateJoin)
        .AppendIf(!string.IsNullOrWhiteSpace(model.Where), $"WHERE {model.Where}")
        .AppendIf(!string.IsNullOrWhiteSpace(model.GroupBy), $"GROUP BY {model.GroupBy}")
        .AppendIf(!string.IsNullOrWhiteSpace(model.Having), $"HAVING {model.Having}")
        .AppendIf(hasOrderBy, $"ORDER BY {model.OrderBy}")
        .AppendIf(hasOffset && hasOrderBy && !IsCountQuery, $"OFFSET {model.Offset} ROWS")
        .AppendIf(hasLimit && !IsCountQuery, $"FETCH NEXT {model.Limit} ROWS ONLY")
        .AddParameters(model.Parameters);
    }
}