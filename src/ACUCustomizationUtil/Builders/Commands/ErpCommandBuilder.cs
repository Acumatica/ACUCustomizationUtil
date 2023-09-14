using System.CommandLine;
using System.CommandLine.Binding;
using ACUCustomizationUtils.Builders.Commands.Common;
using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Configuration.Erp;
using ACUCustomizationUtils.Services;
using ACUCustomizationUtils.Services.ERP;

namespace ACUCustomizationUtils.Builders.Commands;
/// <summary>
/// This class is the point of building an application commands routing (ERP subcommand)
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public class ErpCommandBuilder : CommandBuilderBase
{
    private readonly IErpService _erpService;

    public ErpCommandBuilder(IErpService erpService)
    {
        _erpService = erpService;
    }

    public override Command BuildCommand()
    {
        var downloadCommand = BuildDownloadCommand();
        var installCommand = BuildInstallCommand();
        var deleteCommand = BuildDeleteCommand();
        var erpCommand = new Command("erp", "Work with a Acumatica ERP.")
        {
            downloadCommand,
            installCommand,
            deleteCommand
        };

        return erpCommand;
    }

    private Command BuildInstallCommand()
    {
        var version = GetVersionOption();
        var directory = GetDestinationDirectoryOption();
        var file = GetFileNameOption();
        var command = new Command("install", "Install ERP.")
        {
            version,
            directory,
            file
        };

        command.SetHandler(
            async config => { await _erpService.InstallErp(config); },
            new ErpInstallConfigurationBinder(ConfigOption!, UserConfigOption!, version, directory, file));

        return command;
    }

    private Command BuildDeleteCommand()
    {
        var version = GetVersionOption();
        var directory = GetDestinationDirectoryOption();
        var file = GetFileNameOption();
        var command = new Command("delete", "Delete ERP.")
        {
            version,
            directory,
            file
        };

        command.SetHandler(
            async config => { await _erpService.UninstallErp(config); },
            new ErpDeleteConfigurationBinder(ConfigOption!, UserConfigOption!, version, directory, file));

        return command;
    }

    private Command BuildDownloadCommand()
    {
        var version = GetVersionOption();
        var url = GetUrlOption();
        var directory = GetDestinationDirectoryOption();
        var file = GetFileNameOption();
        var command = new Command("download", "Download ERP installation.")
        {
            version,
            directory,
            file,
            url
        };

        command.SetHandler(
            async config => { await _erpService.DownloadErp(config); },
            new ErpDownloadConfigurationBinder(ConfigOption!, UserConfigOption!, version, directory, file, url));

        return command;
    }

    private static Option<string> GetFileNameOption()
    {
        return new Option<string>(
            name: "--installerName",
            description: "Name of ERP installer file",
            getDefaultValue: () => Messages.AcumaticaErpInstallMsi);
    }

    private static Option<string> GetUrlOption()
    {
        return new Option<string>(
            name: "--url",
            description: "ERP installer download url");
    }

    private static Option<string> GetDestinationDirectoryOption()
    {
        return new Option<string>(
            name: "--destinationDirectory",
            description: "Base directory for install ERP");
    }

    private static Option<string> GetVersionOption()
    {
        return new Option<string>(
            name: "--erpVersion",
            description: "ERP version");
    }
}

public class ErpInstallConfigurationBinder : CommandParametersBinder
{
    public ErpInstallConfigurationBinder(Option<FileInfo> configFile, Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions)
        : base(configFile, userConfigFile, commandOptions)
    {
    }

    protected override IAcuConfiguration GetUserConfiguration(BindingContext bindingContext,
        Option<string>?[] commandOptions)
    {
        var erpVersion = bindingContext.ParseResult.GetValueForOption(commandOptions[0]!);
        var destinationDirectory = bindingContext.ParseResult.GetValueForOption(commandOptions[1]!);
        var installationFileName = bindingContext.ParseResult.GetValueForOption(commandOptions[2]!);

        return new AcuConfiguration
        {
            Erp = new ErpConfiguration
            {
                ErpVersion = erpVersion,
                DestinationDirectory = destinationDirectory,
                InstallationFileName = installationFileName
            }
        };
    }
}

public class ErpDownloadConfigurationBinder : CommandParametersBinder
{
    public ErpDownloadConfigurationBinder(Option<FileInfo> configFile, Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions)
        : base(configFile, userConfigFile, commandOptions)
    {
    }

    protected override IAcuConfiguration GetUserConfiguration(BindingContext bindingContext,
        Option<string>?[] commandOptions)
    {
        var erpVersion = bindingContext.ParseResult.GetValueForOption(commandOptions[0]!);
        var destinationDirectory = bindingContext.ParseResult.GetValueForOption(commandOptions[1]!);
        var installationFileName = bindingContext.ParseResult.GetValueForOption(commandOptions[2]!);
        var url = bindingContext.ParseResult.GetValueForOption(commandOptions[3]!);

        return new AcuConfiguration()
        {
            Erp = new ErpConfiguration
            {
                ErpVersion = erpVersion,
                Url = !string.IsNullOrWhiteSpace(url) ? new Uri(url) : null,
                DestinationDirectory = destinationDirectory,
                InstallationFileName = installationFileName
            }
        };
    }
}

public class ErpDeleteConfigurationBinder : CommandParametersBinder
{
    public ErpDeleteConfigurationBinder(Option<FileInfo> configFile, Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions)
        : base(configFile, userConfigFile, commandOptions)
    {
    }

    protected override IAcuConfiguration GetUserConfiguration(BindingContext bindingContext,
        Option<string>?[] commandOptions)
    {
        var erpVersion = bindingContext.ParseResult.GetValueForOption(commandOptions[0]!);
        var destinationDirectory = bindingContext.ParseResult.GetValueForOption(commandOptions[1]!);
        var installationFileName = bindingContext.ParseResult.GetValueForOption(commandOptions[2]!);

        return new AcuConfiguration
        {
            Erp = new ErpConfiguration
            {
                ErpVersion = erpVersion,
                DestinationDirectory = destinationDirectory,
                InstallationFileName = installationFileName
            }
        };
    }
}