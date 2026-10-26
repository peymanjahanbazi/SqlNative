namespace SqlNative
{
    /// <summary>
    /// Represents an SQL expression fragment that can be composed using operators.
    /// </summary>
    public class QueryPart
    {
        private object rawQuery;

        /// <summary>
        /// Initializes a new query part that references a table.
        /// </summary>
        /// <param name="table">The table represented by this query part.</param>
        public QueryPart(TableStructure table)
        {
            this.rawQuery = table;
        }

        /// <summary>
        /// Initializes a new query part that references a column.
        /// </summary>
        /// <param name="column">The column represented by this query part.</param>
        public QueryPart(TableColumn column)
        {
            this.rawQuery = column;
        }

        /// <summary>
        /// Initializes a new query part from raw SQL text.
        /// </summary>
        /// <param name="rawQuery">The SQL text represented by this query part.</param>
        public QueryPart(string rawQuery)
        {
            this.rawQuery = rawQuery;
        }

        /// <summary>
        /// Returns the SQL text represented by this query part.
        /// </summary>
        /// <returns>The SQL text of this query part.</returns>
        public override string ToString()
        {
            return rawQuery.ToString();
        }

        /// <summary>
        /// Determines whether this query part represents the same SQL text as another object.
        /// </summary>
        /// <param name="obj">The object to compare with this query part.</param>
        /// <returns><see langword="true"/> when the SQL text is equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is QueryPart other)
                return ToString() == other.ToString();
            return false;
        }

        /// <summary>
        /// Returns a hash code based on the SQL text represented by this query part.
        /// </summary>
        /// <returns>A hash code for this query part.</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Gets a query part that indexes this part.
        /// </summary>
        /// <param name="index">The index expression.</param>
        /// <returns>A query part containing the indexed expression.</returns>
        public QueryPart this[object index]
        {
            get { return new QueryPart($"{this}[{index}]"); }
        }

        /// <summary>Creates an addition expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed addition expression.</returns>
        public static QueryPart operator +(QueryPart a, QueryPart b) => new QueryPart($"{a} + {b}");

        /// <summary>Creates a subtraction expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed subtraction expression.</returns>
        public static QueryPart operator -(QueryPart a, QueryPart b) => new QueryPart($"{a} - {b}");

        /// <summary>Creates a multiplication expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed multiplication expression.</returns>
        public static QueryPart operator *(QueryPart a, QueryPart b) => new QueryPart($"{a} * {b}");

        /// <summary>Creates a division expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed division expression.</returns>
        public static QueryPart operator /(QueryPart a, QueryPart b) => new QueryPart($"{a} / {b}");

        /// <summary>Creates a modulo expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed modulo expression.</returns>
        public static QueryPart operator %(QueryPart a, QueryPart b) => new QueryPart($"{a} % {b}");

        /// <summary>Creates a bitwise AND expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed bitwise AND expression.</returns>
        public static QueryPart operator &(QueryPart a, QueryPart b) => new QueryPart($"{a} & {b}");

        /// <summary>Creates a bitwise OR expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed bitwise OR expression.</returns>
        public static QueryPart operator |(QueryPart a, QueryPart b) => new QueryPart($"{a} | {b}");

        /// <summary>Creates a bitwise XOR expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed bitwise XOR expression.</returns>
        public static QueryPart operator ^(QueryPart a, QueryPart b) => new QueryPart($"{a} ^ {b}");

        /// <summary>Creates a left-shift expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed left-shift expression.</returns>
        public static QueryPart operator <<(QueryPart a, QueryPart b) => new QueryPart($"{a} << {b}");

        /// <summary>Creates a right-shift expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed right-shift expression.</returns>
        public static QueryPart operator >>(QueryPart a, QueryPart b) => new QueryPart($"{a} >> {b}");

        /// <summary>Creates a unary plus expression.</summary>
        /// <param name="a">The operand.</param>
        /// <returns>The composed unary plus expression.</returns>
        public static QueryPart operator +(QueryPart a) => new QueryPart($"+{a}");

        /// <summary>Creates a unary minus expression.</summary>
        /// <param name="a">The operand.</param>
        /// <returns>The composed unary minus expression.</returns>
        public static QueryPart operator -(QueryPart a) => new QueryPart($"-{a}");

        /// <summary>Creates a logical NOT expression.</summary>
        /// <param name="a">The operand.</param>
        /// <returns>The composed NOT expression.</returns>
        public static QueryPart operator !(QueryPart a) => new QueryPart($"NOT {a}");

        /// <summary>Creates a bitwise complement expression.</summary>
        /// <param name="a">The operand.</param>
        /// <returns>The composed bitwise complement expression.</returns>
        public static QueryPart operator ~(QueryPart a) => new QueryPart($"~{a}");

        /// <summary>
        /// Creates an equality expression. When either operand represents <c>null</c>,
        /// the result is an <c>IS NULL</c> expression.
        /// </summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed equality or null-check expression.</returns>
        public static QueryPart operator ==(QueryPart a, QueryPart b)
        {
            if (b.ToString() == "null")
                return new QueryPart($"{a} IS NULL");
            if (a.ToString() == "null")
                return new QueryPart($"{b} IS NULL");
            return new QueryPart($"{a} = {b}");
        }

        /// <summary>Creates an inequality expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed inequality expression.</returns>
        public static QueryPart operator !=(QueryPart a, QueryPart b) => new QueryPart($"{a} <> {b}");

        /// <summary>Creates a less-than expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed less-than expression.</returns>
        public static QueryPart operator <(QueryPart a, QueryPart b) => new QueryPart($"{a} < {b}");

        /// <summary>Creates a greater-than expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed greater-than expression.</returns>
        public static QueryPart operator >(QueryPart a, QueryPart b) => new QueryPart($"{a} > {b}");

        /// <summary>Creates a less-than-or-equal expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed less-than-or-equal expression.</returns>
        public static QueryPart operator <=(QueryPart a, QueryPart b) => new QueryPart($"{a} <= {b}");

        /// <summary>Creates a greater-than-or-equal expression.</summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>The composed greater-than-or-equal expression.</returns>
        public static QueryPart operator >=(QueryPart a, QueryPart b) => new QueryPart($"{a} >= {b}");

        /// <summary>Tests whether a query part represents a non-empty expression.</summary>
        /// <param name="a">The query part to test.</param>
        /// <returns><see langword="true"/> when the expression is non-empty; otherwise, <see langword="false"/>.</returns>
        public static bool operator true(QueryPart a)
        {
            var r = a?.rawQuery;
            return r != null && !string.IsNullOrEmpty(r.ToString());
        }

        /// <summary>Tests whether a query part represents an empty expression.</summary>
        /// <param name="a">The query part to test.</param>
        /// <returns><see langword="true"/> when the expression is empty; otherwise, <see langword="false"/>.</returns>
        public static bool operator false(QueryPart a)
        {
            var r = a?.rawQuery;
            return r == null || string.IsNullOrEmpty(r.ToString());
        }

        /// <summary>Converts text to a query part.</summary>
        /// <param name="value">The SQL text to convert.</param>
        /// <returns>A query part containing the text.</returns>
        public static implicit operator QueryPart(string value) => new QueryPart(value ?? "");

        /// <summary>Converts a column to a query part.</summary>
        /// <param name="column">The column to convert.</param>
        /// <returns>A query part referencing the column.</returns>
        public static implicit operator QueryPart(TableColumn column) => new QueryPart(column ?? "");

        /// <summary>Converts a table to a query part.</summary>
        /// <param name="table">The table to convert.</param>
        /// <returns>A query part referencing the table.</returns>
        public static implicit operator QueryPart(TableStructure table) => new QueryPart(table ?? "");

        /// <summary>Converts an <see cref="sbyte"/> to a query part.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A query part containing the value.</returns>
        public static implicit operator QueryPart(sbyte value) => new QueryPart(value.ToString());

        /// <summary>Converts a <see cref="byte"/> to a query part.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A query part containing the value.</returns>
        public static implicit operator QueryPart(byte value) => new QueryPart(value.ToString());

        /// <summary>Converts a <see cref="short"/> to a query part.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A query part containing the value.</returns>
        public static implicit operator QueryPart(short value) => new QueryPart(value.ToString());

        /// <summary>Converts a <see cref="ushort"/> to a query part.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A query part containing the value.</returns>
        public static implicit operator QueryPart(ushort value) => new QueryPart(value.ToString());

        /// <summary>Converts an <see cref="int"/> to a query part.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A query part containing the value.</returns>
        public static implicit operator QueryPart(int value) => new QueryPart(value.ToString());

        /// <summary>Converts a <see cref="uint"/> to a query part.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A query part containing the value.</returns>
        public static implicit operator QueryPart(uint value) => new QueryPart(value.ToString());

        /// <summary>Converts a <see cref="long"/> to a query part.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A query part containing the value.</returns>
        public static implicit operator QueryPart(long value) => new QueryPart(value.ToString());

        /// <summary>Converts a <see cref="ulong"/> to a query part.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A query part containing the value.</returns>
        public static implicit operator QueryPart(ulong value) => new QueryPart(value.ToString());

        /// <summary>Converts a <see cref="float"/> to a query part.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A query part containing the value.</returns>
        public static implicit operator QueryPart(float value) => new QueryPart(value.ToString());

        /// <summary>Converts a <see cref="double"/> to a query part.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A query part containing the value.</returns>
        public static implicit operator QueryPart(double value) => new QueryPart(value.ToString());

        /// <summary>Converts a <see cref="decimal"/> to a query part.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A query part containing the value.</returns>
        public static implicit operator QueryPart(decimal value) => new QueryPart(value.ToString());

        /// <summary>Converts a <see cref="bool"/> to a query part.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A query part containing <c>TRUE</c> or <c>FALSE</c>.</returns>
        public static implicit operator QueryPart(bool value) => new QueryPart(value ? "TRUE" : "FALSE");

        /// <summary>Converts a <see cref="char"/> to a query part.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A query part containing the value.</returns>
        public static implicit operator QueryPart(char value) => new QueryPart(value.ToString());
    }
}
