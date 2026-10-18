namespace SqlNative.ModelGenerator.Generation;

internal interface IModelIdentifierHelper
{
    string GetObjectNameInCs(string originalname);
    string EnsureUnique(string candidate, ISet<string> usedNames);
    string EscapeCsStringLiteral(string value);
    string EscapeCsStringLiteralNoQuote(string value);
    bool NeedsBackticks(string identifier);
    string WrapObjectName(string objectName);
}