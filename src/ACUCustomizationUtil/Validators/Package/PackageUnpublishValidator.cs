using ACUCustomizationUtils.Configuration.Package;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Package;

internal class PackageUnpublishValidator : AbstractValidator<IPackageConfiguration>
{
    public PackageUnpublishValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
    }
}