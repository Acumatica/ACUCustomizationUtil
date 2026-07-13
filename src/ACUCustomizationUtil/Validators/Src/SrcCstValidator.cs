using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Configuration.Site;
using ACUCustomizationUtils.Configuration.Src;
using ACUCustomizationUtils.Helpers.Db;

using FluentValidation;

namespace ACUCustomizationUtils.Validators.Src;

internal class SiteCstValidator : AbstractValidator<ISiteConfiguration>
{
    public SiteCstValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.DbProvider)
            .Must(p => DbProviderResolver.TryResolve(p) != null)
            .WithMessage("DbProvider must be one of: mssql, mysql");
        RuleFor(c => c.DbConnectionString)
            .NotNull()
            .WithMessage(
                "DbConnectionString is required. Set dbConnectionString explicitly or provide the discrete fields it is derived from (sqlServerName, dbName and, for mysql, dbUser/dbPassword)."
            );
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
