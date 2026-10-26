using System.Collections.Generic;

namespace SqlNative
{
    /// <summary>
    /// Describes a SELECT query and its optional clauses.
    /// </summary>
    public class SelectModel
    {
        /// <summary>Gets or sets the expressions to select.</summary>
        public string Select { get; set; }

        /// <summary>Gets or sets the source table or expression to select from.</summary>
        public string From { get; set; }

        /// <summary>Gets or sets the WHERE predicate.</summary>
        public string Where { get; set; }

        /// <summary>Gets or sets the parameter values used by the query.</summary>
        public object Parameters { get; set; }

        /// <summary>Gets or sets the ORDER BY clause.</summary>
        public string OrderBy { get; set; }

        /// <summary>Gets or sets the GROUP BY clause.</summary>
        public string GroupBy { get; set; }

        /// <summary>Gets or sets the HAVING predicate.</summary>
        public string Having { get; set; }

        /// <summary>Gets or sets the maximum number of rows to return.</summary>
        public int? Limit { get; set; }

        /// <summary>Gets or sets the number of rows to skip.</summary>
        public int? Offset { get; set; }

        /// <summary>Gets or sets the joins applied to the query.</summary>
        public List<JoinClause> Joins { get; set; } = new();
    }

    /// <summary>
    /// Base class describing a join in a SELECT query.
    /// </summary>
    public class JoinClause
    {
        /// <summary>Gets or sets the joined table or expression.</summary>
        public string Table { get; set; }

        /// <summary>Gets or sets the join condition.</summary>
        public string On { get; set; }
    }

    /// <summary>
    /// Describes a LEFT JOIN clause.
    /// </summary>
    public class LeftJoin : JoinClause
    {
    }

    /// <summary>
    /// Describes a RIGHT JOIN clause.
    /// </summary>
    public class RightJoin : JoinClause
    {
    }

    /// <summary>
    /// Describes an INNER JOIN clause.
    /// </summary>
    public class InnerJoin : JoinClause
    {
    }

    /// <summary>
    /// Describes a FULL JOIN clause.
    /// </summary>
    public class FullJoin : JoinClause
    {
    }

    /// <summary>
    /// Describes a CROSS JOIN clause.
    /// </summary>
    public class CrossJoin : JoinClause
    {
    }
}
