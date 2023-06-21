using System.Text.Json.Serialization;
using ACUCustomizationUtils.JSON;

namespace ACUCustomizationUtils.Configuration.Site;

[JsonConverter(typeof(SiteConfigurationConverter))]
public interface ISiteConfiguration
{
    string? AcumaticaToolPath { get; }
    string? InstanceName { get; }
    string? InstancePath { get; }
    string? SqlServerName { get; }
    string? DbName { get; }
    string? DbConnectionString { get; }
    string? AcumaticaAdminName { get; }
    string? AcumaticaAdminPassword { get; }
    string? SitePhysicalPath { get; }
    string? IisAppPool { get; }
    string? IisWebSite { get; }
    [JsonIgnore] bool IsNotNull { get; }
    ISiteConfiguration SetDefaultValues(IAcuConfiguration configuration);
}