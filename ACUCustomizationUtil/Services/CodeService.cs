using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Extensions;
using ACUCustomizationUtils.Helpers;
using ACUCustomizationUtils.Helpers.Proxy;
using ACUCustomizationUtils.Validators.Code;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using CstEntityHelper = ACUCustomizationUtils.Helpers.CstEntityHelper;

namespace ACUCustomizationUtils.Services;
/// <summary>
/// This class contains methods for handle Code subcommands
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public class CodeService : ICodeService
{
    private readonly ILogger<CodeService> _logger;
    private readonly IPackageHelperProxy _packageHelperProxy;

    #region Public methods

    public CodeService(ILogger<CodeService> logger, IPackageHelperProxy packageHelperProxy)
    {
        _logger = logger;
        _packageHelperProxy = packageHelperProxy;
    }

    public async Task GetProjectSource(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute GetProjectSource action");
        try
        {
            await AnsiConsole.Status().StartAsync("Download project source items", async ctx =>
            {
                ctx.Status("Reading configuration ...");
                _logger.LogInformation("Reading configuration");
                ConfigurationHelper.PrintConfiguration(config, _logger,  nameof(IAcuConfiguration.Package), nameof(IAcuConfiguration.Code));

                ctx.Status("Validate configuration ...");
                _logger.LogInformation("Validate configuration");
                CodeValidator.ValidateForSrc(config);

                ctx.Status("Download in progress, please wait ...");
                _logger.LogInformation("Download source items for project {Package}", config.Package.PackageName);
                await GetProjectSourceExAsync(config);
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetProjectSource: action error!");
        }

        _logger.LogInformation("GetProjectSource action complete");
    }

    public async Task MakeProjectFromSource(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute MakeProjectFromSource action");
        try
        {
            await AnsiConsole.Status().StartAsync("Making project from source", async ctx =>
            {
                ctx.Status("Reading configuration ...");
                _logger.LogInformation("Reading configuration");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Package), nameof(IAcuConfiguration.Code));

                ctx.Status("Validate configuration ...");
                _logger.LogInformation("Validate configuration");
                CodeValidator.ValidateForMake(config);

                ctx.Status("Making in progress, please wait ...");
                _logger.LogInformation("Making package for project {Package}", config.Package.PackageName);
                await MakeProjectFromSourceExAsync(config);
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "MakeProjectFromSource: action error!");
        }

        _logger.LogInformation("MakeProjectFromSource action complete");
    }

    public async Task CompileSolution(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute CompileSolution action");
        try
        {
            await AnsiConsole.Status().StartAsync("Compile external library code", async ctx =>
            {
                ctx.Status("Reading configuration ...");
                _logger.LogInformation("Reading configuration");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Package), nameof(IAcuConfiguration.Code));

                ctx.Status("Validate configuration ...");
                _logger.LogInformation("Validate configuration");
                CodeValidator.ValidateForCompile(config.Code);

                ctx.Status("Compile in progress, please wait ...");
                _logger.LogInformation("Compile external library code for project {Package}",
                    config.Package.PackageName);
                var msBuildHelper = new MsBuildHelper(config, ctx);
                await msBuildHelper.Execute();
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CompileSolution: action error!");
        }

        _logger.LogInformation("CompileSolution action complete");
    }

    #endregion

    #region Private methods

    private static async Task GetProjectSourceExAsync(IAcuConfiguration config)
    {
        var packageName = config.Package.PackageName!;
        var sourceDirectory = config.Code.PkgSourceDirectory!;
        var connectionString = config.Site.DbConnectionString!;

        sourceDirectory.TryCheckCreateDirectory();
        if (!Directory.Exists(sourceDirectory))
        {
            throw new Exception($"Directory {sourceDirectory} is not exist or access is not allowed to it");
        }

        var dataHelper = new DatabaseHelper(config);
        var itemHandler = new CstEntityHelper(config);

        var projectInfo = await dataHelper.GetCustomizationProjectAsync(packageName) ??
                          throw new Exception(
                              $"Project {packageName} does not found for connection {connectionString}");

        //Clear directory
        itemHandler.ClearProjectDirectory();

        //Write Customization entities
        var result = await dataHelper.GetCustomizationProjectEntitiesAsync(packageName);
        foreach (var item in result)
        {
            itemHandler.HandleCustomizationsEntity(item);
        }

        //itemHandler.SaveHandledDlls();

        //Write project meta-file
        itemHandler.SaveProjectMetadata(projectInfo);
    }

    private  async Task MakeProjectFromSourceExAsync(IAcuConfiguration config)
    {
        await Task.Run(() =>
        {
            _packageHelperProxy.MakePackage(config);
        });
    }

    #endregion
}