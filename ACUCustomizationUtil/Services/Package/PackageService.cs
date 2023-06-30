using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Helpers;
using ACUCustomizationUtils.Validators.Package;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace ACUCustomizationUtils.Services.Package;

/// <summary>
/// This class contains methods for handle Package subcommands
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public class PackageService : IPackageService
{
    private readonly ILogger<PackageService> _logger;

    public PackageService(ILogger<PackageService> logger)
    {
        _logger = logger;
    }

    public async Task GetPackage(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute GetPackage action");
        try
        {
            await AnsiConsole.Status().StartAsync("Download package", async ctx =>
            {
                ctx.Status("Reading configuration ...");
                _logger.LogInformation("Reading configuration");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Package));

                ctx.Status("Validate configuration ...");
                _logger.LogInformation("Validate configuration");
                PackageValidator.ValidateForGet(config.Package);

                ctx.Status("Download in progress, please wait ...");
                _logger.LogInformation("Download package {Package}", config.Package.PackageName);
                using var client = new SoapClient(config);
                await client.GetPackage();
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetPackage: action error!");
        }

        _logger.LogInformation("GetPackage action complete");
    }

    public async Task PublishPackages(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute PublishPackages action");
        try
        {
            await AnsiConsole.Status().StartAsync("Publish package", async ctx =>
            {
                ctx.Status("Reading configuration ...");
                _logger.LogInformation("Reading configuration");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Package));

                ctx.Status("Validate configuration ...");
                _logger.LogInformation("Validate configuration");
                PackageValidator.ValidateForPublish(config.Package);

                ctx.Status("Publish in progress, please wait ...");
                _logger.LogInformation("Publish package {Package}", config.Package.PackageName);
                using var client = new SoapClient(config);
                await client.PublishPackages();
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PublishPackages: action error!");
        }

        _logger.LogInformation("PublishPackages action complete");
    }


    public async Task UnpublishAllPackages(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute UnpublishAllPackages action");
        try
        {
            await AnsiConsole.Status().StartAsync("Unpublish packages", async ctx =>
            {
                ctx.Status("Reading configuration ...");
                _logger.LogInformation("Reading configuration");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Package));

                ctx.Status("Validate configuration ...");
                _logger.LogInformation("Validate configuration");
                PackageValidator.ValidateForUnpublish(config.Package);

                ctx.Status("Unpublish in progress, please wait ...");
                _logger.LogInformation("Unpublish package(s) {Package} complete", config.Package.PackageName);
                using var client = new SoapClient(config);
                await client.UnpublishAllPackages();
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "UnpublishAllPackages: action error!");
        }

        _logger.LogInformation("UnpublishAllPackages action complete");
    }

    public async Task UploadPackage(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute UploadPackage action");
        try
        {
            await AnsiConsole.Status().StartAsync("UploadPackage packages", async ctx =>
            {
                ctx.Status("Reading configuration ...");
                _logger.LogInformation("Reading configuration");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Package));

                ctx.Status("Validate configuration ...");
                _logger.LogInformation("Validate configuration");
                PackageValidator.ValidateForUpload(config.Package);

                ctx.Status("UploadPackage in progress, please wait ...");
                _logger.LogInformation("Uploading package {Package} ...", config.Package.PackageName);
                using var client = new SoapClient(config);
                await client.UploadPackage();
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "UploadPackage: action error!");
        }

        _logger.LogInformation("UploadPackage action complete");
    }
}