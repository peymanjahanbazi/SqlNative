namespace SqlNative.ModelGenerator.Generation;

internal interface IContextWriter
{
    string Generate(
        IReadOnlyList<(string ClassName, string EntityClassName)> tables,
        string contextName,
        string packageNamespace);
}