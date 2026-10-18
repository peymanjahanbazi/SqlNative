namespace SqlNative
{
    public class QueryPart
    {
        private object rawQuery;

        public QueryPart(TableStructure table)
        {
            this.rawQuery = table;
        }

        public QueryPart(TableColumn column)
        {
            this.rawQuery = column;
        }

        public QueryPart(string rawQuery)
        {
            this.rawQuery = rawQuery;
        }

        public override string ToString()
        {
            return rawQuery.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is QueryPart other)
                return ToString() == other.ToString();
            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public QueryPart this[object index]
        {
            get { return new QueryPart($"{this}[{index}]"); }
        }

        public static QueryPart operator +(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} + {b}");
        }

        public static QueryPart operator -(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} - {b}");
        }

        public static QueryPart operator *(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} * {b}");
        }

        public static QueryPart operator /(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} / {b}");
        }

        public static QueryPart operator %(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} % {b}");
        }

        public static QueryPart operator &(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} & {b}");
        }

        public static QueryPart operator |(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} | {b}");
        }

        public static QueryPart operator ^(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} ^ {b}");
        }

        public static QueryPart operator <<(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} << {b}");
        }

        public static QueryPart operator >>(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} >> {b}");
        }

        public static QueryPart operator +(QueryPart a)
        {
            return new QueryPart($"+{a}");
        }

        public static QueryPart operator -(QueryPart a)
        {
            return new QueryPart($"-{a}");
        }

        public static QueryPart operator !(QueryPart a)
        {
            return new QueryPart($"NOT {a}");
        }

        public static QueryPart operator ~(QueryPart a)
        {
            return new QueryPart($"~{a}");
        }

        public static QueryPart operator ==(QueryPart a, QueryPart b)
        {
            if (b.ToString() == "null")
            {
                return new QueryPart($"{a} IS NULL");
            }
            if (a.ToString() == "null")
            {
                return new QueryPart($"{b} IS NULL");
            }
            return new QueryPart($"{a} = {b}");
        }

        public static QueryPart operator !=(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} <> {b}");
        }

        public static QueryPart operator <(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} < {b}");
        }

        public static QueryPart operator >(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} > {b}");
        }

        public static QueryPart operator <=(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} <= {b}");
        }

        public static QueryPart operator >=(QueryPart a, QueryPart b)
        {
            return new QueryPart($"{a} >= {b}");
        }

        public static bool operator true(QueryPart a)
        {
            var r = a?.rawQuery;
            return r != null && !string.IsNullOrEmpty(r.ToString());
        }

        public static bool operator false(QueryPart a)
        {
            var r = a?.rawQuery;
            return r == null || string.IsNullOrEmpty(r.ToString());
        }

        public static implicit operator QueryPart(string value)
        {
            if (value == null)
                return new QueryPart("");
            return new QueryPart(value);
        }

        public static implicit operator QueryPart(TableColumn column)
        {
            if (column == null)
                return new QueryPart("");
            return new QueryPart(column);
        }

        public static implicit operator QueryPart(TableStructure table)
        {
            if (table == null)
                return new QueryPart("");
            return new QueryPart(table);
        }

        public static implicit operator QueryPart(sbyte value)
        {
            return new QueryPart(value.ToString());
        }

        public static implicit operator QueryPart(byte value)
        {
            return new QueryPart(value.ToString());
        }

        public static implicit operator QueryPart(short value)
        {
            return new QueryPart(value.ToString());
        }

        public static implicit operator QueryPart(ushort value)
        {
            return new QueryPart(value.ToString());
        }

        public static implicit operator QueryPart(int value)
        {
            return new QueryPart(value.ToString());
        }

        public static implicit operator QueryPart(uint value)
        {
            return new QueryPart(value.ToString());
        }

        public static implicit operator QueryPart(long value)
        {
            return new QueryPart(value.ToString());
        }

        public static implicit operator QueryPart(ulong value)
        {
            return new QueryPart(value.ToString());
        }

        public static implicit operator QueryPart(float value)
        {
            return new QueryPart(value.ToString());
        }

        public static implicit operator QueryPart(double value)
        {
            return new QueryPart(value.ToString());
        }

        public static implicit operator QueryPart(decimal value)
        {
            return new QueryPart(value.ToString());
        }

        public static implicit operator QueryPart(bool value)
        {
            return new QueryPart(value ? "TRUE" : "FALSE");
        }

        public static implicit operator QueryPart(char value)
        {
            return new QueryPart(value.ToString());
        }
    }
}