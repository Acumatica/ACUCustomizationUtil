using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Extensions;
using ACUCustomizationUtils.Helpers;
using ACUCustomizationUtils.Validators.Src;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using CstEntityHelper = ACUCustomizationUtils.Helpers.CstEntityHelper;

namespace ACUCustomizationUtils.Services.Src;

/// <summary>
/// This class contains methods for handle Code subcommands
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public class SrcService : ISrcService
{
    private readonly ILogger<SrcService> _logger;

    #region Public methods

    public SrcService(ILogger<SrcService> logger)
    {
        _logger = logger;
    }

    public async Task GetProjectSource(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute GetProjectSource action");
        try
        {
            await AnsiConsole.Status().StartAsync("Download project source items", async ctx =>
            {
                _logger.LogInformation("Reading configuration");
                ctx.Status("Reading configuration ...");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Pkg),
                    nameof(IAcuConfiguration.Src));

                _logger.LogInformation("Validate configuration");
                ctx.Status("Validate configuration ...");
                SrcValidator.ValidateForSrc(config);

                _logger.LogInformation("Download source items for project {Package}", config.Pkg.PkgName);
                ctx.Status("Download in progress, please wait ...");
                await GetProjectSourceExAsync(config);
            });
            _logger.LogInformation("GetProjectSource action complete");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetProjectSource: action error!");
        }
    }

    public async Task MakeProjectFromSource(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute MakeProjectFromSource action");
        try
        {
            await AnsiConsole.Status().StartAsync("Making project from source", async ctx =>
            {
                _logger.LogInformation("Reading configuration");
                ctx.Status("Reading configuration ...");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Pkg),
                    nameof(IAcuConfiguration.Src));

                _logger.LogInformation("Validate configuration");
                ctx.Status("Validate configuration ...");
                SrcValidator.ValidateForMake(config);

                _logger.LogInformation("Making package for project {Package}", config.Pkg.PkgName);
                ctx.Status("Making in progress, please wait ...");
                await MakeProjectFromSourceExAsync(config);
            });
            _logger.LogInformation("MakeProjectFromSource action complete");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "MakeProjectFromSource: action error!");
        }
    }

    public async Task CompileSolution(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute CompileSolution action");
        try
        {
            await AnsiConsole.Status().StartAsync("Compile external library code", async ctx =>
            {
                _logger.LogInformation("Reading configuration");
                ctx.Status("Reading configuration ...");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Pkg),
                    nameof(IAcuConfiguration.Src));

                _logger.LogInformation("Validate configuration");
                ctx.Status("Validate configuration ...");
                SrcValidator.ValidateForBuild(config);

                _logger.LogInformation("Compile external library code for project {Package}",
                    config.Pkg.PkgName);
                ctx.Status("Compile in progress, please wait ...");
                var msBuildHelper = new MsBuildHelper(config, ctx);
                await msBuildHelper.Execute();

                ctx.Status("Copy external library assembly to package source");
                _logger.LogInformation("Copy external library assembly to package source");
                await msBuildHelper.CopyAssemblyToPackageBinAsync();
            });

            _logger.LogInformation("CompileSolution action success");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CompileSolution action NOT success");
        }
    }

    #endregion

    #region Private methods

    private static async Task GetProjectSourceExAsync(IAcuConfiguration config)
    {
        var packageName = config.Pkg.PkgName!;
        var sourceDirectory = config.Src.PkgSourceDirectory!;
        var connectionString = config.Site.DbConnectionString!;

        sourceDirectory.TryCheckCreateDirectory();
        if (!Directory.Exists(sourceDirectory))
            throw new Exception($"Directory {sourceDirectory} is not exist or access is not allowed to it");

        var dataHelper = new DatabaseHelper(config);
        var itemHandler = new CstEntityHelper(config);

        var projectInfo = await dataHelper.GetCustomizationProjectAsync(packageName) ??
                          throw new Exception(
                              $"Project {packageName} does not found for connection {connectionString}");

        //Clear directory
        itemHandler.ClearProjectDirectory();

        //Write Customization entities
        var result = await dataHelper.GetCustomizationProjectEntitiesAsync(packageName);
        if (result != null)
            foreach (var item in result)
                itemHandler.HandleCustomizationsEntity(item);

        //Write project meta-file
        itemHandler.SaveProjectMetadata(projectInfo);
    }

    private static async Task MakeProjectFromSourceExAsync(IAcuConfiguration config)
    {
        await Task.Run(() =>
        {
            var packageHelper = new PackageHelper(config);
            packageHelper.MakePackage();
        });
    }

    #endregion
}