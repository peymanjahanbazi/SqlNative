using SqlNative.ModelGenerator.Schema;

namespace SqlNative.ModelGenerator.Generation;

internal interface IModelWriter
{
    string Generate(
        TableInfo table,
        string className,
        string entityClassName,
        string contextName,
        string packageNamespace);
}