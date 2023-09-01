using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Configuration.ACU;

namespace ACUCustomizationUtils.Configuration.Erp;
/// <summary>
/// POCO configuration class for acu util (ERP section)
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public abstract class ErpConfigurationBase : IErpConfiguration
{
    public Uri? Url { get; set; }
    public string? ErpVersion { get; set; }
    public string? InstallationFileName { get; set; }
    public string? DestinationDirectory { get; set; }
    public string? InstallationDirectory { get; private set; }
    public string? InstallationFilePath { get; private set; }
    public abstract bool IsNotNull { get; }

    public IErpConfiguration SetDefaultValues(IAcuConfiguration configuration)
    {
        Url ??= GetDownloadUri(ErpVersion);
        InstallationFileName ??= Messages.AcumaticaErpInstallMsi;
        InstallationFilePath ??= GetErpInstallationFile(DestinationDirectory, InstallationFileName);
        InstallationDirectory ??= GetErpInstallationDirectory(DestinationDirectory, ErpVersion);
        return this;
    }

    private static string? GetErpInstallationFile(string? directory, string? file)
    {
        if (directory != null && file != null)
        {
            return Path.Combine(directory, file);
        }

        return null;
    }

    private static Uri? GetDownloadUri(string? version)
    {
        if (version == null) return null;
        var majorNbr = version[..4];
        var uri = new UriBuilder(Messages.DownloadUri(majorNbr, version)).Uri;

        return uri;
    }

    private static string? GetErpInstallationDirectory(string? targetDir, string? version)
    {
        if (version != null && targetDir != null)
        {
            return Path.Combine(targetDir, version);
        }

        return null;
    }
}