using ACUCustomizationUtils.Configuration.Site;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Site;

internal class SiteUninstallValidator : AbstractValidator<ISiteConfiguration>
{
    public SiteUninstallValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.AcumaticaToolPath).NotNull();
        RuleFor(c => c.InstanceName).NotNull();
    }
}