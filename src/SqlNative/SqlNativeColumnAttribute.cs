using System;

namespace SqlNative;

/// <summary>
/// Associates a property with the database column name used to map query results.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SqlNativeColumnAttribute(string name) : Attribute
{
    /// <summary>Gets the database column name.</summary>
    public string Name { get; } = name;
}
