using ACUCustomizationUtils.Configuration.Package;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Package;

internal class PackageUploadValidator : AbstractValidator<IPackageConfiguration>
{
    public PackageUploadValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.PkgName).NotNull();
        RuleFor(c => c.PkgDirectory).NotNull();
        RuleFor(c => c.PackageFilePath).NotNull();
        RuleFor(c => c.PackageFilePath).Must(File.Exists).WithMessage("Package {PropertyValue} is not found!");
    }
}