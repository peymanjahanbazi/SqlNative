namespace SqlNative;

public abstract partial class SqlNativeContext
{
    private bool _ownsConnection;
    private bool _disposed;

    /// <summary>
    /// Gets or sets whether this context owns its <see cref="Connection"/>.
    /// When <c>true</c>, the connection is closed and disposed at scope disposal
    /// time. When <c>false</c> (default — e.g., borrowed from an EF
    /// <c>DbContext</c> or transferred to another owner), the connection is left
    /// alone. The DI factory that creates and opens the connection sets this to
    /// <c>true</c>; consumers may set it to <c>false</c> to opt out of disposal.
    /// </summary>
    public bool OwnsConnection
    {
        get => _ownsConnection;
        set => _ownsConnection = value;
    }

    /// <summary>
    /// Releases the resources owned by this context asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous disposal operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the resources owned by this context.
    /// </summary>
    public void Dispose()
    {
        // Mirror EF Core's pattern: sync path forwards to async so cleanup
        // logic stays in one place. DisposeAsync already suppresses finalization.
        DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (_disposed) return;
        _disposed = true;

        var conn = Connection;
        if (conn == null || !_ownsConnection) return;

        try
        {
            if (conn.State != System.Data.ConnectionState.Closed)
            {
#if NET6_0_OR_GREATER
                await conn.CloseAsync().ConfigureAwait(false);
#else
                conn.Close();
#endif
            }
        }
        catch { /* non-fatal */ }

        try
        {
#if NET6_0_OR_GREATER
            await conn.DisposeAsync().ConfigureAwait(false);
#else
            conn.Dispose();
#endif
        }
        catch { /* swallow */ }
    }
}