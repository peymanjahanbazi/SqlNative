namespace SqlNative;

/// <summary>
/// Specifies how an inserted record is handled and whether its primary key is included.
/// </summary>
public enum InsertOptions
{
    /// <summary>Loads the inserted record and includes its generated primary key.</summary>
    LoadRecord = 1,

    /// <summary>Skips the primary key when creating the insert statement.</summary>
    SkipPrimaryKey = 2,

    /// <summary>Loads the inserted record while skipping the primary key in the insert statement.</summary>
    LoadAndSkip = 3
}