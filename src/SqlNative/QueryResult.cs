using System;
using System.Data.Common;

namespace SqlNative
{
    public abstract class QueryResult
    {
        protected static int OrdinalN(DbDataReader reader, string name, int occurrence)
        {
            int found = 0;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), name, StringComparison.OrdinalIgnoreCase))
                {
                    if (found == occurrence) return i;
                    found++;
                }
            }
            throw new IndexOutOfRangeException($"Column '{name}' occurrence {occurrence} not found in reader.");
        }

        public abstract void SetFields(DbDataReader reader, int[] orders);

        public abstract int[] GetOrders(DbDataReader reader);
    }
}