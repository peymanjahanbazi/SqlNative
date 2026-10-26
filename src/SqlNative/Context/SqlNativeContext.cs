using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace SqlNative;

/// <summary>
/// Provides database query and command operations for a database connection.
/// </summary>
public abstract partial class SqlNativeContext : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Initializes a new context with its default configuration.
    /// </summary>
    public SqlNativeContext()
    {
    }

    /// <summary>
    /// Gets or sets the database connection used by this context.
    /// </summary>
    public DbConnection Connection { get; set; }

    /// <summary>
    /// Gets or sets the logger used to write executed SQL commands.
    /// When <see langword="null"/>, the commands are written to the console instead.
    /// </summary>
    public ILogger? Logger { get; set; }

    private SqlNativeContextConfig config = new()
    {
        IsDevelopMode = true,
        LogSqlCommands = true,
        MaxPacketSize = 1_000_000,
        MaxParametersPerStatement = 60_000,
    };

    /// <summary>
    /// Gets or sets the configuration for this context.
    /// </summary>
    public SqlNativeContextConfig Config
    {
        get
        {
            return config;
        }
        set
        {
            if (value == null) return;
            config = value;
        }
    }
}
