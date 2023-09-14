using System.CommandLine;
using System.CommandLine.Binding;
using ACUCustomizationUtils.Builders.Commands.Common;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Configuration.Site;
using ACUCustomizationUtils.Configuration.Src;

namespace ACUCustomizationUtils.Builders.Commands.Binders;

public class CodeSrcConfigurationBinder : CommandParametersBinder
{
    public CodeSrcConfigurationBinder(Option<FileInfo> configFile, Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions) : base(configFile, userConfigFile, commandOptions)
    {
    }

    protected override IAcuConfiguration GetUserConfiguration(BindingContext bindingContext,
        Option<string>?[] commandOptions)
    {
        var packageName = bindingContext.ParseResult.GetValueForOption(commandOptions[0]!);
        var dbConnection = bindingContext.ParseResult.GetValueForOption(commandOptions[1]!);
        var instancePath = bindingContext.ParseResult.GetValueForOption(commandOptions[2]!);
        var sourceDirectory = bindingContext.ParseResult.GetValueForOption(commandOptions[3]!);

        return new AcuConfiguration
        {
            Site = new SiteConfiguration
            {
                DbConnectionString = dbConnection,
                InstancePath = instancePath
            },

            Pkg = new PackageConfiguration
            {
                PkgName = packageName
            },

            Src = new SrcConfiguration
            {
                PkgSourceDirectory = sourceDirectory
            }
        };
    }
}