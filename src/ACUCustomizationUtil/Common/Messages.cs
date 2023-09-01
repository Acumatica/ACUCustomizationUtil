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
    public const string MakeModeNAW = "NAW";
}