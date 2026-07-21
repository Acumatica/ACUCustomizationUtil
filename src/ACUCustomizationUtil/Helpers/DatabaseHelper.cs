using System.Data.Common;

using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Helpers.CommonTypes;
using ACUCustomizationUtils.Helpers.Db;

using Dapper;

namespace ACUCustomizationUtils.Helpers;

public class DatabaseHelper
{
    private readonly Func<DbConnection> _connectionFactory;
    private readonly IAcuConfiguration _config;
    private readonly IDbProvider _provider;

    public DatabaseHelper(IAcuConfiguration config)
    {
        _config = config;
        _provider = DbProviderResolver.Resolve(config.Site);
        _connectionFactory = () => _provider.CreateConnection(_config.Site.DbConnectionString!);
    }

    public async Task UpdateAdminPasswordDefault()
    {
        string? username = _config.Site.AcumaticaAdminName;
        string? password = _config.Site.AcumaticaAdminPassword;
        const int companyId = 2;
        const string nullValue = null!;
        const int zeroValue = 0;

        object[] parameters =
        {
            new
            {
                NullValue = nullValue,
                ZeroValue = zeroValue,
                UserName = username,
                Password = password,
                CompanyId = companyId,
            },
        };
        await using DbConnection connection = _connectionFactory();
        await connection.ExecuteAsync(SqlQueries.Common.UpdateAdminPassword, parameters);
    }

    public async Task UpdateServerLoginDefault()
    {
        await _provider.EnsureAppLoginAsync(_connectionFactory, _config.Site);
    }

    public async Task<IEnumerable<CustomizationProjectEntity>?> GetCustomizationProjectEntitiesAsync(string projectName)
    {
        string sql = _provider.CustObjectByProjectNameSql;

        object param = new { ProjectName = projectName };
        await using DbConnection connection = _connectionFactory();
        return await connection.QueryAsync<CustomizationProjectEntity>(sql, param);
    }

    public async Task<CustomizationProject?> GetCustomizationProjectAsync(string projectName)
    {
        object param = new { ProjectName = projectName };
        await using DbConnection connection = _connectionFactory();
        return await connection.QuerySingleAsync<CustomizationProject>(SqlQueries.Common.GetCustomizationProject, param);
    }

    public async Task<UploadFileRevision> GetUploadFileRevision(string fileID, int? revisionID)
    {
        object param = new { FileID = fileID, FileRevisionID = revisionID };
        await using DbConnection connection = _connectionFactory();
        return await connection.QuerySingleAsync<UploadFileRevision>(SqlQueries.Common.GetUploadFileRevision, param);
    }
    public async Task<UploadFile> GetUploadFile(string fileID)
    {
        object param = new { FileID = fileID };
        await using DbConnection connection = _connectionFactory();
        return await connection.QuerySingleAsync<UploadFile>(SqlQueries.Common.GetUploadFile, param);
    }
}
