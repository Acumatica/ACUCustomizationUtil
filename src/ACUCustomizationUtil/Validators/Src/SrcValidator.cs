using ACUCustomizationUtils.Configuration.ACU;

namespace ACUCustomizationUtils.Validators.Src;
/// <summary>
/// Configuration validator - validate required configuration parameters for execute Code subcommands 
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public abstract class SrcValidator : OptionsValidatorBase
{
    public static void ValidateForSrc(IAcuConfiguration config)
    {
        Validate(config.Site, new SiteCstValidator());
        Validate(config.Pkg, new PackageGetSrcValidator());
        Validate(config.Src, new SrcCstValidator());
    }

    public static void ValidateForMake(IAcuConfiguration config)
    {
        Validate(config.Pkg, new PackageMakeValidator());
        Validate(config.Src, new SrcMakeValidator());
    }

    public static void ValidateForBuild(IAcuConfiguration configuration)
    {
        Validate(configuration.Pkg, new PackageBuildValidator());
        Validate(configuration.Src, new SrcBuildValidator());
    }
    
}