using ACUCustomizationUtils.Configuration;
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
        ValidateForSrc(config.Site);
        ValidateForSrc(config.Package);
        ValidateForSrc(config.Code);
    }

    public static void ValidateForMake(IAcuConfiguration config)
    {
        ValidateForMake(config.Package);
        ValidateForMake(config.Code);
    }

    public static void ValidateForCompile(ICodeConfiguration configuration)
    {
        Validate(configuration, new CodeCompileValidator());
    }

    private static void ValidateForSrc(ISiteConfiguration configuration)
    {
        Validate(configuration, new SiteGetSrcValidator());
    }

    private static void ValidateForSrc(IPackageConfiguration configuration)
    {
        Validate(configuration, new PackageGetSrcValidator());
    }

    private static void ValidateForSrc(ICodeConfiguration configuration)
    {
        Validate(configuration, new CodeSrcValidator());
    }

    private static void ValidateForMake(IPackageConfiguration configuration)
    {
        Validate(configuration, new PackageMakeValidator());
    }

    private static void ValidateForMake(ICodeConfiguration configuration)
    {
        Validate(configuration, new CodeMakeValidator());
    }
}