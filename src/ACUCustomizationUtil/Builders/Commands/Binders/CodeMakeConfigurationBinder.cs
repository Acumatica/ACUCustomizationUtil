using System.CommandLine;
using System.CommandLine.Binding;

using ACUCustomizationUtils.Builders.Commands.Common;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Configuration.Src;

namespace ACUCustomizationUtils.Builders.Commands.Binders;

public class CodeMakeConfigurationBinder(
    Option<FileInfo> configFile,
    Option<FileInfo> userConfigFile,
    params Option<string>?[] commandOptions
) : CommandParametersBinder(configFile, userConfigFile, commandOptions)
{
    protected override IAcuConfiguration GetUserConfiguration(
        BindingContext bindingContext,
        Option<string>?[] commandOptions
    )
    {
        string? packageName = bindingContext.ParseResult.GetValueForOption(commandOptions[0]!);
        string? sourceDirectory = bindingContext.ParseResult.GetValueForOption(commandOptions[1]!);
        string? packageDirectory = bindingContext.ParseResult.GetValueForOption(commandOptions[2]!);
        string? projectDescription = bindingContext.ParseResult.GetValueForOption(commandOptions[3]!);
        string? projectLevel = bindingContext.ParseResult.GetValueForOption(commandOptions[4]!);
        string? makeMode = bindingContext.ParseResult.GetValueForOption(commandOptions[5]!);
        string? packageSuffix = bindingContext.ParseResult.GetValueForOption(commandOptions[6]!);

        return new AcuConfiguration
        {
            Pkg = new PackageConfiguration
            {
                PkgName = packageName,
                PkgSuffix = packageSuffix,
                PkgDirectory = packageDirectory,
            },

            Src = new SrcConfiguration
            {
                PkgSourceDirectory = sourceDirectory,
                PkgDescription = projectDescription,
                PkgLevel = projectLevel,
                MakeMode = makeMode,
            },
        };
    }
}
