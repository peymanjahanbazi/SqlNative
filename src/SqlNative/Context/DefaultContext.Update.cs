using System;

namespace SqlNative;

public abstract partial class DefaultContext
{
    public Query Update<T1>(Func<T1, UpdateModel> func)
       where T1 : TableStructure, new()
    {
        T1 t1 = new T1
        {
            DisableAlias = true
        };

        var model = func(t1);

        return BuildQuery(q => q
            .Append($"UPDATE {t1} ")
            .Append($"SET {model.Set}")
            .Append($"WHERE {model.Where}")
            .SetParameters(model.Parameters)
        );
    }
}