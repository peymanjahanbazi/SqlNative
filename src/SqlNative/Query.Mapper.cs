using System.Data.Common;
using System.Reflection;
using System.Text;

namespace SqlNative;

internal class QueryMapper
{
    internal static List<Result> MapObject<Result>(DbCommand command) where Result : new()
    {
        var rows = new List<Result>();
        using var reader = command.ExecuteReader();
        if (typeof(QueryResult).IsAssignableFrom(typeof(Result)))
        {
            int[] orders = null;
            while (reader.Read())
            {
                Result row = new();
                QueryResult qr = row as QueryResult;
                orders ??= qr.GetOrders(reader);
                qr.SetFields(reader, orders);
                rows.Add(row);
            }
            return rows;
        }
        var resultType = typeof(Result);
        var properties = resultType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var fieldCount = reader.FieldCount;
        var propertyByOrdinal = new PropertyInfo[fieldCount];

        for (var i = 0; i < fieldCount; i++)
        {
            propertyByOrdinal[i] = ResolveProperty(properties, reader.GetName(i), i);
        }

        while (reader.Read())
        {
            //object row = Activator.CreateInstance(resultType);
            Result row = new();
            for (var i = 0; i < fieldCount; i++)
            {
                var property = propertyByOrdinal[i];
                if (property == null || !property.CanWrite || reader.IsDBNull(i))
                {
                    continue;
                }

                var value = reader.GetValue(i);
                var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (!targetType.IsInstanceOfType(value))
                {
                    value = Convert.ChangeType(value, targetType);
                }

                property.SetValue(row, value);
            }

            rows.Add(row);
        }
        return rows;
    }

    private static PropertyInfo ResolveProperty(PropertyInfo[] properties, string columnName, int ordinal)
    {
        PropertyInfo nameMatch = null;
        PropertyInfo canonicalMatch = null;
        var canonicalName = CanonicalizeName(columnName);

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<SqlNativeColumnAttribute>();
            if (attribute != null)
            {
                if (nameMatch == null &&
                    string.Equals(attribute.Name, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    nameMatch = property;
                    continue;
                }
            }

            if (canonicalMatch == null &&
                string.Equals(CanonicalizeName(property.Name), canonicalName, StringComparison.OrdinalIgnoreCase))
            {
                canonicalMatch = property;
            }
        }

        return nameMatch ?? canonicalMatch;
    }

    private static string CanonicalizeName(string name)
    {
        var builder = new StringBuilder(name.Length);
        foreach (var c in name)
        {
            if (char.IsLetterOrDigit(c))
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }
}