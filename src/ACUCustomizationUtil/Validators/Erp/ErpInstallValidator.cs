using System.Diagnostics;
using ACUCustomizationUtils.Configuration.Erp;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Erp;

public class ErpInstallValidator : AbstractValidator<IErpConfiguration>
{
    public ErpInstallValidator()
    {
        RuleFor(c => c.IsNotNull)
            .Equal(true)
            .WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.ErpVersion).NotNull();
        RuleFor(c => c.DestinationDirectory)
            .NotNull()
            .Must(d => new DirectoryInfo(d!).Exists)
            .WithMessage("Destination directory {PropertyValue} is not found!");
        RuleFor(c => c.InstallationFileName).NotNull();
        RuleFor(c => c.InstallationFilePath)
            .NotNull()
            .Must(f => new FileInfo(f!).Exists)
            .WithMessage("Installation file {PropertyValue} is not found");
        // RuleFor(c => c)
        //     .Must(c => c.ErpVersion == FileVersionInfo.GetVersionInfo(c.InstallationFilePath!).FileVersion)
        //     .WithMessage("Installation file version is not as configured ERP version");
    }
}