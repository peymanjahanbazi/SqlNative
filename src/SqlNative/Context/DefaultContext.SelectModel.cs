using System.Collections.Generic;

namespace SqlNative
{
    public class SelectModel
    {
        public string Select { get; set; }
        public string From { get; set; }
        public string Where { get; set; }
        public object Parameters { get; set; }
        public string OrderBy { get; set; }
        public string GroupBy { get; set; }
        public string Having { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public List<JoinClause> Joins { get; set; } = new();
    }

    public class JoinClause
    {
        public string Table { get; set; }
        public string On { get; set; }
    }

    public class LeftJoin : JoinClause
    {
    }

    public class RightJoin : JoinClause
    {
    }

    public class InnerJoin : JoinClause
    {
    }

    public class FullJoin : JoinClause
    {
    }

    public class CrossJoin : JoinClause
    {
    }
}