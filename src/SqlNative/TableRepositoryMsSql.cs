namespace SqlNative;

public abstract partial class TableRepositoryMsSql<Row, Table> : TableRepository<Row, Table>
    where Row : TableRow, new()
    where Table : TableStructure, new()
{

    internal protected override Query GenerateQueryByModel(SelectModel model, Query q, TableStructure t1)
    {
        throw new NotImplementedException("Not Implemented for Mssql");
    }
}