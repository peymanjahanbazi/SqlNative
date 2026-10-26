namespace SqlNative;

/// <summary>
/// Describes an UPDATE statement.
/// </summary>
public class UpdateModel
{
    /// <summary>Gets or sets the assignments in the SET clause.</summary>
    public string Set { get; set; }

    /// <summary>Gets or sets the WHERE predicate.</summary>
    public string Where { get; set; }

    /// <summary>Gets or sets the parameter values used by the statement.</summary>
    public object Parameters { get; set; }
}
