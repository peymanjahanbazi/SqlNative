using System;

namespace SqlNative;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SqlNativeColumnAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}