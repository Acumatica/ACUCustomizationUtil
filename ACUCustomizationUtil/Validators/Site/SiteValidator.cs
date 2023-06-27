using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.Site;
using ACUCustomizationUtils.Extensions;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Site;
/// <summary>
/// Configuration validator - validate required configuration parameters for execute Site subcommands 
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public abstract class SiteValidator : OptionsValidatorBase
{
    public static void ValidateForInstallV(IAcuConfiguration configuration)
    {
        Validate(configuration, new SiteInstallValidatorV());
    }
    
    public static void ValidateForInstall(ISiteConfiguration configuration)
    {
        Validate(configuration, new SiteInstallValidator());
    }

    public static void ValidateForUpdate(ISiteConfiguration configuration)
    {
        Validate(configuration, new SiteUpdateValidator());
    }

    public static void ValidateForDelete(ISiteConfiguration configuration)
    {
        Validate(configuration, new SiteUninstallValidator());
    }

    private static void Validate(ISiteConfiguration obj, IValidator<ISiteConfiguration> validator)
    {
        validator.ValidateAndThrowArgumentException(obj);
    }
}