using System.Text.Json.Serialization;

using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.JSON;

namespace ACUCustomizationUtils.Configuration.Site;

[JsonConverter(typeof(SiteConfigurationConverter))]
public interface ISiteConfiguration
{
    string? AcumaticaToolPath { get; }
    string? InstanceName { get; }
    string? InstancePath { get; }
    string? DbProvider { get; }
    string? SqlServerName { get; }
    string? DbPort { get; }
    string? DbName { get; }
    string? DbUser { get; }
    string? DbPassword { get; }
    string? DbConnectionString { get; }
    string? AcumaticaAdminName { get; }
    string? AcumaticaAdminPassword { get; }
    string? IisAppPool { get; }
    string? IisWebSite { get; }

    [JsonIgnore]
    bool IsNotNull { get; }
    ISiteConfiguration SetDefaultValues(IAcuConfiguration configuration);
    void DeriveDbConnectionString();
}
