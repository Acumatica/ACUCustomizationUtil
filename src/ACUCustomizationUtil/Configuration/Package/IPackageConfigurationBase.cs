using System.Text.Json.Serialization;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.JSON;

namespace ACUCustomizationUtils.Configuration.Package;

[JsonConverter(typeof(PackageConfigurationConverter))]
public interface IPackageConfiguration
{
    Uri? Url { get; }
    string? Login { get; }
    string? Password { get; }
    string? PkgName { get; }
    string? Tenant { get; set; }
    string? PkgDirectory { get; }
    [JsonIgnore] string? PackageFilePath { get; }
    [JsonIgnore] bool IsNotNull { get; }

    IPackageConfiguration SetDefaultValues(IAcuConfiguration configuration);
}