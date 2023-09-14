using System.CommandLine;
using System.CommandLine.Binding;
using ACUCustomizationUtils.Builders.Commands.Common;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Configuration.Src;

namespace ACUCustomizationUtils.Builders.Commands.Binders;

public class CodeMakeConfigurationBinder : CommandParametersBinder
{
    public CodeMakeConfigurationBinder(Option<FileInfo> configFile, Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions) : base(configFile, userConfigFile, commandOptions)
    {
    }

    protected override IAcuConfiguration GetUserConfiguration(BindingContext bindingContext,
        Option<string>?[] commandOptions)
    {
        var packageName = bindingContext.ParseResult.GetValueForOption(commandOptions[0]!);
        var sourceDirectory = bindingContext.ParseResult.GetValueForOption(commandOptions[1]!);
        var packageDirectory = bindingContext.ParseResult.GetValueForOption(commandOptions[2]!);
        var projectDescription = bindingContext.ParseResult.GetValueForOption(commandOptions[3]!);
        var projectLevel = bindingContext.ParseResult.GetValueForOption(commandOptions[4]!);
        var makeMode = bindingContext.ParseResult.GetValueForOption(commandOptions[5]!);

        return new AcuConfiguration
        {
            Pkg = new PackageConfiguration
            {
                PkgName = packageName,
                PkgDirectory = packageDirectory
            },

            Src = new SrcConfiguration
            {
                PkgSourceDirectory = sourceDirectory,
                PkgDescription = projectDescription,
                PkgLevel = projectLevel,
                MakeMode = makeMode
            }
        };
    }
}