using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlNative;

public enum InsertOptions
{
    LoadRecord = 1,
    SkipPrimaryKey = 2,
    LoadAndSkip = 3
}

public abstract class DbEntity<Row, Structure, Pk>
    where Row : TableRow<Pk>, new()
    where Structure : TableStructure, new()
{
    public abstract int Insert(Row row, InsertOptions options = InsertOptions.SkipPrimaryKey);

    public abstract int Insert(List<Row> rows, InsertOptions options = InsertOptions.SkipPrimaryKey);

    public abstract int Update(Row row, bool reload = false);

    public abstract int Update(List<Row> rows, bool reload = false);

    public abstract int Delete(Row row);

    public abstract int Delete(List<Row> rows);

    protected bool IsNullOrDefault(Pk value)
    {
        if (value == null) return true;
        return EqualityComparer<Pk>.Default.Equals(value, default(Pk));
    }

    public List<Row> All()
    {
        return Select(t => "");
    }

    public abstract List<Row> Select(Func<Structure, string> predicate, object parameters = null);

    public int Store(Row row)
    {
        if (IsNullOrDefault(row.Key))
        {
            return Insert(row);
        }
        return Update(row);
    }

    public int Store(List<Row> rows)
    {
        var updates = rows.Where(r => !IsNullOrDefault(r.Key)).ToList();
        var inserts = rows.Where(r => IsNullOrDefault(r.Key)).ToList();
        return Insert(inserts) + Update(updates);
    }
}