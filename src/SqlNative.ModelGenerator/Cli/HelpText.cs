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
          --include-tables   Only generate models for tables matching these patterns.
                        Comma separated ("emp,dept") and/or repeated (--include-tables emp --include-tables dept).
                        '_' matches any sequence of characters ("e_" = tables starting with "e").   (aliases: --include)
          --exclude-tables   Skip tables matching these patterns. Same syntax as --include-tables.
                        Applied after --include-tables.                   (aliases: --exclude)
          --help        Prints this help and exits.                       (aliases: --h, --?)
          -i <file>     Read parameters from <file>. Multi-line content works: newlines act as
                        separators like a space, quotes group values, e.g. --cs "conn str".
                        Command-line options override file values; table filters are combined. (alias: --input-file)

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

          8. Generate models only for the emp and dept tables:
             SqlNative.ModelGenerator --cs "..." --engine mssql --folder "./models" --package "Sample.App.Models" --include-tables "emp,dept"

          9. Generate models for every table except emp and dept:
             SqlNative.ModelGenerator --cs "..." --engine mssql --folder "./models" --package "Sample.App.Models" --exclude-tables emp --exclude-tables dept

         10. Generate models for tables whose name starts with "e" (the '_' wildcard):
             SqlNative.ModelGenerator --cs "..." --engine mysql --folder "./models" --package "Sample.App.Models" --include-tables "e_"

         11. Read the parameters from a file, overriding the engine on the command line:
             SqlNative.ModelGenerator -i params.txt --engine mssql

        NOTES:
          - One <TableName>.cs file is generated per database table, plus a <ContextName>.cs file.
          - Currently implemented engines: mysql, postgresql, mssql, oracle.
        """;
}