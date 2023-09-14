using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Configuration.Src;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Src;

internal class PackageMakeValidator : AbstractValidator<IPackageConfiguration>
{
    public PackageMakeValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.PkgName).NotNull();
        RuleFor(c => c.PkgDirectory).NotNull(); //.Must(Directory.Exists);
    }
}

internal class PackageBuildValidator : AbstractValidator<IPackageConfiguration>
{
    public PackageBuildValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.PkgName).NotNull();
    }
}

internal class SrcMakeValidator : AbstractValidator<ISrcConfiguration>
{
    public SrcMakeValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.PkgSourceDirectory).NotNull().Must(Directory.Exists);
    }
}