namespace SqlNative.AspNetCore;

public class SqlNativeOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public ConnectionCreatePolicy ConnectionCreatePolicy { get; set; } = ConnectionCreatePolicy.ConnectionPool;
    public int ConnectionPoolSize { get; set; } = 10;
}
