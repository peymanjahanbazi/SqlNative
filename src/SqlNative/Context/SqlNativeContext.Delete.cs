using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlNative;

public abstract partial class SqlNativeContext
{
    public int DeleteById<T, TPk>(TPk pk)
        where T : TableStructure, HasPrimaryKey, new()
    {
        if (pk is null) return 0;
        return DeleteByIds<T, TPk>(new TPk[] { pk });
    }

public int DeleteByIds<T, TPk>(IEnumerable<TPk> pks)
        where T : TableStructure, HasPrimaryKey, new()
    {
        if (pks is null) return 0;

        var ids = pks.ToList();
        if (ids.Count == 0) return 0;

        var batchSize = Math.Max(1, Config?.MaxParametersPerStatement ?? 60_000);
        var affected = 0;
        for (var offset = 0; offset < ids.Count; offset += batchSize)
        {
            var count = Math.Min(batchSize, ids.Count - offset);
            affected += BuildQuery<T>((q, t) =>
            {
                t.DisableAlias = true;
                if (t.PKey == null)
                {
                    throw new InvalidOperationException($"Table '{t.TableName}' does not have a primary key.");
                }

                var parameters = new Dictionary<string, object>();
                var parts = new List<string>(count);
                var idx = 1;
                for (var i = 0; i < count; i++)
                {
                    var paramName = $"@p{idx++}";
                    parameters[paramName] = ids[offset + i]!;
                    parts.Add(paramName);
                }

                return q
                    .Append($"DELETE FROM {t}")
                    .Append($"WHERE {t.PKey} IN ({string.Join(", ", parts)})")
                    .SetParameters(parameters);
            }).ExecuteNonQuery();
        }
        return affected;
    }

    public int DeleteWhere<T>(Func<T, string> predicate, object parameters = null)
        where T : TableStructure, new()
    {
        T t = new T { DisableAlias = true };
        var where = predicate(t);
        return BuildQuery(q => q
            .Append($"DELETE FROM {t}")
            .AppendIf(!string.IsNullOrWhiteSpace(where), $"WHERE {where}")
            .SetParameters(parameters)
        ).ExecuteNonQuery();
    }

    public int DeleteAll<T>() where T : TableStructure, new()
    {
        var t = new T { DisableAlias = true };
        return BuildQuery(q => q.Append($"DELETE FROM {t}")).ExecuteNonQuery();
    }
}