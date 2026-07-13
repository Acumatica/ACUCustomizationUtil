using System.Diagnostics;

using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Configuration.Site;
using ACUCustomizationUtils.Helpers.Db;

using FluentValidation;

namespace ACUCustomizationUtils.Validators.Site;

internal class SiteInstallValidator : AbstractValidator<ISiteConfiguration>
{
    public SiteInstallValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.AcumaticaToolPath).NotNull();
        RuleFor(c => c.InstanceName).NotNull();
        RuleFor(c => c.InstancePath).NotNull();
        RuleFor(c => c.DbProvider)
            .Must(p => DbProviderResolver.TryResolve(p) != null)
            .WithMessage("DbProvider must be one of: mssql, mysql");
        RuleFor(c => c.SqlServerName).NotNull();
        RuleFor(c => c.DbConnectionString).NotNull();
        RuleFor(c => c.DbName).NotNull();
        RuleFor(c => c.DbUser)
            .NotNull()
            .When(c => IsMySql(c))
            .WithMessage("DbUser is required when dbProvider is mysql");
        RuleFor(c => c.DbPassword)
            .NotNull()
            .When(c => IsMySql(c))
            .WithMessage("DbPassword is required when dbProvider is mysql");
        RuleFor(c => c.IisAppPool).NotNull();
        RuleFor(c => c.IisWebSite).NotNull();
    }

    private static bool IsMySql(ISiteConfiguration site)
    {
        return Messages.DbProviderMySql.Equals(
            site.DbProvider,
            StringComparison.OrdinalIgnoreCase
        );
    }
}

internal class SiteInstallValidatorV : AbstractValidator<IAcuConfiguration>
{
    public SiteInstallValidatorV()
    {
        RuleFor(c => c)
            .Must(c =>
                c.Erp.ErpVersion
                == FileVersionInfo.GetVersionInfo(c.Site.AcumaticaToolPath!).FileVersion
            )
            .WithMessage("Acumatica tool file version is not equal configured ERP version");
    }
}
