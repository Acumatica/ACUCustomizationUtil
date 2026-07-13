using System.Data.Common;

using ACUCustomizationUtils.Configuration.Site;

namespace ACUCustomizationUtils.Helpers.Db;

/// <summary>
/// Abstraction over the database engine (SQL Server / MySQL) hosting the Acumatica instance database.
/// Encapsulates every provider-specific concern: ADO.NET client, connection string derivation,
/// SQL dialect differences, post-install application login setup and ac.exe database arguments.
/// </summary>
/// <remarks>
/// Authored by Sprinterra
/// Copyright Sprinterra(c) 2023
/// </remarks>
public interface IDbProvider
{
    /// <summary>
    /// Creates a closed <see cref="DbConnection"/> for the given connection string.
    /// </summary>
    DbConnection CreateConnection(string connectionString);

    /// <summary>
    /// Derives a connection string from the discrete site configuration fields
    /// (server, database, user, password). Returns null when required fields are missing.
    /// </summary>
    string? BuildConnectionString(ISiteConfiguration site);

    /// <summary>
    /// Dialect-correct query returning all CustObject rows of the customization project
    /// with the given name (@ProjectName parameter).
    /// </summary>
    string CustObjectByProjectNameSql { get; }

    /// <summary>
    /// Ensures the Acumatica web application can authenticate to the instance database
    /// after a new site installation.
    /// </summary>
    Task EnsureAppLoginAsync(Func<DbConnection> connectionFactory, ISiteConfiguration site);

    /// <summary>
    /// Builds the database-related fragment of the ac.exe command line
    /// (server, database and authentication arguments).
    /// </summary>
    string BuildAcExeDbArgs(ISiteConfiguration site);
}
