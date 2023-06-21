using ACUCustomizationUtils.Configuration.Code;
using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Configuration.Site;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Code;

internal class SiteGetSrcValidator : AbstractValidator<ISiteConfiguration>
{
    public SiteGetSrcValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.DbConnectionString).NotNull();
        RuleFor(c => c.SitePhysicalPath).NotNull().Must(Directory.Exists);
    }
}

internal class CodeSrcValidator : AbstractValidator<ICodeConfiguration>
{
    public CodeSrcValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.PkgSourceDirectory).NotNull().Must(Directory.Exists);
    }
}

internal class PackageGetSrcValidator : AbstractValidator<IPackageConfiguration>
{
    public PackageGetSrcValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.PackageName).NotNull();
    }
}