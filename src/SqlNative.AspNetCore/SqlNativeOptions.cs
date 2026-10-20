namespace SqlNative.AspNetCore;

public class SqlNativeOptions
{
    public bool IsDevelopMode { get; set; } = false;

    public int MaxParametersPerStatement { get; set; } = 60_000;
    public int MaxPacketSize { get; set; } = 1_000_000;

    public string ConnectionString { get; set; } = string.Empty;

    //public ConnectionCreatePolicy ConnectionCreatePolicy { get; set; } = ConnectionCreatePolicy.ConnectionPool;
    //public int ConnectionPoolSize { get; set; } = 10;
}