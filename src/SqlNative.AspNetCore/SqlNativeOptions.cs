namespace SqlNative.AspNetCore;

/// <summary>
/// Configures the registration and behavior of a SqlNative context in an ASP.NET Core host.
/// </summary>
public class SqlNativeOptions
{
    /// <summary>
    /// Gets or sets whether development features such as model generation are enabled.
    /// </summary>
    public bool IsDevelopMode { get; set; } = false;

    /// <summary>
    /// Gets or sets whether SQL commands are written to the host's logger.
    /// </summary>
    public bool LogSqlCommands { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum number of parameters allowed in one statement.
    /// </summary>
    public int MaxParametersPerStatement { get; set; } = 60_000;

    /// <summary>
    /// Gets or sets the maximum packet size used for database communication.
    /// </summary>
    public int MaxPacketSize { get; set; } = 1_000_000;

    /// <summary>
    /// Gets or sets the connection string used by the registered database connection.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    //public ConnectionCreatePolicy ConnectionCreatePolicy { get; set; } = ConnectionCreatePolicy.ConnectionPool;
    //public int ConnectionPoolSize { get; set; } = 10;
}
