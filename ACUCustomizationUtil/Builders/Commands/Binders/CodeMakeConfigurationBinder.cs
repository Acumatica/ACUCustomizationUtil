using System.CommandLine;
using System.CommandLine.Binding;
using ACUCustomizationUtils.Builders.Commands.Common;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.Code;
using ACUCustomizationUtils.Configuration.Package;

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
        var makeQA = bindingContext.ParseResult.GetValueForOption(commandOptions[5]!);
        var makeISV = bindingContext.ParseResult.GetValueForOption(commandOptions[6]!);

        return new AcuConfiguration
        {
            Package = new PackageConfiguration
            {
                PackageName = packageName,
                PackageDirectory = packageDirectory
            },

            Code = new CodeConfiguration
            {
                PkgSourceDirectory = sourceDirectory,
                PkgDescription = projectDescription,
                PkgLevel = projectLevel,
                MakeQA = makeQA != null ? bool.Parse(makeQA) : null,
                MakeISV = makeISV != null ? bool.Parse(makeISV) : null
            }
        };
    }
}