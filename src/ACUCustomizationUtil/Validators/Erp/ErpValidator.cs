using ACUCustomizationUtils.Configuration.Erp;
using ACUCustomizationUtils.Extensions;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Erp;
/// <summary>
/// Configuration validator - validate required configuration parameters for execute Erp subcommands 
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public abstract class ErpValidator : OptionsValidatorBase
{
    public static void ValidateForDownload(IErpConfiguration configuration)
    {
        Validate(configuration, new ErpDownloadValidator());
    }

    public static void ValidateForInstall(IErpConfiguration configuration)
    {
        Validate(configuration, new ErpInstallValidator());
    }

    public static void ValidateForDelete(IErpConfiguration configuration)
    {
        Validate(configuration, new ErpUninstallValidator());
    }

    private static void Validate(IErpConfiguration obj, IValidator<IErpConfiguration> validator)
    {
        validator.ValidateAndThrowArgumentException(obj);
    }
}