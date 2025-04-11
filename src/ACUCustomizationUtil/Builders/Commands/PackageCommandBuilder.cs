using System.CommandLine;
using System.CommandLine.Binding;
using ACUCustomizationUtils.Builders.Commands.Common;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Services;
using ACUCustomizationUtils.Services.Package;

namespace ACUCustomizationUtils.Builders.Commands;
/// <summary>
/// This class is the point of building an application commands routing (Package subcommand)
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public class PackageCommandBuilder : CommandBuilderBase
{
    private readonly IPackageService _packageService;
    private readonly Option<string> _urlOption;
    private readonly Option<string> _loginOption;
    private readonly Option<string> _passwordOption;
    private readonly Option<string> _tenantOption;

    public PackageCommandBuilder(IPackageService packageService)
    {
        _packageService = packageService;
        _urlOption = GetUrlOption();
        _loginOption = GetLoginOption();
        _passwordOption = GetPasswordOption();
        _tenantOption = GetTenantOption();
    }

    public override Command BuildCommand()
    {
        var getCommand = BuildGetCommand();
        var publishCommand = BuildPublishCommand();
        var unpublishAllCommand = BuildUnpublishAllCommand();
        var uploadCommand = BuildUploadCommand();

        var packageCommand = new Command("pkg", "Work with a customization package.")
        {
            getCommand,
            publishCommand,
            unpublishAllCommand,
            uploadCommand
        };
        
        packageCommand.AddGlobalOption(_urlOption);
        packageCommand.AddGlobalOption(_loginOption);
        packageCommand.AddGlobalOption(_passwordOption);
        packageCommand.AddGlobalOption(_tenantOption);

        return packageCommand;
    }

    private Command BuildUploadCommand()
    {
        var packageNameOption = GetPackageNameOption();
        var packageDirOption = GetPackageDirectoryOption();
        var tenantOption = GetTenantOption();

        var command = new Command("upload", "Upload package.")
        {
            packageNameOption,
            packageDirOption,
            tenantOption
        };

        command.SetHandler(
            _packageService.UploadPackage,
            new PackageUploadConfigurationBinder(ConfigOption!,
                UserConfigOption!,
                _urlOption,
                _loginOption,
                _passwordOption,
                tenantOption,
                packageNameOption,
                packageDirOption));

        return command;
    }

    private Command BuildUnpublishAllCommand()
    {
        var tenantOption = GetTenantOption();

        var command = new Command("unpublish", "Unpublish all packages.")
        {
            tenantOption
        };
        
        command.SetHandler(
            async config => await _packageService.UnpublishAllPackages(config),
            new PackageUnpublishAllConfigurationBinder(ConfigOption!,
                UserConfigOption!,
                _urlOption,
                _loginOption,
                _passwordOption,
                tenantOption));

        return command;
    }

    private Command BuildPublishCommand()
    {
        var packageNameOption = GetPackageNameOption();
        var tenantOption = GetTenantOption();

        var command = new Command("publish", "Publish package(s).")
        {
            packageNameOption,
            tenantOption
        };

        command.SetHandler(
            async config => await _packageService.PublishPackages(config),
            new PackagePublishConfigurationBinder(ConfigOption!,
                UserConfigOption!,
                _urlOption,
                _loginOption,
                _passwordOption,
                tenantOption,
                packageNameOption));

        return command;
    }

    private Command BuildGetCommand()
    {
        var packageNameOption = GetPackageNameOption();
        var packageDirOption = GetPackageDirectoryOption();
        var tenantOption = GetTenantOption();

        var command = new Command("get", "Get package content.")
        {
            packageNameOption,
            packageDirOption,
            tenantOption
        };

        command.SetHandler(
            async config => await _packageService.GetPackage(config),
            new PackageUploadConfigurationBinder(ConfigOption!,
                UserConfigOption!,
                _urlOption,
                _loginOption,
                _passwordOption,
                tenantOption,
                packageNameOption,
                packageDirOption));

        return command;
    }

    private static Option<string> GetUrlOption()
    {
        return new Option<string>("--url", "Acumatica instance url");
    }

    private static Option<string> GetLoginOption()
    {
        return new Option<string>("--login", "User login");
    }

    private static Option<string> GetPasswordOption()
    {
        return new Option<string>("--password", "User password");
    }

    private static Option<string> GetTenantOption()
    {
        return new Option<string>("--tenant", "Tenant to login");
    }

    private static Option<string> GetPackageNameOption()
    {
        return new Option<string>("--pkgName", "Package name");
    }

    private static Option<string> GetPackageDirectoryOption()
    {
        return new Option<string>("--pkgDir", "Package directory");
    }
}

public class PackageUploadConfigurationBinder : CommandParametersBinder
{
    public PackageUploadConfigurationBinder(Option<FileInfo> configFile, Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions) : base(configFile, userConfigFile, commandOptions)
    {
    }

    protected override IAcuConfiguration GetUserConfiguration(BindingContext bindingContext,
        Option<string>?[] commandOptions)
    {
        var url = bindingContext.ParseResult.GetValueForOption(commandOptions[0]!);
        var login = bindingContext.ParseResult.GetValueForOption(commandOptions[1]!);
        var password = bindingContext.ParseResult.GetValueForOption(commandOptions[2]!);
        var tenant = bindingContext.ParseResult.GetValueForOption(commandOptions[3]!);
        var pkgName = bindingContext.ParseResult.GetValueForOption(commandOptions[4]!);
        var pkgDir = bindingContext.ParseResult.GetValueForOption(commandOptions[5]!);

        return new AcuConfiguration
        {
            Pkg = new PackageConfiguration
            {
                Url = url != null ? new Uri(url) : null,
                Login = login, Password = password, Tenant = tenant, PkgName = pkgName, PkgDirectory = pkgDir
            }
        };
    }
}

public class PackagePublishConfigurationBinder : CommandParametersBinder
{
    public PackagePublishConfigurationBinder(Option<FileInfo> configFile, Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions) : base(configFile, userConfigFile, commandOptions)
    {
    }

    protected override IAcuConfiguration GetUserConfiguration(BindingContext bindingContext,
        Option<string>?[] commandOptions)
    {
        var url = bindingContext.ParseResult.GetValueForOption(commandOptions[0]!);
        var login = bindingContext.ParseResult.GetValueForOption(commandOptions[1]!);
        var password = bindingContext.ParseResult.GetValueForOption(commandOptions[2]!);
        var tenant = bindingContext.ParseResult.GetValueForOption(commandOptions[3]!);
        var pkgName = bindingContext.ParseResult.GetValueForOption(commandOptions[4]!);

        return new AcuConfiguration
        {
            Pkg = new PackageConfiguration
            {
                Url = url != null ? new Uri(url) : null,
                Login = login, Password = password, Tenant = tenant, PkgName = pkgName
            }
        };
    }
}

public class PackageUnpublishAllConfigurationBinder : CommandParametersBinder
{
    public PackageUnpublishAllConfigurationBinder(Option<FileInfo> configFile, Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions) : base(configFile, userConfigFile, commandOptions)
    {
    }

    protected override IAcuConfiguration GetUserConfiguration(BindingContext bindingContext,
        Option<string>?[] commandOptions)
    {
        var url = bindingContext.ParseResult.GetValueForOption(commandOptions[0]!);
        var login = bindingContext.ParseResult.GetValueForOption(commandOptions[1]!);
        var password = bindingContext.ParseResult.GetValueForOption(commandOptions[2]!);
        var tenant = bindingContext.ParseResult.GetValueForOption(commandOptions[3]!);

        return new AcuConfiguration
        {
            Pkg = new PackageConfiguration
            {
                Url = url != null ? new Uri(url) : null,
                Login = login, Password = password, Tenant = tenant
            }
        };
    }
}