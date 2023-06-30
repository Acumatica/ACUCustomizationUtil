using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Extensions;

namespace ACUCustomizationUtils.Configuration.Site;
/// <summary>
/// POCO configuration class for acu util (Site section)
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public abstract class SiteConfigurationBase : ISiteConfiguration
{
    public string? AcumaticaToolPath { get; set; }
    public string? InstanceName { get; set; }
    public string? InstancePath { get; set; }
    public string? SqlServerName { get; set; }
    public string? DbName { get; set; }
    public string? DbConnectionString { get; set; }
    public string? AcumaticaAdminName { get; set; }
    public string? AcumaticaAdminPassword { get; set; }
    public string? IisAppPool { get; set; }
    public string? IisDbUsername { get; set; }
    public string? IisWebSite { get; set; }
    public abstract bool IsNotNull { get; }

    public ISiteConfiguration SetDefaultValues(IAcuConfiguration configuration)
    {
        AcumaticaToolPath ??= configuration.Erp.InstallationDirectory != null
            ? Path.Combine(configuration.Erp.InstallationDirectory!, "Acumatica ERP\\Data\\ac.exe")
            : null;
        SqlServerName ??= "localhost";
        InstancePath = InstancePath.TryGetFullDirectoryPath();  
        DbName ??= InstanceName != null ? $"{InstanceName}DB" : null;
        DbConnectionString ??= SqlServerName != null && DbName != null
            ? $"Data Source={SqlServerName};Initial Catalog={DbName};Integrated Security=True;Encrypt=False;"
            : null;
        IisAppPool ??= "DefaultAppPool";
        IisWebSite ??= "Default Web Site";
        IisDbUsername ??= null;

        return this;
    }
}