using SqlNative.ModelGenerator.Generation;
using System.Text;

namespace SqlNative.ModelGenerator.Generation.MySql;

internal sealed class MySqlContextWriter : IContextWriter
{
    public string Generate(
        IReadOnlyList<(string ClassName, string EntityClassName)> tables,
        string contextName,
        string packageNamespace)
    {
        var builder = new StringBuilder();
        builder.AppendLine("using SqlNative;");
        builder.AppendLine();
        builder.AppendLine($"namespace {packageNamespace};");
        builder.AppendLine();
        builder.AppendLine($"public partial class {contextName} : DefaultContext");
        builder.AppendLine("{");

        foreach (var (className, entityClassName) in tables)
        {
            var propertyName = className == contextName ? className + "Table" : className;
            var fieldName = "_" + propertyName;
            builder.AppendLine($"    private {entityClassName}? {fieldName};");
            builder.AppendLine();
            builder.AppendLine($"    public {entityClassName} {propertyName} => {fieldName} ??= new(this);");
            builder.AppendLine();
        }

        builder.AppendLine("}");
        return builder.ToString();
    }
}