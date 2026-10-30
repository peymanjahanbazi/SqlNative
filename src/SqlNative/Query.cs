using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace SqlNative;

/// <summary>
/// Builds and executes SQL commands using provider-neutral SQL text.
/// </summary>
public class Query
{
    /// <summary>
    /// Gets the SQL text accumulated by this query.
    /// </summary>
    public string Sql
    {
        get
        {
            return sqlBuilder.ToString();
        }
    }

    private Dictionary<string, object?> Parameters = new();

    /// <summary>
    /// Gets or sets the database connection used to execute this query.
    /// </summary>
    public DbConnection Connection { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether result models are generated for this query.
    /// </summary>
    public bool GenerateModel { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether generated SQL commands are written to the logger,
    /// or to the console when no logger is set.
    /// </summary>
    public bool LogSqlCommand { get; internal set; }

    /// <summary>
    /// Gets the logger used to write generated SQL commands.
    /// When <see langword="null"/>, the commands are written to the console instead.
    /// </summary>
    public ILogger? Logger { get; internal set; }

    private StringBuilder sqlBuilder = new();

    /// <summary>Appends SQL text to the query.</summary>
    /// <param name="v">The SQL text to append.</param>
    /// <returns>This query instance.</returns>
    public Query Append(string v)
    {
        if (string.IsNullOrWhiteSpace(v))
        {
            return this;
        }
        sqlBuilder.AppendLine(v);
        return this;
    }

    /// <summary>Appends SQL text when the specified condition is true.</summary>
    /// <param name="condition">Whether the text should be appended.</param>
    /// <param name="v">The SQL text to append.</param>
    /// <returns>This query instance.</returns>
    public Query AppendIf(bool condition, string v)
    {
        if (!condition || string.IsNullOrWhiteSpace(v))
        {
            return this;
        }
        sqlBuilder.AppendLine(v);
        return this;
    }

    /// <summary>Replaces the current query parameters with the specified dictionary.</summary>
    /// <param name="parameters">The parameters to use.</param>
    /// <returns>This query instance.</returns>
    public Query SetParameters(Dictionary<string, object?> parameters)
    {
        this.Parameters.Clear();
        return AddParameters(parameters);
    }

    /// <summary>Replaces the current query parameters with values from an object.</summary>
    /// <param name="parameters">The object whose public properties supply parameter values.</param>
    /// <returns>This query instance.</returns>
    public Query SetParameters(object parameters)
    {
        this.Parameters.Clear();
        return AddParameters(parameters);
    }

    /// <summary>Adds the specified dictionary to the current query parameters.</summary>
    /// <param name="parameters">The parameters to add.</param>
    /// <returns>This query instance.</returns>
    public Query AddParameters(Dictionary<string, object?> parameters)
    {
        if (parameters == null) return this;
        foreach (var kvp in parameters)
        {
            this.Parameters[kvp.Key] = kvp.Value;
        }
        return this;
    }

    /// <summary>Adds values from an object's public properties to the current query parameters.</summary>
    /// <param name="parameters">The object whose public properties supply parameter values.</param>
    /// <returns>This query instance.</returns>
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

    /// <summary>Executes the query and returns the first value of the first row.</summary>
    /// <returns>The scalar result, or <see langword="null"/> when no value is returned.</returns>
    public object ExecuteScalar()
    {
        return ExecuteScalarAsync().GetAwaiter().GetResult();
    }

    /// <summary>Executes the query asynchronously and returns the first value of the first row.</summary>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The scalar result, or <see langword="null"/> when no value is returned.</returns>
    public async Task<object> ExecuteScalarAsync(CancellationToken ct = default)
    {
        using var command = CreateCommand();
        var result = await command.ExecuteScalarAsync(ct).ConfigureAwait(false);
        return result;
    }

    /// <summary>Executes the query as a non-query command.</summary>
    /// <returns>The number of rows affected.</returns>
    public int ExecuteNonQuery()
    {
        return ExecuteNonQueryAsync().GetAwaiter().GetResult();
    }

    /// <summary>Executes the query asynchronously as a non-query command.</summary>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The number of rows affected.</returns>
    public async Task<int> ExecuteNonQueryAsync(CancellationToken ct = default)
    {
        using var command = CreateCommand();
        var result = await command.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        return result;
    }

    /// <summary>Executes the query and maps the returned rows to result objects.</summary>
    /// <typeparam name="Result">The result type to create for each row.</typeparam>
    /// <returns>The mapped result rows.</returns>
    public List<Result> Execute<Result>()
        where Result : new()
    {
        return ExecuteAsync<Result>().GetAwaiter().GetResult();
    }

    /// <summary>Executes the query asynchronously and maps the returned rows to result objects.</summary>
    /// <typeparam name="Result">The result type to create for each row.</typeparam>
    /// <param name="ct">The token used to cancel the command.</param>
    /// <returns>The mapped result rows.</returns>
    public async Task<List<Result>> ExecuteAsync<Result>(CancellationToken ct = default)
        where Result : new()
    {
        using var command = CreateCommand();

        var rows = await QueryMapper.MapObjectAsync<Result>(command, ct).ConfigureAwait(false);
        return rows;
    }

    /// <summary>
    /// Executes the query and optionally generates a model class describing its result columns.
    /// </summary>
    /// <typeparam name="T">The model type for which column metadata is generated.</typeparam>
    /// <param name="className">The generated class name. When empty, a name is derived from the caller.</param>
    /// <param name="ns">The generated class namespace. When empty, the namespace of <typeparamref name="T"/> is used.</param>
    /// <param name="file">The source file path of the caller.</param>
    /// <param name="member">The calling member name.</param>
    /// <returns>This query instance.</returns>
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
            /*if (param.Value == null)
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
            }*/
            var dbParam = command.CreateParameter();
            dbParam.ParameterName = name;
            dbParam.Value = param.Value ?? DBNull.Value;
            command.Parameters.Add(dbParam);
        }
        command.CommandText = sql.ToString();
        if (LogSqlCommand)
        {
            if (Logger is not null)
                Logger.LogInformation("SQL command: {Sql}", command.CommandText);
            else
                Console.WriteLine(command.CommandText);
        }

        return command;
    }

    /// <summary>Appends text generated for every item in a list.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="items">The items to process.</param>
    /// <param name="func">The function that generates text for each item.</param>
    /// <returns>This query instance.</returns>
    public Query AppendFor<T>(List<T> items, Func<T, string> func)
    {
        return this.AppendFor("", items, func);
    }

    /// <summary>Appends text generated for every item in a list, separated by the specified join text.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="join">The separator inserted between generated items.</param>
    /// <param name="items">The items to process.</param>
    /// <param name="func">The function that generates text for each item.</param>
    /// <returns>This query instance.</returns>
    public Query AppendFor<T>(string join, List<T> items, Func<T, string> func)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (i > 0) Append(join);
            Append(func(items[i]));
        }
        return this;
    }

    /// <summary>Invokes a configuration action and returns this query instance.</summary>
    /// <param name="value">The configuration action to invoke.</param>
    /// <returns>This query instance.</returns>
    public Query Config(Action value)
    {
        value();
        return this;
    }
}