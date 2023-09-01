using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Configuration.Code;
using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Configuration.Site;

namespace ACUCustomizationUtils.Validators.Code;
/// <summary>
/// Configuration validator - validate required configuration parameters for execute Code subcommands 
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public abstract class CodeValidator : OptionsValidatorBase
{
    public static void ValidateForSrc(IAcuConfiguration config)
    {
        Validate(config.Site, new SiteGetSrcValidator());
        Validate(config.Package, new PackageGetSrcValidator());
        Validate(config.Code, new CodeSrcValidator());
    }

    public static void ValidateForMake(IAcuConfiguration config)
    {
        Validate(config.Package, new PackageMakeValidator());
        Validate(config.Code, new CodeMakeValidator());
    }

    public static void ValidateForCompile(IAcuConfiguration configuration)
    {
        Validate(configuration.Package, new PackageCompileValidator());
        Validate(configuration.Code, new CodeCompileValidator());
    }
    
}