using ACUCustomizationUtils.Configuration.Erp;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Erp;

internal class ErpUninstallValidator : AbstractValidator<IErpConfiguration>
{
    public ErpUninstallValidator()
    {
        RuleFor(c => c.IsNotNull).Equal(true).WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.Version).NotNull();
        RuleFor(c => c.DestinationDirectory).NotNull();
        RuleFor(c => c.InstallationDirectory).NotNull()
            .Must(d => new DirectoryInfo(d!).Exists)
            .WithMessage("Installation directory {PropertyValue} is not found!");
    }
}