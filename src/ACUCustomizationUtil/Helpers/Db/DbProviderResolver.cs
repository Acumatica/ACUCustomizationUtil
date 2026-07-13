using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Configuration.Site;

namespace ACUCustomizationUtils.Helpers.Db;

/// <summary>
/// Resolves the <see cref="IDbProvider"/> implementation from the site configuration
/// dbProvider value ("mssql" is the default when the value is not set).
/// </summary>
/// <remarks>
/// Authored by Sprinterra
/// Copyright Sprinterra(c) 2023
/// </remarks>
public static class DbProviderResolver
{
    private static readonly MsSqlDbProvider MsSql = new MsSqlDbProvider();
    private static readonly MySqlDbProvider MySql = new MySqlDbProvider();

    public static IDbProvider Resolve(ISiteConfiguration site)
    {
        return TryResolve(site)
            ?? throw new ArgumentException(
                $"Unknown database provider '{site.DbProvider}'. Supported values: '{Messages.DbProviderMsSql}', '{Messages.DbProviderMySql}'."
            );
    }

    public static IDbProvider? TryResolve(ISiteConfiguration site)
    {
        return TryResolve(site.DbProvider);
    }

    public static IDbProvider? TryResolve(string? dbProvider)
    {
        if (dbProvider == null || dbProvider.Equals(Messages.DbProviderMsSql, StringComparison.OrdinalIgnoreCase))
            return MsSql;
        if (dbProvider.Equals(Messages.DbProviderMySql, StringComparison.OrdinalIgnoreCase))
            return MySql;
        return null;
    }
}
