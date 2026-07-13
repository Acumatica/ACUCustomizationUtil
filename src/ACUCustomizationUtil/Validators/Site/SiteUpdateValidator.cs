using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Configuration.Site;
using ACUCustomizationUtils.Helpers.Db;

using FluentValidation;

namespace ACUCustomizationUtils.Validators.Site;

internal class SiteUpdateValidator : AbstractValidator<ISiteConfiguration>
{
    public SiteUpdateValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.AcumaticaToolPath).NotNull();
        RuleFor(c => c.InstanceName).NotNull();
        RuleFor(c => c.DbProvider)
            .Must(p => DbProviderResolver.TryResolve(p) != null)
            .WithMessage("DbProvider must be one of: mssql, mysql");
        RuleFor(c => c.SqlServerName).NotNull();
        RuleFor(c => c.DbName).NotNull();
        RuleFor(c => c.DbUser)
            .NotNull()
            .When(c =>
                Messages.DbProviderMySql.Equals(c.DbProvider, StringComparison.OrdinalIgnoreCase)
            )
            .WithMessage("DbUser is required when dbProvider is mysql");
        RuleFor(c => c.DbPassword)
            .NotNull()
            .When(c =>
                Messages.DbProviderMySql.Equals(c.DbProvider, StringComparison.OrdinalIgnoreCase)
            )
            .WithMessage("DbPassword is required when dbProvider is mysql");
    }
}
