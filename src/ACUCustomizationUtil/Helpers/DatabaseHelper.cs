using System.Data.Common;
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
        
        const string sql = @"UPDATE users 
                             SET LockedOutDate = @NullValue, 
                                 LastLockedOutDate = @NullValue, 
                                 FailedPasswordAttemptCount = @ZeroValue, 
                                 Password = @Password,     
                                 PasswordChangeOnNextLogin = @ZeroValue 
                             WHERE Username = @UserName 
                               AND CompanyID = @CompanyID";
        object[] parameters =
        {
            new { NullValue = nullValue,  ZeroValue = zeroValue, UserName = username, Password = password, CompanyId = companyId }
        }; 
        await using var connection = _connectionFactory();
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task UpdateServerLoginDefault()
    {
        const string serverLogin = @"IIS APPPOOL\DefaultAppPool";
        const string databaseUser = @"DefaultAppPool";
        
        const string sql = @"IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = @ServerLogin)
                             BEGIN
                                CREATE LOGIN [@ServerLogin]
                                FROM WINDOWS WITH DEFAULT_DATABASE=[master],
                                DEFAULT_LANGUAGE=[us_english]
                            END";
        
        const string sql1 = @"IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @DatabaseUser)
                              BEGIN
                                CREATE USER [@DatabaseUser]
                                FOR LOGIN [@ServerLogin]
                              END";
        
        const string sql2 = @"IF EXISTS (SELECT name FROM sys.database_principals WHERE name = @DatabaseUser)
                              BEGIN
                                EXEC sp_addrolemember 'db_owner', @DatabaseUser
                              END";
        
        await using var connection = _connectionFactory();
        await connection.OpenAsync();
        var tr = await connection.BeginTransactionAsync();
        try
        {
            //Create login
            object[] parameters = { new {ServerLogin = serverLogin } };
            await connection.ExecuteAsync(sql, parameters, tr);

            //Create db user
            object[] parametersA = { new {ServerLogin = serverLogin, DatabaseUser = databaseUser } };
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

    public async Task<IEnumerable<CustomizationProjectEntity>?> GetCustomizationProjectEntitiesAsync(string projectName)
    {
        const string sql = @"SELECT * FROM CustObject 
                                 WHERE ProjectID IN 
                                      (SELECT TOP 1 ProjID 
                                       FROM CustProject 
                                       WHERE Name = @ProjectName) 
                                 ORDER by Type";
        
        object[] parameters = { new {ProjectName = projectName } };
        await using var connection = _connectionFactory();
        return await connection.QueryAsync<CustomizationProjectEntity>(sql, parameters);
    }

    public async Task<CustomizationProject?> GetCustomizationProjectAsync(string projectName)
    {
        const string sql = @"SELECT * FROM CustProject WHERE Name = @ProjectName";
        object[] parameters = { new {ProjectName = projectName } };
        await using var connection = _connectionFactory();
        return await connection.QueryFirstAsync<CustomizationProject>(sql, parameters);
    }

}