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
public class PackageService(ILogger<PackageService> logger) : IPackageService
{
	public async Task GetPackage(IAcuConfiguration config)
    {
        logger.LogInformation("Execute GetPackage action");
        try
        {
            await AnsiConsole.Status().StartAsync("Download package", async ctx =>
            {
                ctx.Status("Reading configuration ...");
                logger.LogInformation("Reading configuration");
                ConfigurationHelper.PrintConfiguration(config, logger, nameof(IAcuConfiguration.Pkg));

                ctx.Status("Validate configuration ...");
                logger.LogInformation("Validate configuration");
                PackageValidator.ValidateForGet(config.Pkg);

                logger.LogInformation("Download package {Package}", config.Pkg.PkgName);
                ctx.Status("Download in progress, please wait ...");
                using var client = GetClient(config);
                await client.GetPackage();

            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetPackage: action error!");
        }

        logger.LogInformation("GetPackage action complete");
    }

    public async Task PublishPackages(IAcuConfiguration config)
    {
        logger.LogInformation("Execute PublishPackages action");
        try
        {
            await AnsiConsole.Status().StartAsync("Publish package", async ctx =>
            {
                ctx.Status("Reading configuration ...");
                logger.LogInformation("Reading configuration");
                ConfigurationHelper.PrintConfiguration(config, logger, nameof(IAcuConfiguration.Pkg));

                ctx.Status("Validate configuration ...");
                logger.LogInformation("Validate configuration");
                PackageValidator.ValidateForPublish(config.Pkg);

                logger.LogInformation("Publish package {Package}", config.Pkg.PkgName);
                ctx.Status("Publish in progress, please wait ...");
                using var client = GetClient(config);
                await client.PublishPackages();

            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "PublishPackages: action error!");
        }

        logger.LogInformation("PublishPackages action complete");
    }

    public async Task UnpublishAllPackages(IAcuConfiguration config)
    {
        logger.LogInformation("Execute UnpublishAllPackages action");
        try
        {
            await AnsiConsole.Status().StartAsync("Unpublish packages", async ctx =>
            {
                ctx.Status("Reading configuration ...");
                logger.LogInformation("Reading configuration");
                ConfigurationHelper.PrintConfiguration(config, logger, nameof(IAcuConfiguration.Pkg));

                ctx.Status("Validate configuration ...");
                logger.LogInformation("Validate configuration");
                PackageValidator.ValidateForUnpublish(config.Pkg);

                logger.LogInformation("Unpublish package(s) {Package}", config.Pkg.PkgName);
                ctx.Status("Unpublish in progress, please wait ...");
                using var client = GetClient(config);
                await client.UnpublishAllPackages();

            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "UnpublishAllPackages: action error!");
        }

        logger.LogInformation("UnpublishAllPackages action complete");
    }

    public async Task UploadPackage(IAcuConfiguration config)
    {
        logger.LogInformation("Execute UploadPackage action");
        try
        {
            await AnsiConsole.Status().StartAsync("UploadPackage packages", async ctx =>
            {
                ctx.Status("Reading configuration ...");
                logger.LogInformation("Reading configuration");
                ConfigurationHelper.PrintConfiguration(config, logger, nameof(IAcuConfiguration.Pkg));

                ctx.Status("Validate configuration ...");
                logger.LogInformation("Validate configuration");
                PackageValidator.ValidateForUpload(config.Pkg);

                logger.LogInformation("Uploading package {Package}", config.Pkg.PkgName);
                ctx.Status("UploadPackage in progress, please wait ...");
                using var client = GetClient(config);
                await client.UploadPackage();

            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "UploadPackage: action error!");
        }

        logger.LogInformation("UploadPackage action complete");
    }

    /// <summary>
    /// Acumatica introduced the Customization API in version 2022 R2 of its REST API. 
    /// https://community.acumatica.com/develop-customizations-288/customization-api-17892
    /// </summary>
    private static IAcuCustomizationClient GetClient(IAcuConfiguration config)
    {
        var erpVersion = config.Erp.ErpVersion!.Split('.');
        if (decimal.TryParse(erpVersion[0] + "." + erpVersion[1], out decimal result)
            && result >= 22.2m)
        {
            return new RestClient(config);
        }
        return new SoapClient(config);
    }
}