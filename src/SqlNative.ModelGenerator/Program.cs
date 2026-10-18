using SqlNative.ModelGenerator;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        return await CliRunner.RunAsync(args);
    }
}