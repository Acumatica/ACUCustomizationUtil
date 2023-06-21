using System.Text.Json.Serialization;
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
    bool? MakeQA { get; }
    bool? MakeISV { get; }
    bool IsNotNull { get; }
    string? PkgSourceBinDirectory { get; }

    ICodeConfiguration SetDefaultValues(IAcuConfiguration configuration);
}