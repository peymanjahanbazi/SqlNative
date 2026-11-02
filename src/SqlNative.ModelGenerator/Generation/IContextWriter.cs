namespace SqlNative.ModelGenerator.Generation;

internal interface IContextWriter
{
    string Generate(
        IReadOnlyList<(string ClassName, string RepositoryClassName)> tables,
        string contextName,
        string packageNamespace);
}