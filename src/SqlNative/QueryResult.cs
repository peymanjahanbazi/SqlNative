using System;
using System.Data.Common;

namespace SqlNative
{
    /// <summary>
    /// Base class for objects mapped from query result rows.
    /// </summary>
    public abstract class QueryResult
    {
        /// <summary>
        /// Finds the ordinal of a named column occurrence in a data reader.
        /// </summary>
        /// <param name="reader">The reader containing the result columns.</param>
        /// <param name="name">The column name to find.</param>
        /// <param name="occurrence">The zero-based occurrence of the column name.</param>
        /// <returns>The zero-based ordinal of the column occurrence.</returns>
        /// <exception cref="IndexOutOfRangeException">The requested column occurrence is not present.</exception>
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

        /// <summary>
        /// Populates this result object from the current row of a data reader.
        /// </summary>
        /// <param name="reader">The reader positioned on the row to map.</param>
        /// <param name="orders">The column ordinals to map, in property order.</param>
        public abstract void SetFields(DbDataReader reader, int[] orders);

        /// <summary>
        /// Gets the column ordinals used to map result properties.
        /// </summary>
        /// <param name="reader">The reader containing the result columns.</param>
        /// <returns>The column ordinals in property order.</returns>
        public abstract int[] GetOrders(DbDataReader reader);
    }
}
