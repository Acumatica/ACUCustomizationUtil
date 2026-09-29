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
public class SrcService(ILogger<SrcService> logger) : ISrcService
{
    private readonly ILogger<SrcService> _logger = logger;

    #region Public methods

    public async Task GetProjectSource(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute GetProjectSource action");
        try
        {
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Download project source items",
                    async ctx =>
                    {
                        _logger.LogInformation("Reading configuration");
                        ctx.Status("Reading configuration ...");
                        ConfigurationHelper.PrintConfiguration(
                            config,
                            _logger,
                            nameof(IAcuConfiguration.Pkg),
                            nameof(IAcuConfiguration.Src)
                        );

                        _logger.LogInformation("Validate configuration");
                        ctx.Status("Validate configuration ...");
                        SrcValidator.ValidateForSrc(config);

                        _logger.LogInformation(
                            "Download source items for project {Package}",
                            config.Pkg.PkgName
                        );
                        ctx.Status("Download in progress, please wait ...");
                        await GetProjectSourceExAsync(config);
                    }
                );
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
            await AnsiConsole
                .Status()
                .StartAsync(
                    "Making project from source",
                    async ctx =>
                    {
                        _logger.LogInformation("Reading configuration");
                        ctx.Status("Reading configuration ...");
                        ConfigurationHelper.PrintConfiguration(
                            config,
                            _logger,
                            nameof(IAcuConfiguration.Pkg),
                            nameof(IAcuConfiguration.Src)
                        );

                        _logger.LogInformation("Validating configuration ...");
                        ctx.Status("Validate configuration ...");
                        SrcValidator.ValidateForMake(config);

                        bool hasSolution = HasSolution(config);

                        if (hasSolution)
                        {
                            _logger.LogInformation("Validate build configuration");
                            ctx.Status("Validating build configuration ...");
                            SrcValidator.ValidateForBuild(config);

                            _logger.LogInformation("Compile external library code for project {Package}", config.Pkg.PkgName);
                            ctx.Status("Compiling project ...");
                            MsBuildHelper msBuildHelper = new MsBuildHelper(config, ctx);
                            await msBuildHelper.Execute();

                            _logger.LogInformation("Copy external library assembly to package source");
                            ctx.Status("Copying external library assembly to package source ...");
                            await msBuildHelper.CopyAssemblyToPackageBinAsync();
                        }
                        else
                        {
                            _logger.LogInformation("External library solution is not found, build is skipped");
                        }

                        ctx.Status("Making package ...");
                        _logger.LogInformation(
                            "Making package for project {Package}",
                            config.Pkg.PkgName
                        );
                        await MakeProjectFromSourceExAsync(config);

                    }
                );
            _logger.LogInformation("MakeProjectFromSource action complete");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "MakeProjectFromSource: action error!");
        }
    }

    #endregion

    #region Private methods

    private static async Task GetProjectSourceExAsync(IAcuConfiguration config)
    {
        string packageName = config.Pkg.PkgName!;
        string sourceDirectory = config.Src.PkgSourceDirectory!;

        sourceDirectory.TryCheckCreateDirectory();
        if (!Directory.Exists(sourceDirectory))
            throw new Exception(
                $"Directory {sourceDirectory} is not exist or access is not allowed to it"
            );

        DatabaseHelper dataHelper = new DatabaseHelper(config);
        CstEntityHelper itemHandler = new CstEntityHelper(config);

        Helpers.CommonTypes.CustomizationProject projectInfo =
            await dataHelper.GetCustomizationProjectAsync()
            ?? throw new Exception(
                $"Project {packageName} not found in the target database"
            );

        //Clear directory
        itemHandler.ClearProjectDirectory();

        //Write Customization entities
        IEnumerable<Helpers.CommonTypes.CustomizationProjectEntity>? result = await dataHelper.GetCustomizationProjectEntitiesAsync();
        if (result != null)
            foreach (Helpers.CommonTypes.CustomizationProjectEntity item in result)
                itemHandler.HandleCustomizationsEntity(item, dataHelper);

        //Write project meta-file
        itemHandler.SaveProjectMetadata(projectInfo);
    }

    private static bool HasSolution(IAcuConfiguration config)
    {
        return File.Exists(config.Src.MsBuildSolutionFile);
    }

    private async Task MakeProjectFromSourceExAsync(IAcuConfiguration config)
    {
        await Task.Run(() =>
        {
            PackageHelper packageHelper = new(config);
            if (packageHelper.PackageVersion != null)
                _logger.LogInformation(
                    "Package version {Version} (source: {Source})",
                    packageHelper.PackageVersion.Value,
                    packageHelper.PackageVersion.Source
                );
            packageHelper.MakePackage();
            _logger.LogInformation("Package {PackageFile} created", packageHelper.PackageFileName);
        });
    }

    #endregion
}
