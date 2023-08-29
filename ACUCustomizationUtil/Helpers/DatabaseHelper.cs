using System.Data.Common;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;
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
        const int companyId = 2;
        const string nullValue = null!;
        const int zeroValue = 0;
        

        var sql = $"UPDATE Users " +
                  $"SET LockedOutDate = null, " +
                  $"    LastLockedOutDate = null, " +
                  $"    FailedPasswordAttemptCount = 0," +
                  $"    Password = '{password}', " +
                  $"    PasswordChangeOnNextLogin = 0 " +
                  $"WHERE Username = '{username}' " +
                  $"AND CompanyID = {companyId}";
        
        
        var sql2 = @"UPDATE Users 
                        SET LockedOutDate = null, 
                            LastLockedOutDate = null, 
                            FailedPasswordAttemptCount = @ZeroValue, 
                            Password = @Password,     
                            PasswordChangeOnNextLogin = @ZaroValue 
                        WHERE Username = @UserName 
                        AND CompanyID = @CompanyID";
        object[] parameters =
        {
            new { NullValue = nullValue,  ZeroValue = zeroValue, UserName = username, Password = password, CompanyId = companyId }
        }; 
        await using var connection = _connectionFactory();
        var rows = await connection.ExecuteAsync(sql2, parameters);
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