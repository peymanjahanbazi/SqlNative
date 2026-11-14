namespace SqlNative;

public partial class MySqlFunctions
{
    public static string AS(string column, string alias)
    {
        return $"{column} AS {alias}";
    }
}

public class My : MySqlFunctions
{
}