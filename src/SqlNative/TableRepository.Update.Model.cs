namespace SqlNative;


/// <summary>
/// Describes an UPDATE statement and its optional clauses.
/// </summary>
public class UpdateModel
{
    /// <summary>Gets or sets the assignments of the SET clause.</summary>
    public string? Set { get; set; }

    /// <summary>Gets or sets the predicate of the WHERE clause.</summary>
    public string? Where { get; set; }

    /// <summary>Gets or sets the parameter values used by the statement.</summary>
    public object? Parameters { get; set; }
}
