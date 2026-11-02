using System.Text;

namespace SqlNative.ModelGenerator.Generation.Postgres;

internal sealed class PostgresContextWriter : IContextWriter
{
    public string Generate(
        IReadOnlyList<(string ClassName, string RepositoryClassName)> tables,
        string contextName,
        string packageNamespace)
    {
        var builder = new StringBuilder();
        builder.AppendLine("using SqlNative;");
        builder.AppendLine();
        builder.AppendLine($"namespace {packageNamespace};");
        builder.AppendLine();
        builder.AppendLine($"public partial class {contextName} : SqlNativeContext");
        builder.AppendLine("{");

        foreach (var (className, repositoryClassName) in tables)
        {
            var propertyName = className == contextName ? className + "Table" : className;
            var fieldName = "_" + propertyName;
            builder.AppendLine($"    private {repositoryClassName}? {fieldName};");
            builder.AppendLine();
            builder.AppendLine($"    public {repositoryClassName} {propertyName} => {fieldName} ??= new(this);");
            builder.AppendLine();
        }

        builder.AppendLine("}");
        return builder.ToString();
    }
}