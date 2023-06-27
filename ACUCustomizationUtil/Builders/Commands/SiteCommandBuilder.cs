using System.CommandLine;
using System.CommandLine.Binding;
using ACUCustomizationUtils.Builders.Commands.Common;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.Site;
using ACUCustomizationUtils.Services;

namespace ACUCustomizationUtils.Builders.Commands;
/// <summary>
/// This class is the point of building an application commands routing (Site subcommand)
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public class SiteCommandBuilder : CommandBuilderBase
{
    private readonly ISiteService _siteService;
    private Option<string>? _acuToolPathOption;

    public SiteCommandBuilder(ISiteService siteService)
    {
        _siteService = siteService;
    }

    public override Command BuildCommand()
    {
        _acuToolPathOption = GetAcuToolPathOption();

        var installCommand = BuildInstallCommand();
        var deleteCommand = BuildDeleteCommand();
        var updateCommand = BuildUpdateCommand();

        var siteCommand = new Command("site", "Work with a Acumatica instance.")
        {
            installCommand,
            updateCommand,
            deleteCommand
        };

        siteCommand.AddGlobalOption(_acuToolPathOption!);

        return siteCommand;
    }

    private Command BuildInstallCommand()
    {
        var serverNameOption = GetServerNameOption();
        var databaseNameOption = GetDatabaseNameOption();
        var instanceNameOption = GetInstanceOption();
        var instancePathOption = GetInstancePathOption();
        var acuAdminName = GetAcumaticaAdminNameOption();
        var acuAdminPassword = GetAcumaticaAdminPasswordOption();

        var command = new Command("install", "Install Acumatica instance.")
        {
            serverNameOption,
            databaseNameOption,
            instanceNameOption,
            instancePathOption,
            acuAdminName,
            acuAdminPassword
        };
        command.SetHandler(async config => { await _siteService.InstallSite(config); },
            new SiteInstallConfigurationBinder(ConfigOption!, UserConfigOption!, _acuToolPathOption,
                serverNameOption,
                databaseNameOption,
                instanceNameOption,
                instancePathOption,
                acuAdminName,
                acuAdminPassword));

        return command;
    }


    private Command BuildDeleteCommand()
    {
        var instanceNameOption = GetInstanceOption();
        var command = new Command("delete", "Delete Acumatica instance.") { instanceNameOption };
        command.SetHandler(async config => { await _siteService.DeleteSite(config); },
            new SiteDeleteConfigurationBinder(ConfigOption!, UserConfigOption!, _acuToolPathOption,
                instanceNameOption));

        return command;
    }

    private Command BuildUpdateCommand()
    {
        var updateInstanceCommand = BuildUpdateInstanceCommand();
        var updateDatabaseCommand = BuildUpdateDatabaseCommand();
        var command = new Command("update", "Update Acumatica instance.")
        {
            updateInstanceCommand, updateDatabaseCommand
        };

        return command;
    }

    private Command BuildUpdateDatabaseCommand()
    {
        var serverNameOption = GetServerNameOption();
        var databaseNameOption = GetDatabaseNameOption();
        var command = new Command("database", "Update Acumatica database.")
        {
            serverNameOption, databaseNameOption
        };
        command.SetHandler(async config => { await _siteService.UpdateDatabase(config); },
            new SiteUpdateDatabaseConfigurationBinder(ConfigOption!, UserConfigOption!, _acuToolPathOption,
                serverNameOption,
                databaseNameOption));

        return command;
    }

    private Command BuildUpdateInstanceCommand()
    {
        var instanceNameOption = GetInstanceOption();

        var command = new Command("instance", "Update Acumatica instance.")
        {
            instanceNameOption
        };

        command.SetHandler(async config => { await _siteService.UpdateSite(config); },
            new SiteUpdateInstanceConfigurationBinder(ConfigOption!, UserConfigOption!, _acuToolPathOption,
                instanceNameOption));

        return command;
    }

    private static Option<string> GetAcuToolPathOption()
    {
        return new Option<string>(name: "--acuToolPath", description: "Acumatica ac.exe tool path");
    }

    private static Option<string> GetDatabaseNameOption()
    {
        return new Option<string>("--dbName", "Acumatica database name");
    }

    private static Option<string> GetAcumaticaAdminNameOption()
    {
        return new Option<string>("--acuAdminName", "Acumatica instance admin name");
    }

    private static Option<string> GetAcumaticaAdminPasswordOption()
    {
        return new Option<string>("--acuAdminPassword", "Acumatica instance admin password");
    }

    private static Option<string> GetServerNameOption()
    {
        return new Option<string>("--sqlServerName",
            description: "SQL Server instance for Acumatica database",
            getDefaultValue: () => "localhost");
    }

    private static Option<string> GetInstanceOption()
    {
        return new Option<string>("--instanceName", "Acumatica instance name");
    }

    private static Option<string> GetInstancePathOption()
    {
        return new Option<string>("--instancePath", "Acumatica instance path");
    }
}

public class SiteInstallConfigurationBinder : CommandParametersBinder
{
    public SiteInstallConfigurationBinder(Option<FileInfo> configFile, Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions)
        : base(configFile, userConfigFile, commandOptions)
    {
    }

    protected override IAcuConfiguration GetUserConfiguration(BindingContext bindingContext,
        Option<string>?[] commandOptions)
    {
        var acuToolPath = bindingContext.ParseResult.GetValueForOption(commandOptions[0]!);
        var serverName = bindingContext.ParseResult.GetValueForOption(commandOptions[1]!);
        var databaseName = bindingContext.ParseResult.GetValueForOption(commandOptions[2]!);
        var instanceName = bindingContext.ParseResult.GetValueForOption(commandOptions[3]!);
        var instancePath = bindingContext.ParseResult.GetValueForOption(commandOptions[4]!);
        var acuAdminName = bindingContext.ParseResult.GetValueForOption(commandOptions[5]!);
        var acuAdminPassword = bindingContext.ParseResult.GetValueForOption(commandOptions[6]!);

        return new AcuConfiguration
        {
            Site = new SiteConfiguration
            {
                AcumaticaToolPath = acuToolPath,
                SqlServerName = serverName,
                DbName = databaseName,
                InstanceName = instanceName,
                InstancePath = instancePath,
                AcumaticaAdminName = acuAdminName,
                AcumaticaAdminPassword = acuAdminPassword
            }
        };
    }
}

public class SiteDeleteConfigurationBinder : CommandParametersBinder
{
    public SiteDeleteConfigurationBinder(Option<FileInfo> configFile, Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions)
        : base(configFile, userConfigFile, commandOptions)
    {
    }

    protected override IAcuConfiguration GetUserConfiguration(BindingContext bindingContext,
        Option<string>?[] commandOptions)
    {
        var acuToolPath =
            bindingContext.ParseResult.GetValueForOption(commandOptions[0] ?? throw new InvalidOperationException());
        var instanceName =
            bindingContext.ParseResult.GetValueForOption(commandOptions[1] ?? throw new InvalidOperationException());
        return new AcuConfiguration
        {
            Site = new SiteConfiguration
            {
                AcumaticaToolPath = acuToolPath,
                InstanceName = instanceName
            }
        };
    }
}

public class SiteUpdateDatabaseConfigurationBinder : CommandParametersBinder
{
    public SiteUpdateDatabaseConfigurationBinder(Option<FileInfo> configFile, Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions) : base(configFile, userConfigFile, commandOptions)
    {
    }

    protected override IAcuConfiguration GetUserConfiguration(BindingContext bindingContext,
        Option<string>?[] commandOptions)
    {
        var acuToolPath =
            bindingContext.ParseResult.GetValueForOption(commandOptions[0] ?? throw new InvalidOperationException());
        var sqlServerName =
            bindingContext.ParseResult.GetValueForOption(commandOptions[1] ?? throw new InvalidOperationException());
        var dbName =
            bindingContext.ParseResult.GetValueForOption(commandOptions[2] ?? throw new InvalidOperationException());
        return new AcuConfiguration
        {
            Site = new SiteConfiguration
            {
                AcumaticaToolPath = acuToolPath,
                SqlServerName = sqlServerName,
                DbName = dbName
            }
        };
    }
}

public class SiteUpdateInstanceConfigurationBinder : CommandParametersBinder
{
    public SiteUpdateInstanceConfigurationBinder(Option<FileInfo> configFile, Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions)
        : base(configFile, userConfigFile, commandOptions)
    {
    }

    protected override IAcuConfiguration GetUserConfiguration(BindingContext bindingContext,
        Option<string>?[] commandOptions)
    {
        var acuToolPath =
            bindingContext.ParseResult.GetValueForOption(commandOptions[0] ?? throw new InvalidOperationException());
        var instanceName =
            bindingContext.ParseResult.GetValueForOption(commandOptions[1] ?? throw new InvalidOperationException());
        return new AcuConfiguration
        {
            Site = new SiteConfiguration
            {
                AcumaticaToolPath = acuToolPath,
                InstanceName = instanceName
            }
        };
    }
}