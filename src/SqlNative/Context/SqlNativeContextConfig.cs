namespace SqlNative;

/// <summary>
/// Configures the behavior of a <see cref="SqlNativeContext"/>.
/// </summary>
public class SqlNativeContextConfig
{
    /// <summary>
    /// Gets or sets whether development features such as model generation are enabled.
    /// </summary>
    public bool IsDevelopMode { get; set; }

    /// <summary>
    /// Gets or sets whether SQL commands are written to the console.
    /// </summary>
    public bool LogSqlCommands { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of parameters allowed in one statement.
    /// </summary>
    public int MaxParametersPerStatement { get; set; }

    /// <summary>
    /// Gets or sets the maximum packet size used for database communication.
    /// </summary>
    public int MaxPacketSize { get; set; }
}
