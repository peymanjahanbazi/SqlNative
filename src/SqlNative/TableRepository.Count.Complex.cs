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
{
    /// <summary>Generates the SELECT statement described by a <see cref="SelectModel"/>.</summary>
    /// <param name="model">The model describing the select statement.</param>
    /// <param name="q">The query being built.</param>
    /// <param name="t1">The main table of the statement.</param>
    /// <returns>The query containing the generated statement.</returns>
    /// <remarks>
    /// Each dialect supplies its own implementation, because the clause order and the
    /// paging syntax differ between engines.
    /// </remarks>
    //abstract internal protected Query GenerateQueryByModel(SelectModel model, Query q, TableStructure t1);

    /// <summary>
    /// Creates a SELECT query from 1 table structure with automatically assigned aliases.
    /// </summary>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count(Func<Table, SelectModel> func)
    {
        var result = Context.BuildQuery<Table>((q, t1) =>
        {
            var model = func(t1);
            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }

    /// <summary>
    /// Creates a SELECT query from 2 table structures with automatically assigned aliases.
    /// </summary>
    /// <typeparam name="T2">The 2 table structure type.</typeparam>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count<T2>(Func<Table, T2, SelectModel> func)

        where T2 : TableStructure, new()
    {
        var result = Context.BuildQuery<Table, T2>((q, t1, t2) =>
        {
            var model = func(t1, t2);

            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }

    /// <summary>
    /// Creates a SELECT query from 3 table structures with automatically assigned aliases.
    /// </summary>
    /// <typeparam name="T2">The 2 table structure type.</typeparam>
    /// <typeparam name="T3">The 3 table structure type.</typeparam>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count<T2, T3>(Func<Table, T2, T3, SelectModel> func)
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
    {
        var result = Context.BuildQuery<Table, T2, T3>((q, t1, t2, t3) =>
        {
            var model = func(t1, t2, t3);

            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }

    /// <summary>
    /// Creates a SELECT query from 4 table structures with automatically assigned aliases.
    /// </summary>

    /// <typeparam name="T2">The 2 table structure type.</typeparam>
    /// <typeparam name="T3">The 3 table structure type.</typeparam>
    /// <typeparam name="T4">The 4 table structure type.</typeparam>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count<T2, T3, T4>(Func<Table, T2, T3, T4, SelectModel> func)
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
    {
        var result = Context.BuildQuery<Table, T2, T3, T4>((q, t1, t2, t3, t4) =>
        {
            var model = func(t1, t2, t3, t4);

            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }

    /// <summary>
    /// Creates a SELECT query from 5 table structures with automatically assigned aliases.
    /// </summary>
    /// <typeparam name="T2">The 2 table structure type.</typeparam>
    /// <typeparam name="T3">The 3 table structure type.</typeparam>
    /// <typeparam name="T4">The 4 table structure type.</typeparam>
    /// <typeparam name="T5">The 5 table structure type.</typeparam>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count<T2, T3, T4, T5>(Func<Table, T2, T3, T4, T5, SelectModel> func)
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
    {
        var result = Context.BuildQuery<Table, T2, T3, T4, T5>((q, t1, t2, t3, t4, t5) =>
        {
            var model = func(t1, t2, t3, t4, t5);

            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }

    /// <summary>
    /// Creates a SELECT query from 6 table structures with automatically assigned aliases.
    /// </summary>
    /// <typeparam name="T2">The 2 table structure type.</typeparam>
    /// <typeparam name="T3">The 3 table structure type.</typeparam>
    /// <typeparam name="T4">The 4 table structure type.</typeparam>
    /// <typeparam name="T5">The 5 table structure type.</typeparam>
    /// <typeparam name="T6">The 6 table structure type.</typeparam>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count<T2, T3, T4, T5, T6>(Func<Table, T2, T3, T4, T5, T6, SelectModel> func)

        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
        where T6 : TableStructure, new()
    {
        var result = Context.BuildQuery<Table, T2, T3, T4, T5, T6>((q, t1, t2, t3, t4, t5, t6) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6);

            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }

    /// <summary>
    /// Creates a SELECT query from 7 table structures with automatically assigned aliases.
    /// </summary>
    /// <typeparam name="T2">The 2 table structure type.</typeparam>
    /// <typeparam name="T3">The 3 table structure type.</typeparam>
    /// <typeparam name="T4">The 4 table structure type.</typeparam>
    /// <typeparam name="T5">The 5 table structure type.</typeparam>
    /// <typeparam name="T6">The 6 table structure type.</typeparam>
    /// <typeparam name="T7">The 7 table structure type.</typeparam>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count<T2, T3, T4, T5, T6, T7>(Func<Table, T2, T3, T4, T5, T6, T7, SelectModel> func)
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
        where T6 : TableStructure, new()
        where T7 : TableStructure, new()
    {
        var result = Context.BuildQuery<Table, T2, T3, T4, T5, T6, T7>((q, t1, t2, t3, t4, t5, t6, t7) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7);

            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }

    /// <summary>
    /// Creates a SELECT query from 8 table structures with automatically assigned aliases.
    /// </summary>
    /// <typeparam name="T2">The 2 table structure type.</typeparam>
    /// <typeparam name="T3">The 3 table structure type.</typeparam>
    /// <typeparam name="T4">The 4 table structure type.</typeparam>
    /// <typeparam name="T5">The 5 table structure type.</typeparam>
    /// <typeparam name="T6">The 6 table structure type.</typeparam>
    /// <typeparam name="T7">The 7 table structure type.</typeparam>
    /// <typeparam name="T8">The 8 table structure type.</typeparam>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count<T2, T3, T4, T5, T6, T7, T8>(Func<Table, T2, T3, T4, T5, T6, T7, T8, SelectModel> func)
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
        where T6 : TableStructure, new()
        where T7 : TableStructure, new()
        where T8 : TableStructure, new()
    {
        var result = Context.BuildQuery<Table, T2, T3, T4, T5, T6, T7, T8>((q, t1, t2, t3, t4, t5, t6, t7, t8) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8);

            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }

    /// <summary>
    /// Creates a SELECT query from 9 table structures with automatically assigned aliases.
    /// </summary>
    /// <typeparam name="T2">The 2 table structure type.</typeparam>
    /// <typeparam name="T3">The 3 table structure type.</typeparam>
    /// <typeparam name="T4">The 4 table structure type.</typeparam>
    /// <typeparam name="T5">The 5 table structure type.</typeparam>
    /// <typeparam name="T6">The 6 table structure type.</typeparam>
    /// <typeparam name="T7">The 7 table structure type.</typeparam>
    /// <typeparam name="T8">The 8 table structure type.</typeparam>
    /// <typeparam name="T9">The 9 table structure type.</typeparam>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count<T2, T3, T4, T5, T6, T7, T8, T9>(Func<Table, T2, T3, T4, T5, T6, T7, T8, T9, SelectModel> func)
        where T2 : TableStructure, new()
        where T3 : TableStructure, new()
        where T4 : TableStructure, new()
        where T5 : TableStructure, new()
        where T6 : TableStructure, new()
        where T7 : TableStructure, new()
        where T8 : TableStructure, new()
        where T9 : TableStructure, new()
    {
        var result = Context.BuildQuery<Table, T2, T3, T4, T5, T6, T7, T8, T9>((q, t1, t2, t3, t4, t5, t6, t7, t8, t9) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8, t9);

            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }

    /// <summary>
    /// Creates a SELECT query from 10 table structures with automatically assigned aliases.
    /// </summary>
    /// <typeparam name="T2">The 2 table structure type.</typeparam>
    /// <typeparam name="T3">The 3 table structure type.</typeparam>
    /// <typeparam name="T4">The 4 table structure type.</typeparam>
    /// <typeparam name="T5">The 5 table structure type.</typeparam>
    /// <typeparam name="T6">The 6 table structure type.</typeparam>
    /// <typeparam name="T7">The 7 table structure type.</typeparam>
    /// <typeparam name="T8">The 8 table structure type.</typeparam>
    /// <typeparam name="T9">The 9 table structure type.</typeparam>
    /// <typeparam name="T10">The 10 table structure type.</typeparam>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count<T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<Table, T2, T3, T4, T5, T6, T7, T8, T9, T10, SelectModel> func)
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
        var result = Context.BuildQuery<Table, T2, T3, T4, T5, T6, T7, T8, T9, T10>((q, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);

            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }

    /// <summary>
    /// Creates a SELECT query from 11 table structures with automatically assigned aliases.
    /// </summary>
    /// <typeparam name="T2">The 2 table structure type.</typeparam>
    /// <typeparam name="T3">The 3 table structure type.</typeparam>
    /// <typeparam name="T4">The 4 table structure type.</typeparam>
    /// <typeparam name="T5">The 5 table structure type.</typeparam>
    /// <typeparam name="T6">The 6 table structure type.</typeparam>
    /// <typeparam name="T7">The 7 table structure type.</typeparam>
    /// <typeparam name="T8">The 8 table structure type.</typeparam>
    /// <typeparam name="T9">The 9 table structure type.</typeparam>
    /// <typeparam name="T10">The 10 table structure type.</typeparam>
    /// <typeparam name="T11">The 11 table structure type.</typeparam>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<Table, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, SelectModel> func)
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
        var result = Context.BuildQuery<Table, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>((q, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);

            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }

    /// <summary>
    /// Creates a SELECT query from 12 table structures with automatically assigned aliases.
    /// </summary>
    /// <typeparam name="T2">The 2 table structure type.</typeparam>
    /// <typeparam name="T3">The 3 table structure type.</typeparam>
    /// <typeparam name="T4">The 4 table structure type.</typeparam>
    /// <typeparam name="T5">The 5 table structure type.</typeparam>
    /// <typeparam name="T6">The 6 table structure type.</typeparam>
    /// <typeparam name="T7">The 7 table structure type.</typeparam>
    /// <typeparam name="T8">The 8 table structure type.</typeparam>
    /// <typeparam name="T9">The 9 table structure type.</typeparam>
    /// <typeparam name="T10">The 10 table structure type.</typeparam>
    /// <typeparam name="T11">The 11 table structure type.</typeparam>
    /// <typeparam name="T12">The 12 table structure type.</typeparam>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<Table, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, SelectModel> func)
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
        var result = Context.BuildQuery<Table, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>((q, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);

            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }

    /// <summary>
    /// Creates a SELECT query from 13 table structures with automatically assigned aliases.
    /// </summary>
    /// <typeparam name="T2">The 2 table structure type.</typeparam>
    /// <typeparam name="T3">The 3 table structure type.</typeparam>
    /// <typeparam name="T4">The 4 table structure type.</typeparam>
    /// <typeparam name="T5">The 5 table structure type.</typeparam>
    /// <typeparam name="T6">The 6 table structure type.</typeparam>
    /// <typeparam name="T7">The 7 table structure type.</typeparam>
    /// <typeparam name="T8">The 8 table structure type.</typeparam>
    /// <typeparam name="T9">The 9 table structure type.</typeparam>
    /// <typeparam name="T10">The 10 table structure type.</typeparam>
    /// <typeparam name="T11">The 11 table structure type.</typeparam>
    /// <typeparam name="T12">The 12 table structure type.</typeparam>
    /// <typeparam name="T13">The 13 table structure type.</typeparam>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<Table, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, SelectModel> func)
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
        var result = Context.BuildQuery<Table, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>((q, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);

            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }

    /// <summary>
    /// Creates a SELECT query from 14 table structures with automatically assigned aliases.
    /// </summary>
    /// <typeparam name="T2">The 2 table structure type.</typeparam>
    /// <typeparam name="T3">The 3 table structure type.</typeparam>
    /// <typeparam name="T4">The 4 table structure type.</typeparam>
    /// <typeparam name="T5">The 5 table structure type.</typeparam>
    /// <typeparam name="T6">The 6 table structure type.</typeparam>
    /// <typeparam name="T7">The 7 table structure type.</typeparam>
    /// <typeparam name="T8">The 8 table structure type.</typeparam>
    /// <typeparam name="T9">The 9 table structure type.</typeparam>
    /// <typeparam name="T10">The 10 table structure type.</typeparam>
    /// <typeparam name="T11">The 11 table structure type.</typeparam>
    /// <typeparam name="T12">The 12 table structure type.</typeparam>
    /// <typeparam name="T13">The 13 table structure type.</typeparam>
    /// <typeparam name="T14">The 14 table structure type.</typeparam>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<Table, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, SelectModel> func)
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
        var result = Context.BuildQuery<Table, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>((q, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);

            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }

    /// <summary>
    /// Creates a SELECT query from 15 table structures with automatically assigned aliases.
    /// </summary>
    /// <typeparam name="T2">The 2 table structure type.</typeparam>
    /// <typeparam name="T3">The 3 table structure type.</typeparam>
    /// <typeparam name="T4">The 4 table structure type.</typeparam>
    /// <typeparam name="T5">The 5 table structure type.</typeparam>
    /// <typeparam name="T6">The 6 table structure type.</typeparam>
    /// <typeparam name="T7">The 7 table structure type.</typeparam>
    /// <typeparam name="T8">The 8 table structure type.</typeparam>
    /// <typeparam name="T9">The 9 table structure type.</typeparam>
    /// <typeparam name="T10">The 10 table structure type.</typeparam>
    /// <typeparam name="T11">The 11 table structure type.</typeparam>
    /// <typeparam name="T12">The 12 table structure type.</typeparam>
    /// <typeparam name="T13">The 13 table structure type.</typeparam>
    /// <typeparam name="T14">The 14 table structure type.</typeparam>
    /// <typeparam name="T15">The 15 table structure type.</typeparam>
    /// <param name="func">A function that creates the select model from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public long Count<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<Table, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, SelectModel> func)
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
        var result = Context.BuildQuery<Table, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>((q, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) =>
        {
            var model = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);

            return GenerateQueryByModel(model, q, t1, true);
        }
        ).ExecuteScalar();
        return Convert.ToInt64(result);
    }
}