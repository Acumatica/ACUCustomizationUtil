using ACUCustomizationUtils.Configuration.Package;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Package;

internal class PackageGetValidator : AbstractValidator<IPackageConfiguration>
{
    public PackageGetValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.PackageName).NotNull();
        RuleFor(c => c.PackageDirectory).NotNull();
    }
}