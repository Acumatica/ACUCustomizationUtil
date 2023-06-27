using System.Text.Json.Serialization;
using ACUCustomizationUtils.JSON;

namespace ACUCustomizationUtils.Configuration.Erp;

[JsonConverter(typeof(ErpConfigurationConverter))]
public interface IErpConfiguration
{
    Uri? Url { get; }
    string? ErpVersion { get; }
    string? InstallationFileName { get; }
    string? DestinationDirectory { get; }
    [JsonIgnore] string? InstallationDirectory { get; }
    [JsonIgnore] string? InstallationFilePath { get; }
    [JsonIgnore] bool IsNotNull { get; }
    IErpConfiguration SetDefaultValues(IAcuConfiguration configuration);
}