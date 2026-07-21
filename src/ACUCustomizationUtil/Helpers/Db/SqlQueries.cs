namespace ACUCustomizationUtils.Helpers.Db;

/// <summary>
/// Central holder for every SQL statement executed against the Acumatica instance database.
/// Dialect-neutral (ANSI) statements live under <see cref="Common"/>; SQL Server specific
/// statements under <see cref="MsSql"/>; MySQL specific statements under <see cref="MySql"/>.
/// The <see cref="IDbProvider"/> implementations read their dialect-specific text from here.
/// </summary>
/// <remarks>
/// Authored by Sprinterra
/// Copyright Sprinterra(c) 2023
/// </remarks>
internal static class SqlQueries
{
    /// <summary>
    /// Dialect-neutral (ANSI) statements that run unchanged on SQL Server and MySQL.
    /// </summary>
    internal static class Common
    {
        public const string UpdateAdminPassword =
            @"UPDATE users
                             SET LockedOutDate = @NullValue,
                                 LastLockedOutDate = @NullValue,
                                 FailedPasswordAttemptCount = @ZeroValue,
                                 Password = @Password,
                                 PasswordChangeOnNextLogin = @ZeroValue
                             WHERE Username = @UserName
                               AND CompanyID = @CompanyID";

        public const string GetCustomizationProject =
            @"SELECT * FROM CustProject WHERE Name = @ProjectName";

        public const string GetUploadFileRevision =
            @"SELECT *
                FROM UploadFileRevision
            WHERE FileID = @FileID
            AND FileRevisionID = @FileRevisionID";

        public const string GetUploadFile =
            @"SELECT *
                FROM UploadFile
            WHERE FileID = @FileID";
    }

    /// <summary>
    /// Microsoft SQL Server specific statements.
    /// </summary>
    internal static class MsSql
    {
        public const string CustObjectByProjectName =
            @"SELECT * FROM CustObject
                                 WHERE ProjectID IN
                                      (SELECT TOP 1 ProjID
                                       FROM CustProject
                                       WHERE Name = @ProjectName)
                                 ORDER by Type";

        public const string EnsureServerLogin =
            @"IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = @ServerLogin)
                              BEGIN
                                EXEC('
                                    CREATE LOGIN ['+@ServerLogin+']
                                    FROM WINDOWS WITH DEFAULT_DATABASE=[master],
                                    DEFAULT_LANGUAGE=[us_english]
                                ')
                              END";

        public const string EnsureDatabaseUser =
            @"IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @DatabaseUser)
                              BEGIN
                                EXEC('
                                    CREATE USER '+@DatabaseUser+'
                                    FOR LOGIN ['+@ServerLogin+']
                                ')
                              END";

        public const string AddDbOwnerRole =
            @"IF EXISTS (SELECT name FROM sys.database_principals WHERE name = @DatabaseUser)
                              BEGIN
                                EXEC sp_addrolemember 'db_owner', @DatabaseUser
                              END";
    }

    /// <summary>
    /// MySQL specific statements.
    /// </summary>
    internal static class MySql
    {
        // MySQL does not allow LIMIT directly inside an IN subquery,
        // so the limited subquery is wrapped in a derived table.
        public const string CustObjectByProjectName =
            @"SELECT * FROM CustObject
                                 WHERE ProjectID IN
                                      (SELECT ProjID FROM
                                          (SELECT ProjID
                                           FROM CustProject
                                           WHERE Name = @ProjectName
                                           LIMIT 1) AS proj)
                                 ORDER BY Type";
    }
}
