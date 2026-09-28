namespace ACUCustomizationUtils.Common;

internal static class Messages
{
    public const string AcumaticaErpInstallMsi = "AcumaticaERPInstall.msi";
    public const string Msiexec = "msiexec.exe";
    public const string Acu = "ACU";
    public const string Copyright = "Copyright Sprinterra(c) 2023";

    public static string DownloadUri(string majorNbr, string? version) =>
        $"http://acumatica-builds.s3.amazonaws.com/builds/{majorNbr}/{version}/AcumaticaERP/{AcumaticaErpInstallMsi}";

    public const string MakeModeBase = "Base";
    public const string MakeModeISV = "ISV";
    public const string MakeModeQA = "QA";

    public const string DbProviderMsSql = "mssql";
    public const string DbProviderMySql = "mysql";
    public const string MySqlDefaultPort = "3306";

    public const string DeserializeError = "Deserialization failed";
    public const string ErrorLogType = "error";

    public static class TenantMode
    {
        public const string Current = "Current";
        public const string All = "All";
        public const string List = "List";
    }
}
