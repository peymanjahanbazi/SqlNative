using System.Data.Common;

namespace SqlNative;

public abstract partial class DefaultContext
{
    public DefaultContext()
    {
    }

    public DbConnection Connection { get; set; }

    private ContextConfig config = new()
    {
        IsDevelopMode = true,
        MaxPacketSize = 1_000_000,
        MaxParametersPerStatement = 60_000,
    };

    public ContextConfig Config
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