using System.Data.Common;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Helpers.CommonTypes;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ACUCustomizationUtils.Helpers;

public class DatabaseHelper
{
    private readonly Func<DbConnection> _connectionFactory;
    private readonly IAcuConfiguration _config;

    public DatabaseHelper(IAcuConfiguration config)
    {
        _config = config;
        _connectionFactory = () => new SqlConnection(_config.Site.DbConnectionString);
    }

    public async Task UpdateAdminPasswordDefault()
    {
        var username = _config.Site.AcumaticaAdminName;
        var password = _config.Site.AcumaticaAdminPassword;
        const string companyId = "2";

        var sql = $"update Users " +
                  $"set LockedOutDate = null, " +
                  $"    LastLockedOutDate = null, " +
                  $"    FailedPasswordAttemptCount = 0," +
                  $"    Password = '{password}', " +
                  $"    PasswordChangeOnNextLogin = 0 " +
                  $"WHERE Username = '{username}' " +
                  $"AND CompanyID = {companyId}";

        await using var connection = _connectionFactory();
        var rows = await connection.ExecuteAsync(sql);
    }

    public async Task<IEnumerable<CustomizationProjectEntity>> GetCustomizationProjectEntitiesAsync(
        string projectName)
    {
        var sql =
            $"select * from CustObject where ProjectID in (select top 1 ProjID from CustProject where Name = '{projectName}') order by Type";
        await using var connection = _connectionFactory();
        return await connection.QueryAsync<CustomizationProjectEntity>(sql);
    }

    public async Task<CustomizationProject> GetCustomizationProjectAsync(string projectName)
    {
        var sql = $"select * from CustProject where name = '{projectName}'";
        await using var connection = _connectionFactory();
        return await connection.QueryFirstAsync<CustomizationProject>(sql);
    }
}