using SqlNative.ModelGenerator.Engines;

namespace SqlNative.ModelGenerator.Cli;

internal static class HelpText
{
    public const string Content = """
        SqlNative.ModelGenerator
        Generates strongly typed table model classes and a context class from an existing database.

        USAGE:
          SqlNative.ModelGenerator --cs <connectionString> --engine <mysql|mssql|postgresql|oracle> --folder <path> --package <namespace> [--context <contextName>]

        OPTIONS:
          --cs          Database connection string.                       (aliases: --connectionstring, --conn)
          --engine      Target database engine: mysql | mssql | postgresql | oracle. (aliases: --database, --db, --pg, --postgres, --ora, --oradb)
          --folder      Output folder where the .cs files are written.    (aliases: --out, --output)
          --package     Root namespace of the generated classes.          (aliases: --namespace, --ns)
          --context     Optional name of the generated context class.
                        Defaults to <DatabaseName>Context.                (aliases: --ctx)
          --help        Prints this help and exits.                       (aliases: --h, --?)

        SAMPLES:
          1. Print the usage:
             SqlNative.ModelGenerator --help

          2. Generate models for a MySQL database (context defaults to <DatabaseName>Context):
             SqlNative.ModelGenerator --cs "Server=localhost;Port=3306;Database=shop;User ID=root;Password=p@ssw0rd;" --engine mysql --folder "./models" --package "Sample.App.Models"

          3. Generate models for MySQL with an explicit context name:
             SqlNative.ModelGenerator --cs "Server=localhost;Port=3306;Database=shop;User ID=root;Password=p@ssw0rd;" --engine mysql --folder "./models" --package "Sample.App.Models" --context Db1

          4. Same as above using option=value syntax:
             SqlNative.ModelGenerator --cs="Server=localhost;Database=shop;User ID=root;Password=p@ssw0rd;" --engine=mysql --folder=./models --package="Sample.App.Models" --context=Db1

          5. Generate models for Microsoft SQL Server:
             SqlNative.ModelGenerator --cs "Server=localhost;Database=shop;User Id=sa;Password=p@ssw0rd;TrustServerCertificate=True" --engine mssql --folder "./models" --package "Sample.App.Models"

          6. Generate models for PostgreSQL:
             SqlNative.ModelGenerator --cs "Host=localhost;Database=shop;Username=postgres;Password=p@ssw0rd" --engine postgresql --folder "./models" --package "Sample.App.Models"

          7. Generate models for Oracle Database:
             SqlNative.ModelGenerator --cs "User Id=scott;Password=tiger;Data Source=oracle-host:1521/ORCLPDB1" --engine oracle --folder "./models" --package "Sample.App.Models"

        NOTES:
          - One <TableName>.cs file is generated per database table, plus a <ContextName>.cs file.
          - Currently implemented engines: mysql, postgresql, mssql, oracle.
        """;
}