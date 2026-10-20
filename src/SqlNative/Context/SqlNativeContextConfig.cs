namespace SqlNative;

public class SqlNativeContextConfig
{
    public bool IsDevelopMode { get; set; }
    public int MaxParametersPerStatement { get; set; }
    public int MaxPacketSize { get; set; }
}