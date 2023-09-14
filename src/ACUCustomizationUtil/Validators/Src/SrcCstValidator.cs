using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Configuration.Site;
using ACUCustomizationUtils.Configuration.Src;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Src;

internal class SiteCstValidator : AbstractValidator<ISiteConfiguration>
{
    public SiteCstValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.DbConnectionString).NotNull();
        RuleFor(c => c.InstancePath).NotNull().Must(Directory.Exists);
    }
}

internal class SrcCstValidator : AbstractValidator<ISrcConfiguration>
{
    public SrcCstValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.PkgSourceDirectory).NotNull();
    }
}

internal class PackageGetSrcValidator : AbstractValidator<IPackageConfiguration>
{
    public PackageGetSrcValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.PkgName).NotNull();
    }
}