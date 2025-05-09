using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.JSON;
using System.Text.Json.Serialization;

namespace ACUCustomizationUtils.Configuration.Package;

[JsonConverter(typeof(PackageConfigurationConverter))]
public interface IPackageConfiguration
{
    Uri? Url { get; }
    string? Login { get; }
    string? Password { get; }
    string? Tenant { get; set; }
    string? Branch { get; set; }
    string? PkgName { get; }
    string? PkgDirectory { get; }
    string? PkgSuffix { get; set; }

    [JsonIgnore]
    string? PackageFilePath { get; }

    [JsonIgnore]
    bool IsNotNull { get; }

    IPackageConfiguration SetDefaultValues(IAcuConfiguration configuration);
}
