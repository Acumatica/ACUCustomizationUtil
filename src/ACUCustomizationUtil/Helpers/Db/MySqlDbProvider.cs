using System.Data.Common;

using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Configuration.Site;

using MySqlConnector;

namespace ACUCustomizationUtils.Helpers.Db;

/// <summary>
/// MySQL database provider. Acumatica on MySQL always authenticates with explicit
/// database credentials (no Windows integrated security), so a database user and password
/// are required both for the derived connection string and for ac.exe arguments.
/// </summary>
/// <remarks>
/// Authored by Sprinterra
/// Copyright Sprinterra(c) 2023
/// </remarks>
public class MySqlDbProvider : IDbProvider
{
    public DbConnection CreateConnection(string connectionString)
    {
        return new MySqlConnection(connectionString);
    }

    public string? BuildConnectionString(ISiteConfiguration site)
    {
        if (site.SqlServerName == null || site.DbName == null || site.DbUser == null)
            return null;

        string port = site.DbPort ?? Messages.MySqlDefaultPort;
        return $"Server={site.SqlServerName};Port={port};Database={site.DbName};User ID={site.DbUser};Password={site.DbPassword};AllowUserVariables=True;";
    }

    public string CustObjectByProjectNameSql => SqlQueries.MySql.CustObjectByProjectName;

    public Task EnsureAppLoginAsync(Func<DbConnection> connectionFactory, ISiteConfiguration site)
    {
        // No action is required for MySQL: the Acumatica web application authenticates
        // to MySQL with the explicit database credentials (dbUser/dbPassword) that the
        // configuration tool stores in web.config during installation. There is no
        // IIS application pool database login concept on MySQL.
        return Task.CompletedTask;
    }

    public string BuildAcExeDbArgs(ISiteConfiguration site)
    {
        // ac.exe distinguishes two independent database logins:
        //  * the setup connection ac.exe uses to create/upgrade the database
        //    ("Database Server ..." => -sw/-u/-p), and
        //  * the application connection written into the instance and used by the web app at
        //    runtime ("DB Connection ..." => -dw/-dn/-du/-dp).
        // Both default to Windows authentication (-sw and -dw default to True). MySQL has no
        // Windows integrated security, so both must be forced to server authentication; otherwise
        // ac.exe's "Creating a User" stage passes a null password to PointMySql and throws a
        // NullReferenceException. -dn:"False" grants rights to the existing MySQL user instead of
        // attempting to create it. The same credentials are reused for setup and application.
        return $"-t:\"MySql\" -s:\"{site.SqlServerName}\" -d:\"{site.DbName}\" "
            + $"-sw:\"False\" -u:\"{site.DbUser}\" -p:\"{site.DbPassword}\" "
            + $"-dw:\"False\" -dn:\"False\" -du:\"{site.DbUser}\" -dp:\"{site.DbPassword}\"";
    }
}
