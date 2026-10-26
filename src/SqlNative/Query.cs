using System.Data.Common;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace SqlNative;

public class Query
{
    public string Sql
    {
        get
        {
            return sqlBuilder.ToString();
        }
    }

    private Dictionary<string, object?> Parameters = new();

    public DbConnection Connection { get; internal set; }

    public bool GenerateModel { get; internal set; }
    public bool LogSqlCommand { get; internal set; }

    private StringBuilder sqlBuilder = new();

    public Query Append(string v)
    {
        if (string.IsNullOrWhiteSpace(v))
        {
            return this;
        }
        sqlBuilder.AppendLine(v);
        return this;
    }

    public Query AppendIf(bool condition, string v)
    {
        if (!condition || string.IsNullOrWhiteSpace(v))
        {
            return this;
        }
        sqlBuilder.AppendLine(v);
        return this;
    }

    public Query SetParameters(Dictionary<string, object?> parameters)
    {
        this.Parameters.Clear();
        return AddParameters(parameters);
    }

    public Query SetParameters(object parameters)
    {
        this.Parameters.Clear();
        return AddParameters(parameters);
    }

    public Query AddParameters(Dictionary<string, object?> parameters)
    {
        if (parameters == null) return this;
        foreach (var kvp in parameters)
        {
            this.Parameters[kvp.Key] = kvp.Value;
        }
        return this;
    }

    public Query AddParameters(object parameters)
    {
        if (parameters == null) return this;

        var properties = parameters.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            Parameters[property.Name] = property.GetValue(parameters);
        }
        return this;
    }

    public object ExecuteScalar()
    {
        using var command = CreateCommand();
        var result = command.ExecuteScalar();
        return result;
    }

    public int ExecuteNonQuery()
    {
        using var command = CreateCommand();
        var result = command.ExecuteNonQuery();
        return result;
    }

    public List<Result> Execute<Result>()
        where Result : new()
    {
        using var command = CreateCommand();

        var rows = QueryMapper.MapObject<Result>(command);
        return rows;
    }

    public Query Model<T>(string className = "",
        string ns = "",
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "")
    {
        if (!GenerateModel) return this;
        string? dir = Path.GetDirectoryName(file);
        string fileName = Path.GetFileNameWithoutExtension(file);
        Type type = typeof(T);
        if (string.IsNullOrEmpty(ns))
            ns = type.Namespace;
        string modelFileName;
        if (className == "")
        {
            className = fileName + member + "Model";
            modelFileName = fileName + "." + member + "Model.cs";
        }
        else
        {
            modelFileName = fileName + "." + className + ".cs";
        }
        using var command = CreateCommand();
        using var reader = command.ExecuteReader();
        QueryModelGenerator.Model(reader, className, ns ?? "", Path.Combine(dir, modelFileName));
        return this;
    }

    private DbCommand CreateCommand()
    {
        DbCommand command = Connection.CreateCommand();
        StringBuilder sql = new(this.Sql);
        foreach (var param in this.Parameters)
        {
            var name = param.Key.StartsWith("@", StringComparison.Ordinal) ? param.Key : "@" + param.Key;
            if (param.Value == null)
            {
                sql = sql.Replace(name, "NULL");
                continue;
            }
            else if (param.Value is byte || param.Value is short || param.Value is int
                || param.Value is long || param.Value is decimal
                || param.Value is double || param.Value is float || param.Value is bool)
            {
                sql = sql.Replace(name, Convert.ToString(param.Value, CultureInfo.InvariantCulture));
                continue;
            }
            var dbParam = command.CreateParameter();
            dbParam.ParameterName = name;
            dbParam.Value = param.Value ?? DBNull.Value;
            command.Parameters.Add(dbParam);
        }
        command.CommandText = sql.ToString();
        if (LogSqlCommand)
            Console.WriteLine(command.CommandText);

        return command;
    }

    public Query AppendFor<T>(List<T> items, Func<T, string> func)
    {
        return this.AppendFor("", items, func);
    }

    public Query AppendFor<T>(string join, List<T> items, Func<T, string> func)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (i > 0) Append(join);
            Append(func(items[i]));
        }
        return this;
    }

    public Query Config(Action value)
    {
        value();
        return this;
    }
}