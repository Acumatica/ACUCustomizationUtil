using System.Text.Json.Serialization;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.JSON;

namespace ACUCustomizationUtils.Configuration.Src;

[JsonConverter(typeof(PackageConfigurationConverter))]
public interface ISrcConfiguration
{
    string? PkgSourceDirectory { get; }
    string? PkgDescription { get; }
    string? PkgLevel { get; }
    string? MsBuildSolutionFile { get; }
    string? MsBuildTargetDirectory { get; }
    string? MsBuildAssemblyName { get; }
    string? MakeMode { get; }
    bool IsNotNull { get; }
    string? PkgSourceBinDirectory { get; }

    ISrcConfiguration SetDefaultValues(IAcuConfiguration configuration);
}