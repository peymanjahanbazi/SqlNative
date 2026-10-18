using System;

namespace SqlNative;

public abstract partial class DefaultContext
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

    private Query GenerateQueryByModel(SelectModel model, Query q)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        if (string.IsNullOrWhiteSpace(model.Select))
        {
            throw new InvalidOperationException("SelectModel.Select cannot be null or empty.");
        }
        if (string.IsNullOrWhiteSpace(model.From))
        {
            throw new InvalidOperationException("SelectModel.From cannot be null or empty.");
        }

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

    public Query Select<T1>(Func<T1, SelectModel> func)
       where T1 : TableStructure, new()
    {
        return BuildQuery<T1>((q, t1) =>
        {
            var model = func(t1);
            return GenerateQueryByModel(model, q);
        }
        );
    }

    public Query Select<T1, T2>(Func<T1, T2, SelectModel> func)
        where T1 : TableStructure, new()
        where T2 : TableStructure, new()
    {
        return BuildQuery<T1, T2>((q, t1, t2) =>
        {
            var model = func(t1, t2);
            return GenerateQueryByModel(model, q);
        }
        );
    }

    public Query Select<T1, T2, T3>(Func<T1, T2, T3, SelectModel> func)
        where T1 : TableStructure, new()
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
    {
        return BuildQuery<T1, T2, T3>((q, t1, t2, t3) =>
        {
            var model = func(t1, t2, t3);
            return GenerateQueryByModel(model, q);
        }
        );
    }

    public Query Select<T1, T2, T3, T4>(Func<T1, T2, T3, T4, SelectModel> func)
        where T1 : TableStructure, new()
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
    {
        return BuildQuery<T1, T2, T3, T4>((q, t1, t2, t3, t4) =>
        {
            var model = func(t1, t2, t3, t4);
            return GenerateQueryByModel(model, q);
        }
        );
    }

    public Query Select<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, SelectModel> func)
        where T1 : TableStructure, new()
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
    {
        return BuildQuery<T1, T2, T3, T4, T5>((q, t1, t2, t3, t4, t5) =>
        {
            var model = func(t1, t2, t3, t4, t5);
            return GenerateQueryByModel(model, q);
        }
        );
    }

    public Query Select<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, SelectModel> func)
        where T1 : TableStructure, new()
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
        where T6 : TableStructure, new()
    {
        return BuildQuery<T1, T2, T3, T4, T5, T6>((q, t1, t2, t3, t4, t5, t6) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6);
            return GenerateQueryByModel(model, q);
        }
        );
    }

    public Query Select<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, SelectModel> func)
        where T1 : TableStructure, new()
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
        where T6 : TableStructure, new()
        where T7 : TableStructure, new()
    {
        return BuildQuery<T1, T2, T3, T4, T5, T6, T7>((q, t1, t2, t3, t4, t5, t6, t7) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7);
            return GenerateQueryByModel(model, q);
        }
        );
    }

    public Query Select<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, SelectModel> func)
        where T1 : TableStructure, new()
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
        where T6 : TableStructure, new()
        where T7 : TableStructure, new()
        where T8 : TableStructure, new()
    {
        return BuildQuery<T1, T2, T3, T4, T5, T6, T7, T8>((q, t1, t2, t3, t4, t5, t6, t7, t8) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8);
            return GenerateQueryByModel(model, q);
        }
        );
    }

    public Query Select<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, SelectModel> func)
        where T1 : TableStructure, new()
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
        where T6 : TableStructure, new()
        where T7 : TableStructure, new()
        where T8 : TableStructure, new()
        where T9 : TableStructure, new()
    {
        return BuildQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9>((q, t1, t2, t3, t4, t5, t6, t7, t8, t9) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8, t9);
            return GenerateQueryByModel(model, q);
        }
        );
    }

    public Query Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, SelectModel> func)
        where T1 : TableStructure, new()
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
        where T6 : TableStructure, new()
        where T7 : TableStructure, new()
        where T8 : TableStructure, new()
        where T9 : TableStructure, new()
        where T10 : TableStructure, new()
    {
        return BuildQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>((q, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
            return GenerateQueryByModel(model, q);
        }
        );
    }

    public Query Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, SelectModel> func)
        where T1 : TableStructure, new()
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
        where T6 : TableStructure, new()
        where T7 : TableStructure, new()
        where T8 : TableStructure, new()
        where T9 : TableStructure, new()
        where T10 : TableStructure, new()
        where T11 : TableStructure, new()
    {
        return BuildQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>((q, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
            return GenerateQueryByModel(model, q);
        }
        );
    }

    public Query Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, SelectModel> func)
        where T1 : TableStructure, new()
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
        where T6 : TableStructure, new()
        where T7 : TableStructure, new()
        where T8 : TableStructure, new()
        where T9 : TableStructure, new()
        where T10 : TableStructure, new()
        where T11 : TableStructure, new()
        where T12 : TableStructure, new()
    {
        return BuildQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>((q, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
            return GenerateQueryByModel(model, q);
        }
        );
    }

    public Query Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, SelectModel> func)
        where T1 : TableStructure, new()
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
        where T6 : TableStructure, new()
        where T7 : TableStructure, new()
        where T8 : TableStructure, new()
        where T9 : TableStructure, new()
        where T10 : TableStructure, new()
        where T11 : TableStructure, new()
        where T12 : TableStructure, new()
        where T13 : TableStructure, new()
    {
        return BuildQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>((q, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
            return GenerateQueryByModel(model, q);
        }
        );
    }

    public Query Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, SelectModel> func)
        where T1 : TableStructure, new()
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
        where T6 : TableStructure, new()
        where T7 : TableStructure, new()
        where T8 : TableStructure, new()
        where T9 : TableStructure, new()
        where T10 : TableStructure, new()
        where T11 : TableStructure, new()
        where T12 : TableStructure, new()
        where T13 : TableStructure, new()
        where T14 : TableStructure, new()
    {
        return BuildQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>((q, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
            return GenerateQueryByModel(model, q);
        }
        );
    }

    public Query Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, SelectModel> func)
        where T1 : TableStructure, new()
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
        where T6 : TableStructure, new()
        where T7 : TableStructure, new()
        where T8 : TableStructure, new()
        where T9 : TableStructure, new()
        where T10 : TableStructure, new()
        where T11 : TableStructure, new()
        where T12 : TableStructure, new()
        where T13 : TableStructure, new()
        where T14 : TableStructure, new()
        where T15 : TableStructure, new()
    {
        return BuildQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>((q, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
            return GenerateQueryByModel(model, q);
        }
        );
    }
}