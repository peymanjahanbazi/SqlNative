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

    /// <summary>Generates the SELECT statement described by a <see cref="SelectModel"/>.</summary>
    /// <param name="model">The model describing the select statement.</param>
    /// <param name="q">The query being built.</param>
    /// <param name="t1">The main table of the statement.</param>
    /// <returns>The query containing the generated statement.</returns>
    /// <exception cref="NotImplementedException">Always thrown; this dialect is not supported yet.</exception>
    internal protected override Query GenerateQueryByModel(SelectModel model, Query q, TableStructure t1)
    {
        throw new NotImplementedException("Not Implemented for Mssql");
    }
}
