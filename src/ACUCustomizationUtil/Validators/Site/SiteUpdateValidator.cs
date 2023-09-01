using ACUCustomizationUtils.Configuration.Site;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Site;

internal class SiteUpdateValidator : AbstractValidator<ISiteConfiguration>
{
    public SiteUpdateValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.AcumaticaToolPath).NotNull();
        RuleFor(c => c.InstanceName).NotNull();
        RuleFor(c => c.SqlServerName).NotNull();
        RuleFor(c => c.DbName).NotNull();
    }
}