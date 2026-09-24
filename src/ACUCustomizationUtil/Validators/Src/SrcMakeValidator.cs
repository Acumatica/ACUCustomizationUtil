using ACUCustomizationUtils.Common;
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
        RuleFor(c => c.PkgVersion)
            .Matches(Messages.PackageVersionPattern)
            .When(c => c.PkgVersion != null)
            .WithMessage("Package version should be in the form: yyDDD.HHmm or 0.0.yyDDD.HHmm");
    }
}
