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

    public string CustObjectByProjectNameSql => SqlQueries.MsSql.CustObjectByProjectName;

    public async Task EnsureAppLoginAsync(
        Func<DbConnection> connectionFactory,
        ISiteConfiguration site
    )
    {
        const string serverLogin = @"IIS APPPOOL\DefaultAppPool";
        const string databaseUser = @"DefaultAppPool";

        await using DbConnection connection = connectionFactory();
        await connection.OpenAsync();
        DbTransaction tr = await connection.BeginTransactionAsync();
        try
        {
            //Create login
            object[] parameters = { new { ServerLogin = serverLogin } };
            await connection.ExecuteAsync(SqlQueries.MsSql.EnsureServerLogin, parameters, tr);

            //Create db user
            object[] parametersA =
            {
                new { ServerLogin = serverLogin, DatabaseUser = databaseUser },
            };
            await connection.ExecuteAsync(SqlQueries.MsSql.EnsureDatabaseUser, parametersA, tr);

            //Add role to user
            object[] parametersB = { new { DatabaseUser = databaseUser } };
            await connection.ExecuteAsync(SqlQueries.MsSql.AddDbOwnerRole, parametersB, tr);
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
