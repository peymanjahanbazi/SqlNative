namespace SqlNative;

public abstract partial class TableRepositoryMySql<Row, Table> : TableRepository<Row, Table>
    where Row : TableRow, new()
    where Table : TableStructure, new()
{
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

    internal protected override Query GenerateQueryByModel(SelectModel model, Query q, TableStructure t1)
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
        .AppendIf(model.Limit.HasValue, $"LIMIT {model.Limit}")
        .AppendIf(model.Limit.HasValue && model.Offset.HasValue, $"OFFSET {model.Offset}")
        .SetParameters(model.Parameters);
    }
}