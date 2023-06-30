using System.Text.Json.Serialization;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.JSON;

namespace ACUCustomizationUtils.Configuration.Code;

[JsonConverter(typeof(PackageConfigurationConverter))]
public interface ICodeConfiguration
{
    string? PkgSourceDirectory { get; }
    string? PkgDescription { get; }
    string? PkgLevel { get; }
    string? MsBuildSolutionFile { get; }
    string? MsBuildTargetDirectory { get; }
    string? MakeMode { get; }
    
    bool IsNotNull { get; }
    string? PkgSourceBinDirectory { get; }

    ICodeConfiguration SetDefaultValues(IAcuConfiguration configuration);
}