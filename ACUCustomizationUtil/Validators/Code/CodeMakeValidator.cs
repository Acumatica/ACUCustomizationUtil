using ACUCustomizationUtils.Configuration.Code;
using ACUCustomizationUtils.Configuration.Package;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Code;

internal class PackageMakeValidator : AbstractValidator<IPackageConfiguration>
{
    public PackageMakeValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.PackageName).NotNull();
        RuleFor(c => c.PackageDirectory).NotNull(); //.Must(Directory.Exists);
    }
}

internal class CodeMakeValidator : AbstractValidator<ICodeConfiguration>
{
    public CodeMakeValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.PkgSourceDirectory).NotNull().Must(Directory.Exists);
    }
}