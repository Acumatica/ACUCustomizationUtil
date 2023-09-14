using ACUCustomizationUtils.Configuration.Src;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Src;

internal class SrcBuildValidator : AbstractValidator<ISrcConfiguration>
{
    public SrcBuildValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.PkgSourceDirectory).NotNull().Must(Directory.Exists);
        RuleFor(c => c.PkgSourceBinDirectory).NotNull();
        RuleFor(c => c.MsBuildTargetDirectory).NotNull();
        RuleFor(c => c.MsBuildAssemblyName).NotNull();
        RuleFor(c => c.MsBuildSolutionFile).NotNull().Must(File.Exists);
    }
}