using System.Data.Common;

using ACUCustomizationUtils.Configuration.Site;

using Dapper;

using Microsoft.Data.SqlClient;

namespace ACUCustomizationUtils.Helpers.Db;

/// <summary>
/// Microsoft SQL Server database provider. Preserves the original ACU behavior:
/// Windows integrated security by default and an IIS application pool login created after site install.
/// </summary>
/// <remarks>
/// Authored by Sprinterra
/// Copyright Sprinterra(c) 2023
/// </remarks>
public class MsSqlDbProvider : IDbProvider
{
    public DbConnection CreateConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }

    public string? BuildConnectionString(ISiteConfiguration site)
    {
        if (site.SqlServerName == null || site.DbName == null)
            return null;

        return site.DbUser == null
            ? $"Data Source={site.SqlServerName};Initial Catalog={site.DbName};Integrated Security=True;Encrypt=False;"
            : $"Data Source={site.SqlServerName};Initial Catalog={site.DbName};User ID={site.DbUser};Password={site.DbPassword};Encrypt=False;";
    }

    public string CustObjectByProjectNameSql =>
        @"SELECT * FROM CustObject
                                 WHERE ProjectID IN
                                      (SELECT TOP 1 ProjID
                                       FROM CustProject
                                       WHERE Name = @ProjectName)
                                 ORDER by Type";

    public async Task EnsureAppLoginAsync(
        Func<DbConnection> connectionFactory,
        ISiteConfiguration site
    )
    {
        const string serverLogin = @"IIS APPPOOL\DefaultAppPool";
        const string databaseUser = @"DefaultAppPool";

        const string sql =
            @"IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = @ServerLogin)
                              BEGIN
                                EXEC('
                                    CREATE LOGIN ['+@ServerLogin+']
                                    FROM WINDOWS WITH DEFAULT_DATABASE=[master],
                                    DEFAULT_LANGUAGE=[us_english]
                                ')
                              END";

        const string sql1 =
            @"IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @DatabaseUser)
                              BEGIN
                                EXEC('
                                    CREATE USER '+@DatabaseUser+'
                                    FOR LOGIN ['+@ServerLogin+']
                                ')
                              END";

        const string sql2 =
            @"IF EXISTS (SELECT name FROM sys.database_principals WHERE name = @DatabaseUser)
                              BEGIN
                                EXEC sp_addrolemember 'db_owner', @DatabaseUser
                              END";

        await using DbConnection connection = connectionFactory();
        await connection.OpenAsync();
        DbTransaction tr = await connection.BeginTransactionAsync();
        try
        {
            //Create login
            object[] parameters = { new { ServerLogin = serverLogin } };
            await connection.ExecuteAsync(sql, parameters, tr);

            //Create db user
            object[] parametersA =
            {
                new { ServerLogin = serverLogin, DatabaseUser = databaseUser },
            };
            await connection.ExecuteAsync(sql1, parametersA, tr);

            //Add role to user
            object[] parametersB = { new { DatabaseUser = databaseUser } };
            await connection.ExecuteAsync(sql2, parametersB, tr);

            await tr.CommitAsync();
        }
        catch (Exception)
        {
            await tr.RollbackAsync();
            throw;
        }
    }

    public string BuildAcExeDbArgs(ISiteConfiguration site)
    {
        return site.DbUser == null
            ? $"-s:\"{site.SqlServerName}\" -d:\"{site.DbName}\""
            : $"-s:\"{site.SqlServerName}\" -d:\"{site.DbName}\" -sw:\"False\" -du:\"{site.DbUser}\" -dp:\"{site.DbPassword}\"";
    }
}
