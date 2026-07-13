using System.CommandLine;
using System.CommandLine.Binding;

using ACUCustomizationUtils.Builders.Commands.Common;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Configuration.Site;
using ACUCustomizationUtils.Configuration.Src;

namespace ACUCustomizationUtils.Builders.Commands.Binders;

public class CodeSrcConfigurationBinder : CommandParametersBinder
{
    public CodeSrcConfigurationBinder(
        Option<FileInfo> configFile,
        Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions
    )
        : base(configFile, userConfigFile, commandOptions) { }

    protected override IAcuConfiguration GetUserConfiguration(
        BindingContext bindingContext,
        Option<string>?[] commandOptions
    )
    {
        string? packageName = bindingContext.ParseResult.GetValueForOption(commandOptions[0]!);
        string? dbConnection = bindingContext.ParseResult.GetValueForOption(commandOptions[1]!);
        string? instancePath = bindingContext.ParseResult.GetValueForOption(commandOptions[2]!);
        string? sourceDirectory = bindingContext.ParseResult.GetValueForOption(commandOptions[3]!);
        string? dbProvider = bindingContext.ParseResult.GetValueForOption(commandOptions[4]!);
        string? dbUser = bindingContext.ParseResult.GetValueForOption(commandOptions[5]!);
        string? dbPassword = bindingContext.ParseResult.GetValueForOption(commandOptions[6]!);

        return new AcuConfiguration
        {
            Site = new SiteConfiguration
            {
                DbConnectionString = dbConnection,
                InstancePath = instancePath,
                DbProvider = dbProvider,
                DbUser = dbUser,
                DbPassword = dbPassword,
            },

            Pkg = new PackageConfiguration { PkgName = packageName },

            Src = new SrcConfiguration { PkgSourceDirectory = sourceDirectory },
        };
    }
}
