namespace SqlNative;

public abstract partial class SqlNativeContext
{
    private static bool IsAlphabetic(string value)
    {
        foreach (var ch in value)
        {
            if (ch < 'a' || ch > 'z')
            {
                return false;
            }
        }

        return true;
    }

    private string GetAlias(HashSet<string> used, string defaultName)
    {
        var name = defaultName.ToLower();
        var length = name.Length;

        for (var start = 0; start < length; start++)
        {
            if (name[start] < 'a' || name[start] > 'z')
            {
                continue;
            }
            var singleLetter = name.Substring(start, 1);
            if (used.Add(singleLetter))
            {
                return singleLetter;
            }
        }

        for (var candidateLength = 2; candidateLength <= 3; candidateLength++)
        {
            if (candidateLength > length)
            {
                break;
            }
            for (var start = 0; start + candidateLength <= length; start++)
            {
                var candidateAlias = name.Substring(start, candidateLength);
                if (!IsAlphabetic(candidateAlias))
                {
                    continue;
                }
                if (used.Add(candidateAlias))
                {
                    return candidateAlias;
                }
            }
        }

        var baseLetter = '\0';
        foreach (var ch in name)
        {
            if (ch >= 'a' && ch <= 'z')
            {
                baseLetter = ch;
                break;
            }
        }

        if (baseLetter != '\0')
        {
            var idx = 1;
            var fallback = $"{baseLetter}{idx}";
            while (!used.Add(fallback))
            {
                idx++;
                fallback = $"{baseLetter}{idx}";
            }
            return fallback;
        }

        var suffix = 1;
        var lastResort = $"{name}{suffix}";
        while (!used.Add(lastResort))
        {
            suffix++;
            lastResort = $"{name}{suffix}";
        }
        return lastResort;
    }

    /// <summary>
    /// Creates a query using the specified query-building function.
    /// </summary>
    /// <param name="func">A function that creates and configures the query.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery(Func<Query, Query> func)
    {
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery = func(selectQuery);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 1 table structure and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A>(Func<Query, A, Query> func)
        where A : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 2 table structures and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <typeparam name="B">The 2th table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A, B>(Func<Query, A, B, Query> func)
        where A : TableStructure, new()
        where B : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var b = new B();
        b.Alias = GetAlias(used, b.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a, b);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 3 table structures and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <typeparam name="B">The 2th table structure type.</typeparam>
    /// <typeparam name="C">The 3th table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A, B, C>(Func<Query, A, B, C, Query> func)
        where A : TableStructure, new()
        where B : TableStructure, new()
        where C : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var b = new B();
        b.Alias = GetAlias(used, b.TableName);
        var c = new C();
        c.Alias = GetAlias(used, c.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a, b, c);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 4 table structures and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <typeparam name="B">The 2th table structure type.</typeparam>
    /// <typeparam name="C">The 3th table structure type.</typeparam>
    /// <typeparam name="D">The 4th table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A, B, C, D>(Func<Query, A, B, C, D, Query> func)
        where A : TableStructure, new()
        where B : TableStructure, new()
        where C : TableStructure, new()
        where D : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var b = new B();
        b.Alias = GetAlias(used, b.TableName);
        var c = new C();
        c.Alias = GetAlias(used, c.TableName);
        var d = new D();
        d.Alias = GetAlias(used, d.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a, b, c, d);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 5 table structures and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <typeparam name="B">The 2th table structure type.</typeparam>
    /// <typeparam name="C">The 3th table structure type.</typeparam>
    /// <typeparam name="D">The 4th table structure type.</typeparam>
    /// <typeparam name="E">The 5th table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A, B, C, D, E>(Func<Query, A, B, C, D, E, Query> func)
        where A : TableStructure, new()
        where B : TableStructure, new()
        where C : TableStructure, new()
        where D : TableStructure, new()
        where E : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var b = new B();
        b.Alias = GetAlias(used, b.TableName);
        var c = new C();
        c.Alias = GetAlias(used, c.TableName);
        var d = new D();
        d.Alias = GetAlias(used, d.TableName);
        var e = new E();
        e.Alias = GetAlias(used, e.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a, b, c, d, e);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 6 table structures and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <typeparam name="B">The 2th table structure type.</typeparam>
    /// <typeparam name="C">The 3th table structure type.</typeparam>
    /// <typeparam name="D">The 4th table structure type.</typeparam>
    /// <typeparam name="E">The 5th table structure type.</typeparam>
    /// <typeparam name="F">The 6th table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A, B, C, D, E, F>(Func<Query, A, B, C, D, E, F, Query> func)
        where A : TableStructure, new()
        where B : TableStructure, new()
        where C : TableStructure, new()
        where D : TableStructure, new()
        where E : TableStructure, new()
        where F : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var b = new B();
        b.Alias = GetAlias(used, b.TableName);
        var c = new C();
        c.Alias = GetAlias(used, c.TableName);
        var d = new D();
        d.Alias = GetAlias(used, d.TableName);
        var e = new E();
        e.Alias = GetAlias(used, e.TableName);
        var f = new F();
        f.Alias = GetAlias(used, f.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a, b, c, d, e, f);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 7 table structures and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <typeparam name="B">The 2th table structure type.</typeparam>
    /// <typeparam name="C">The 3th table structure type.</typeparam>
    /// <typeparam name="D">The 4th table structure type.</typeparam>
    /// <typeparam name="E">The 5th table structure type.</typeparam>
    /// <typeparam name="F">The 6th table structure type.</typeparam>
    /// <typeparam name="G">The 7th table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A, B, C, D, E, F, G>(Func<Query, A, B, C, D, E, F, G, Query> func)
        where A : TableStructure, new()
        where B : TableStructure, new()
        where C : TableStructure, new()
        where D : TableStructure, new()
        where E : TableStructure, new()
        where F : TableStructure, new()
        where G : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var b = new B();
        b.Alias = GetAlias(used, b.TableName);
        var c = new C();
        c.Alias = GetAlias(used, c.TableName);
        var d = new D();
        d.Alias = GetAlias(used, d.TableName);
        var e = new E();
        e.Alias = GetAlias(used, e.TableName);
        var f = new F();
        f.Alias = GetAlias(used, f.TableName);
        var g = new G();
        g.Alias = GetAlias(used, g.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a, b, c, d, e, f, g);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 8 table structures and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <typeparam name="B">The 2th table structure type.</typeparam>
    /// <typeparam name="C">The 3th table structure type.</typeparam>
    /// <typeparam name="D">The 4th table structure type.</typeparam>
    /// <typeparam name="E">The 5th table structure type.</typeparam>
    /// <typeparam name="F">The 6th table structure type.</typeparam>
    /// <typeparam name="G">The 7th table structure type.</typeparam>
    /// <typeparam name="H">The 8th table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A, B, C, D, E, F, G, H>(Func<Query, A, B, C, D, E, F, G, H, Query> func)
        where A : TableStructure, new()
        where B : TableStructure, new()
        where C : TableStructure, new()
        where D : TableStructure, new()
        where E : TableStructure, new()
        where F : TableStructure, new()
        where G : TableStructure, new()
        where H : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var b = new B();
        b.Alias = GetAlias(used, b.TableName);
        var c = new C();
        c.Alias = GetAlias(used, c.TableName);
        var d = new D();
        d.Alias = GetAlias(used, d.TableName);
        var e = new E();
        e.Alias = GetAlias(used, e.TableName);
        var f = new F();
        f.Alias = GetAlias(used, f.TableName);
        var g = new G();
        g.Alias = GetAlias(used, g.TableName);
        var h = new H();
        h.Alias = GetAlias(used, h.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a, b, c, d, e, f, g, h);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 9 table structures and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <typeparam name="B">The 2th table structure type.</typeparam>
    /// <typeparam name="C">The 3th table structure type.</typeparam>
    /// <typeparam name="D">The 4th table structure type.</typeparam>
    /// <typeparam name="E">The 5th table structure type.</typeparam>
    /// <typeparam name="F">The 6th table structure type.</typeparam>
    /// <typeparam name="G">The 7th table structure type.</typeparam>
    /// <typeparam name="H">The 8th table structure type.</typeparam>
    /// <typeparam name="I">The 9th table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A, B, C, D, E, F, G, H, I>(Func<Query, A, B, C, D, E, F, G, H, I, Query> func)
        where A : TableStructure, new()
        where B : TableStructure, new()
        where C : TableStructure, new()
        where D : TableStructure, new()
        where E : TableStructure, new()
        where F : TableStructure, new()
        where G : TableStructure, new()
        where H : TableStructure, new()
        where I : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var b = new B();
        b.Alias = GetAlias(used, b.TableName);
        var c = new C();
        c.Alias = GetAlias(used, c.TableName);
        var d = new D();
        d.Alias = GetAlias(used, d.TableName);
        var e = new E();
        e.Alias = GetAlias(used, e.TableName);
        var f = new F();
        f.Alias = GetAlias(used, f.TableName);
        var g = new G();
        g.Alias = GetAlias(used, g.TableName);
        var h = new H();
        h.Alias = GetAlias(used, h.TableName);
        var i = new I();
        i.Alias = GetAlias(used, i.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a, b, c, d, e, f, g, h, i);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 10 table structures and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <typeparam name="B">The 2th table structure type.</typeparam>
    /// <typeparam name="C">The 3th table structure type.</typeparam>
    /// <typeparam name="D">The 4th table structure type.</typeparam>
    /// <typeparam name="E">The 5th table structure type.</typeparam>
    /// <typeparam name="F">The 6th table structure type.</typeparam>
    /// <typeparam name="G">The 7th table structure type.</typeparam>
    /// <typeparam name="H">The 8th table structure type.</typeparam>
    /// <typeparam name="I">The 9th table structure type.</typeparam>
    /// <typeparam name="J">The 10th table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A, B, C, D, E, F, G, H, I, J>(Func<Query, A, B, C, D, E, F, G, H, I, J, Query> func)
        where A : TableStructure, new()
        where B : TableStructure, new()
        where C : TableStructure, new()
        where D : TableStructure, new()
        where E : TableStructure, new()
        where F : TableStructure, new()
        where G : TableStructure, new()
        where H : TableStructure, new()
        where I : TableStructure, new()
        where J : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var b = new B();
        b.Alias = GetAlias(used, b.TableName);
        var c = new C();
        c.Alias = GetAlias(used, c.TableName);
        var d = new D();
        d.Alias = GetAlias(used, d.TableName);
        var e = new E();
        e.Alias = GetAlias(used, e.TableName);
        var f = new F();
        f.Alias = GetAlias(used, f.TableName);
        var g = new G();
        g.Alias = GetAlias(used, g.TableName);
        var h = new H();
        h.Alias = GetAlias(used, h.TableName);
        var i = new I();
        i.Alias = GetAlias(used, i.TableName);
        var j = new J();
        j.Alias = GetAlias(used, j.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a, b, c, d, e, f, g, h, i, j);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 11 table structures and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <typeparam name="B">The 2th table structure type.</typeparam>
    /// <typeparam name="C">The 3th table structure type.</typeparam>
    /// <typeparam name="D">The 4th table structure type.</typeparam>
    /// <typeparam name="E">The 5th table structure type.</typeparam>
    /// <typeparam name="F">The 6th table structure type.</typeparam>
    /// <typeparam name="G">The 7th table structure type.</typeparam>
    /// <typeparam name="H">The 8th table structure type.</typeparam>
    /// <typeparam name="I">The 9th table structure type.</typeparam>
    /// <typeparam name="J">The 10th table structure type.</typeparam>
    /// <typeparam name="K">The 11th table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A, B, C, D, E, F, G, H, I, J, K>(Func<Query, A, B, C, D, E, F, G, H, I, J, K, Query> func)
        where A : TableStructure, new()
        where B : TableStructure, new()
        where C : TableStructure, new()
        where D : TableStructure, new()
        where E : TableStructure, new()
        where F : TableStructure, new()
        where G : TableStructure, new()
        where H : TableStructure, new()
        where I : TableStructure, new()
        where J : TableStructure, new()
        where K : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var b = new B();
        b.Alias = GetAlias(used, b.TableName);
        var c = new C();
        c.Alias = GetAlias(used, c.TableName);
        var d = new D();
        d.Alias = GetAlias(used, d.TableName);
        var e = new E();
        e.Alias = GetAlias(used, e.TableName);
        var f = new F();
        f.Alias = GetAlias(used, f.TableName);
        var g = new G();
        g.Alias = GetAlias(used, g.TableName);
        var h = new H();
        h.Alias = GetAlias(used, h.TableName);
        var i = new I();
        i.Alias = GetAlias(used, i.TableName);
        var j = new J();
        j.Alias = GetAlias(used, j.TableName);
        var k = new K();
        k.Alias = GetAlias(used, k.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a, b, c, d, e, f, g, h, i, j, k);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 12 table structures and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <typeparam name="B">The 2th table structure type.</typeparam>
    /// <typeparam name="C">The 3th table structure type.</typeparam>
    /// <typeparam name="D">The 4th table structure type.</typeparam>
    /// <typeparam name="E">The 5th table structure type.</typeparam>
    /// <typeparam name="F">The 6th table structure type.</typeparam>
    /// <typeparam name="G">The 7th table structure type.</typeparam>
    /// <typeparam name="H">The 8th table structure type.</typeparam>
    /// <typeparam name="I">The 9th table structure type.</typeparam>
    /// <typeparam name="J">The 10th table structure type.</typeparam>
    /// <typeparam name="K">The 11th table structure type.</typeparam>
    /// <typeparam name="L">The 12th table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A, B, C, D, E, F, G, H, I, J, K, L>(Func<Query, A, B, C, D, E, F, G, H, I, J, K, L, Query> func)
        where A : TableStructure, new()
        where B : TableStructure, new()
        where C : TableStructure, new()
        where D : TableStructure, new()
        where E : TableStructure, new()
        where F : TableStructure, new()
        where G : TableStructure, new()
        where H : TableStructure, new()
        where I : TableStructure, new()
        where J : TableStructure, new()
        where K : TableStructure, new()
        where L : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var b = new B();
        b.Alias = GetAlias(used, b.TableName);
        var c = new C();
        c.Alias = GetAlias(used, c.TableName);
        var d = new D();
        d.Alias = GetAlias(used, d.TableName);
        var e = new E();
        e.Alias = GetAlias(used, e.TableName);
        var f = new F();
        f.Alias = GetAlias(used, f.TableName);
        var g = new G();
        g.Alias = GetAlias(used, g.TableName);
        var h = new H();
        h.Alias = GetAlias(used, h.TableName);
        var i = new I();
        i.Alias = GetAlias(used, i.TableName);
        var j = new J();
        j.Alias = GetAlias(used, j.TableName);
        var k = new K();
        k.Alias = GetAlias(used, k.TableName);
        var l = new L();
        l.Alias = GetAlias(used, l.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a, b, c, d, e, f, g, h, i, j, k, l);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 13 table structures and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <typeparam name="B">The 2th table structure type.</typeparam>
    /// <typeparam name="C">The 3th table structure type.</typeparam>
    /// <typeparam name="D">The 4th table structure type.</typeparam>
    /// <typeparam name="E">The 5th table structure type.</typeparam>
    /// <typeparam name="F">The 6th table structure type.</typeparam>
    /// <typeparam name="G">The 7th table structure type.</typeparam>
    /// <typeparam name="H">The 8th table structure type.</typeparam>
    /// <typeparam name="I">The 9th table structure type.</typeparam>
    /// <typeparam name="J">The 10th table structure type.</typeparam>
    /// <typeparam name="K">The 11th table structure type.</typeparam>
    /// <typeparam name="L">The 12th table structure type.</typeparam>
    /// <typeparam name="M">The 13th table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A, B, C, D, E, F, G, H, I, J, K, L, M>(Func<Query, A, B, C, D, E, F, G, H, I, J, K, L, M, Query> func)
        where A : TableStructure, new()
        where B : TableStructure, new()
        where C : TableStructure, new()
        where D : TableStructure, new()
        where E : TableStructure, new()
        where F : TableStructure, new()
        where G : TableStructure, new()
        where H : TableStructure, new()
        where I : TableStructure, new()
        where J : TableStructure, new()
        where K : TableStructure, new()
        where L : TableStructure, new()
        where M : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var b = new B();
        b.Alias = GetAlias(used, b.TableName);
        var c = new C();
        c.Alias = GetAlias(used, c.TableName);
        var d = new D();
        d.Alias = GetAlias(used, d.TableName);
        var e = new E();
        e.Alias = GetAlias(used, e.TableName);
        var f = new F();
        f.Alias = GetAlias(used, f.TableName);
        var g = new G();
        g.Alias = GetAlias(used, g.TableName);
        var h = new H();
        h.Alias = GetAlias(used, h.TableName);
        var i = new I();
        i.Alias = GetAlias(used, i.TableName);
        var j = new J();
        j.Alias = GetAlias(used, j.TableName);
        var k = new K();
        k.Alias = GetAlias(used, k.TableName);
        var l = new L();
        l.Alias = GetAlias(used, l.TableName);
        var m = new M();
        m.Alias = GetAlias(used, m.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a, b, c, d, e, f, g, h, i, j, k, l, m);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 14 table structures and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <typeparam name="B">The 2th table structure type.</typeparam>
    /// <typeparam name="C">The 3th table structure type.</typeparam>
    /// <typeparam name="D">The 4th table structure type.</typeparam>
    /// <typeparam name="E">The 5th table structure type.</typeparam>
    /// <typeparam name="F">The 6th table structure type.</typeparam>
    /// <typeparam name="G">The 7th table structure type.</typeparam>
    /// <typeparam name="H">The 8th table structure type.</typeparam>
    /// <typeparam name="I">The 9th table structure type.</typeparam>
    /// <typeparam name="J">The 10th table structure type.</typeparam>
    /// <typeparam name="K">The 11th table structure type.</typeparam>
    /// <typeparam name="L">The 12th table structure type.</typeparam>
    /// <typeparam name="M">The 13th table structure type.</typeparam>
    /// <typeparam name="N">The 14th table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A, B, C, D, E, F, G, H, I, J, K, L, M, N>(Func<Query, A, B, C, D, E, F, G, H, I, J, K, L, M, N, Query> func)
        where A : TableStructure, new()
        where B : TableStructure, new()
        where C : TableStructure, new()
        where D : TableStructure, new()
        where E : TableStructure, new()
        where F : TableStructure, new()
        where G : TableStructure, new()
        where H : TableStructure, new()
        where I : TableStructure, new()
        where J : TableStructure, new()
        where K : TableStructure, new()
        where L : TableStructure, new()
        where M : TableStructure, new()
        where N : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var b = new B();
        b.Alias = GetAlias(used, b.TableName);
        var c = new C();
        c.Alias = GetAlias(used, c.TableName);
        var d = new D();
        d.Alias = GetAlias(used, d.TableName);
        var e = new E();
        e.Alias = GetAlias(used, e.TableName);
        var f = new F();
        f.Alias = GetAlias(used, f.TableName);
        var g = new G();
        g.Alias = GetAlias(used, g.TableName);
        var h = new H();
        h.Alias = GetAlias(used, h.TableName);
        var i = new I();
        i.Alias = GetAlias(used, i.TableName);
        var j = new J();
        j.Alias = GetAlias(used, j.TableName);
        var k = new K();
        k.Alias = GetAlias(used, k.TableName);
        var l = new L();
        l.Alias = GetAlias(used, l.TableName);
        var m = new M();
        m.Alias = GetAlias(used, m.TableName);
        var n = new N();
        n.Alias = GetAlias(used, n.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a, b, c, d, e, f, g, h, i, j, k, l, m, n);

        return selectQuery;
    }

    /// <summary>
    /// Creates a query builder for 15 table structures and assigns unique aliases to them.
    /// </summary>
    /// <typeparam name="A">The first table structure type.</typeparam>
    /// <typeparam name="B">The 2th table structure type.</typeparam>
    /// <typeparam name="C">The 3th table structure type.</typeparam>
    /// <typeparam name="D">The 4th table structure type.</typeparam>
    /// <typeparam name="E">The 5th table structure type.</typeparam>
    /// <typeparam name="F">The 6th table structure type.</typeparam>
    /// <typeparam name="G">The 7th table structure type.</typeparam>
    /// <typeparam name="H">The 8th table structure type.</typeparam>
    /// <typeparam name="I">The 9th table structure type.</typeparam>
    /// <typeparam name="J">The 10th table structure type.</typeparam>
    /// <typeparam name="K">The 11th table structure type.</typeparam>
    /// <typeparam name="L">The 12th table structure type.</typeparam>
    /// <typeparam name="M">The 13th table structure type.</typeparam>
    /// <typeparam name="N">The 14th table structure type.</typeparam>
    /// <typeparam name="O">The 15th table structure type.</typeparam>
    /// <param name="func">A function that creates the query from the table structures.</param>
    /// <returns>The query created by <paramref name="func"/>.</returns>
    public Query BuildQuery<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(Func<Query, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, Query> func)
        where A : TableStructure, new()
        where B : TableStructure, new()
        where C : TableStructure, new()
        where D : TableStructure, new()
        where E : TableStructure, new()
        where F : TableStructure, new()
        where G : TableStructure, new()
        where H : TableStructure, new()
        where I : TableStructure, new()
        where J : TableStructure, new()
        where K : TableStructure, new()
        where L : TableStructure, new()
        where M : TableStructure, new()
        where N : TableStructure, new()
        where O : TableStructure, new()
    {
        var used = new HashSet<string>();
        var a = new A();
        a.Alias = GetAlias(used, a.TableName);
        var b = new B();
        b.Alias = GetAlias(used, b.TableName);
        var c = new C();
        c.Alias = GetAlias(used, c.TableName);
        var d = new D();
        d.Alias = GetAlias(used, d.TableName);
        var e = new E();
        e.Alias = GetAlias(used, e.TableName);
        var f = new F();
        f.Alias = GetAlias(used, f.TableName);
        var g = new G();
        g.Alias = GetAlias(used, g.TableName);
        var h = new H();
        h.Alias = GetAlias(used, h.TableName);
        var i = new I();
        i.Alias = GetAlias(used, i.TableName);
        var j = new J();
        j.Alias = GetAlias(used, j.TableName);
        var k = new K();
        k.Alias = GetAlias(used, k.TableName);
        var l = new L();
        l.Alias = GetAlias(used, l.TableName);
        var m = new M();
        m.Alias = GetAlias(used, m.TableName);
        var n = new N();
        n.Alias = GetAlias(used, n.TableName);
        var o = new O();
        o.Alias = GetAlias(used, o.TableName);
        var selectQuery = new Query
        {
            Connection = Connection,
            GenerateModel = Config.IsDevelopMode,
            LogSqlCommand = Config.IsDevelopMode || Config.LogSqlCommands,
        };
        selectQuery.Connection = Connection;
        selectQuery = func(selectQuery, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o);

        return selectQuery;
    }

#if NET10_0

#elif NET8_0
    // .NET 8 implementation
#else
    // fallback for netstandard2.0 / net461
#endif
}