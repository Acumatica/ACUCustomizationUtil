using System.Diagnostics;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Configuration.Site;
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
        RuleFor(c => c.SqlServerName).NotNull();
        RuleFor(c => c.DbConnectionString).NotNull();
        RuleFor(c => c.DbName).NotNull();
        RuleFor(c => c.IisAppPool).NotNull();
        RuleFor(c => c.IisWebSite).NotNull();
        
    }
}

internal class SiteInstallValidatorV : AbstractValidator<IAcuConfiguration>
{
    public SiteInstallValidatorV()
    {
        RuleFor(c => c)
            .Must(c => c.Erp.ErpVersion == FileVersionInfo.GetVersionInfo(c.Site.AcumaticaToolPath!).FileVersion)
            .WithMessage("Acumatica tool file version is not equal configured ERP version");
    }
}