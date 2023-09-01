using System.CommandLine;
using ACUCustomizationUtils.Builders.Commands.Common;
using Microsoft.Extensions.DependencyInjection;

namespace ACUCustomizationUtils.Builders.Commands;
/// <summary>
/// This class is the point of building an application commands routing
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public class RootCommandBuilder
{
    private RootCommand? _rootCommand;
    private Option<FileInfo?> _configOption;
    private Option<FileInfo?> _userConfigOption;


    private readonly ICommandBuilder _erpCommandBuilder;
    private readonly ICommandBuilder _siteCommandBuilder;
    private readonly ICommandBuilder _packageCommandBuilder;
    private readonly ICommandBuilder _codeCommandBuilder;

    public RootCommandBuilder(IServiceProvider serviceProvider)
    {
        _erpCommandBuilder = serviceProvider.GetRequiredService<ErpCommandBuilder>();
        _siteCommandBuilder = serviceProvider.GetRequiredService<SiteCommandBuilder>();
        _packageCommandBuilder = serviceProvider.GetRequiredService<PackageCommandBuilder>();
        _codeCommandBuilder = serviceProvider.GetRequiredService<CodeCommandBuilder>();

        _configOption = BuildConfigOption();
        _userConfigOption = BuildUserConfigOption();
    }

    public Command BuildCommand()
    {
        BuildRootCommand()
            .BuildErpCommand()
            .BuildSiteCommand()
            .BuildProjectCommand()
            .BuildPackageCommand();

        return _rootCommand!;
    }

    private RootCommandBuilder BuildRootCommand()
    {
        _rootCommand = new RootCommand("Acumatica customization util - tool for work with customization");

        _configOption = BuildConfigOption();
        _userConfigOption = BuildUserConfigOption();

        _rootCommand.AddGlobalOption(_configOption);
        _rootCommand.AddGlobalOption(_userConfigOption);
        
        return this;
    }

    private RootCommandBuilder BuildErpCommand()
    {
        var erpCommand = _erpCommandBuilder.SetGlobalOptions(_configOption, _userConfigOption)
            .BuildCommand();
        _rootCommand?.AddCommand(erpCommand);
        return this;
    }

    private RootCommandBuilder BuildSiteCommand()
    {
        var siteCommand = _siteCommandBuilder.SetGlobalOptions(_configOption, _userConfigOption)
            .BuildCommand();
        _rootCommand?.AddCommand(siteCommand);
        return this;
    }

    private RootCommandBuilder BuildProjectCommand()
    {
        var projectCommand = _codeCommandBuilder.SetGlobalOptions(_configOption, _userConfigOption)
            .BuildCommand();
        _rootCommand?.AddCommand(projectCommand);
        return this;
    }

    private RootCommandBuilder BuildPackageCommand()
    {
        var pkgCommand = _packageCommandBuilder.SetGlobalOptions(_configOption, _userConfigOption)
            .BuildCommand();
        _rootCommand?.AddCommand(pkgCommand);
        return this;
    }

    private static Option<FileInfo?> BuildConfigOption()
    {
        var configOption = new Option<FileInfo?>(
            "--config",
            description: "An option of path to configuration file",
            isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0) return new FileInfo("acu.json");
                var filePath = result.Tokens.Single().Value;
                return !File.Exists(filePath) ? null : new FileInfo(filePath);
            });

        return configOption;
    }

    private static Option<FileInfo?> BuildUserConfigOption()
    {
        var userConfigOption = new Option<FileInfo?>(
            "--user-config",
            description: "An option of path to user configuration file",
            isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0) return new FileInfo("acu.json.user");
                var filePath = result.Tokens.Single().Value;
                return !File.Exists(filePath) ? null : new FileInfo(filePath);
            });

        return userConfigOption;
    }
}