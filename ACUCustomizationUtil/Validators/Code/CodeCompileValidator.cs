using ACUCustomizationUtils.Configuration.Code;
using FluentValidation;

namespace ACUCustomizationUtils.Validators.Code;

internal class CodeCompileValidator : AbstractValidator<ICodeConfiguration>
{
    public CodeCompileValidator()
    {
        RuleFor(c => c).NotNull().WithMessage("Configuration should not be null-configuration!");
        RuleFor(c => c.PkgSourceDirectory).NotNull().Must(Directory.Exists);
        RuleFor(c => c.PkgSourceBinDirectory).NotNull();//.Must(Directory.Exists);
        RuleFor(c => c.MsBuildTargetDirectory).NotNull(); //.Must(Directory.Exists);
        RuleFor(c => c.MsBuildSolutionFile).NotNull().Must(File.Exists);
    }
}