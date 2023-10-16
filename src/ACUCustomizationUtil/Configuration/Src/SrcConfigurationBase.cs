using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Extensions;

namespace ACUCustomizationUtils.Configuration.Src;

/// <summary>
/// POCO configuration class for acu util (Code section)
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public abstract class SrcConfigurationBase : ISrcConfiguration
{
    public string? PkgSourceDirectory { get; set; }
    public string? PkgDescription { get; set; }
    public string? PkgLevel { get; set; }
    public string? MsBuildSolutionFile { get; set; }
    public string? MsBuildTargetDirectory { get; set; }
    public string? MsBuildAssemblyName { get; set; }
    public string? MakeMode { get; set; }
    public abstract bool IsNotNull { get; }

    public string? PkgSourceBinDirectory { get; private set; }

    public ISrcConfiguration SetDefaultValues(IAcuConfiguration configuration)
    {
        PkgSourceDirectory = PkgSourceDirectory.TryGetFullDirectoryPath();
        MsBuildTargetDirectory = MsBuildTargetDirectory.TryGetFullDirectoryPath();
        MsBuildSolutionFile = MsBuildSolutionFile.TryGetFullDirectoryPath();
        if (PkgSourceDirectory != null) PkgSourceBinDirectory = Path.Combine(PkgSourceDirectory, "Bin");
        
        return this;
    }
}