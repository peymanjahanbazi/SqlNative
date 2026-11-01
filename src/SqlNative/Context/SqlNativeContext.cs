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

    /// <summary>
    /// Gets or sets whether development features such as model generation are enabled.
    /// </summary>
    public bool IsDevelopMode { get; set; }

    /// <summary>
    /// Gets or sets whether SQL commands are written to the logger,
    /// or to the console when no logger is set.
    /// </summary>
    public bool LogSqlCommands { get; set; }
}