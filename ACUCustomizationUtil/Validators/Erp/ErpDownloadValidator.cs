using ACUCustomizationUtils.Configuration.Erp;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Erp;

public class ErpDownloadValidator : AbstractValidator<IErpConfiguration>
{
    public ErpDownloadValidator()
    {
        RuleFor(c => c.IsNotNull).Equal(true).WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.Version).NotNull();
        RuleFor(c => c.Url).NotNull();
        RuleFor(c => c.DestinationDirectory).NotNull();
        RuleFor(c => c.InstallationFileName).NotNull();
    }
}