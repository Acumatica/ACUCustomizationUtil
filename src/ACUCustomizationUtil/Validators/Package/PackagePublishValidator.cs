using ACUCustomizationUtils.Configuration.Package;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Package;

internal class PackagePublishValidator : AbstractValidator<IPackageConfiguration>
{
    public PackagePublishValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.PkgName).NotNull();
    }
}