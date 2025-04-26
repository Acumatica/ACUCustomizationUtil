using ACUCustomizationUtils.Configuration.Package;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Package;

internal class PackageGlobalValidator : AbstractValidator<IPackageConfiguration>
{
    public PackageGlobalValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.Url).NotNull();
        RuleFor(c => c.Login).NotNull();
        RuleFor(c => c.Password).NotNull();
        RuleFor(c => c.Tenant).NotNull().When(c => string.IsNullOrEmpty(c.Branch) == false);
    }
}