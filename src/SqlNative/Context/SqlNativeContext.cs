using System.Data.Common;

namespace SqlNative;

public abstract partial class SqlNativeContext : IDisposable, IAsyncDisposable
{
    public SqlNativeContext()
    {
    }

    public DbConnection Connection { get; set; }

    private SqlNativeContextConfig config = new()
    {
        IsDevelopMode = true,
        LogSqlCommands = true,
        MaxPacketSize = 1_000_000,
        MaxParametersPerStatement = 60_000,
    };

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