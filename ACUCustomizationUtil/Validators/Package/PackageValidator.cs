using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Extensions;
using ACUCustomizationUtils.Validators.Site;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Package;
/// <summary>
/// Configuration validator - validate required configuration parameters for execute Package subcommands 
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public abstract class PackageValidator : OptionsValidatorBase
{
    public static void ValidateForGet(IPackageConfiguration configuration)
    {
        Validate(configuration, new PackageGetValidator());
    }

    public static void ValidateForPublish(IPackageConfiguration configuration)
    {
        Validate(configuration, new PackagePublishValidator());
    }

    public static void ValidateForUnpublish(IPackageConfiguration configuration)
    {
        Validate(configuration, new PackageUnpublishValidator());
    }

    public static void ValidateForUpload(IPackageConfiguration configuration)
    {
        Validate(configuration, new PackageUploadValidator());
    }

    private static void Validate(IPackageConfiguration obj, IValidator<IPackageConfiguration> validator)
    {
        validator.ValidateAndThrowArgumentException(obj);
    }
}