using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.Site;
using ACUCustomizationUtils.Helpers;
using ACUCustomizationUtils.Validators.Site;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace ACUCustomizationUtils.Services;
/// <summary>
/// This class contains methods for handle Site subcommands
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public class SiteService : ISiteService
{
    private readonly ILogger<SiteService> _logger;

    public SiteService(ILogger<SiteService> logger)
    {
        _logger = logger;
    }

    public async Task InstallSite(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute InstallSite action");
        try
        {
            await AnsiConsole.Status().StartAsync("Install Acumatica instance", async ctx =>
            {
                ctx.Status("Reading configuration ...");
                _logger.LogInformation("Reading configuration");
                ConfigurationService.PrintConfiguration(config, _logger);
                ctx.Status("Validate configuration ...");
                _logger.LogInformation("Validate configuration");
                SiteValidator.ValidateForInstall(config.Site);
                ctx.Status("Installation in progress, please wait ...");
                _logger.LogInformation("Installing new Acumatica instance");
                var processArgs = GetSiteInstallCmdArgs(config.Site);
                var processHelper = new ProcessHelper(config.Site.AcumaticaToolPath!, processArgs, ctx);
                await processHelper.Execute();
                _logger.LogInformation("Reset passwords to default for new Acumatica instance");
                var dbHelper = new DatabaseHelper(config);
                await dbHelper.UpdateAdminPasswordDefault();
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Install Acumatica instance: action error!");
        }

        _logger.LogInformation("InstallSite action complete");
    }

    public async Task UpdateSite(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute UpdateSite action");
        try
        {
            await AnsiConsole.Status().StartAsync("Delete Acumatica instance", async ctx =>
            {
                _logger.LogInformation("Reading configuration");
                ConfigurationService.PrintConfiguration(config, _logger);

                _logger.LogInformation("Validate configuration");
                SiteValidator.ValidateForUpdate(config.Site);

                _logger.LogInformation("Updating Acumatica instance");
                ctx.Status("Updating in progress, please wait ...");
                var processArgs = GetSiteUpdateCmdArgs(config.Site);
                var processHelper = new ProcessHelper(config.Site.AcumaticaToolPath!, processArgs, ctx);
                await processHelper.Execute();
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update Acumatica instance: action error!");
        }

        _logger.LogInformation("UpdateSite action complete");
    }

    public async Task UpdateDatabase(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute UpdateDatabase action");
        try
        {
            await AnsiConsole.Status().StartAsync("Update Acumatica database", async ctx =>
            {
                ctx.Status("Reading configuration ...");
                _logger.LogInformation("Reading configuration");
                ConfigurationService.PrintConfiguration(config, _logger);
                ctx.Status("Validate configuration ...");
                _logger.LogInformation("Validate configuration");
                SiteValidator.ValidateForUpdate(config.Site);
                ctx.Status("Update in progress, please wait ...");
                _logger.LogInformation("Updating Acumatica database");
                var processArgs = GetDatabaseUpdateCmdArgs(config.Site);
                var processHelper = new ProcessHelper(config.Site.AcumaticaToolPath!, processArgs, ctx);
                await processHelper.Execute();
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update Acumatica database: action error!");
        }

        _logger.LogInformation("DeleteSite action complete");
    }

    public async Task DeleteSite(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute DeleteSite action");
        try
        {
            await AnsiConsole.Status().StartAsync("Delete Acumatica instance", async ctx =>
            {
                ctx.Status("Reading configuration ...");
                _logger.LogInformation("Reading configuration");
                ConfigurationService.PrintConfiguration(config, _logger);
                ctx.Status("Validate configuration ...");
                _logger.LogInformation("Validate configuration");
                SiteValidator.ValidateForDelete(config.Site);
                ctx.Status("Deletion in progress, please wait ...");
                _logger.LogInformation("Deleting Acumatica instance");
                var processArgs = GetSiteDeleteCmdArgs(config.Site);
                var processHelper = new ProcessHelper(config.Site.AcumaticaToolPath!, processArgs, ctx);
                await processHelper.Execute();
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Delete Acumatica instance: action error!");
        }

        _logger.LogInformation("DeleteSite action complete");
    }

    private static string GetSiteInstallCmdArgs(ISiteConfiguration siteConfig)
    {
        var args =
            $"-cm:\"NewInstance\" -s:\"{siteConfig.SqlServerName}\" -d:\"{siteConfig.DbName}\" -c:\"ci=1;ct=;cn=;\" -c:\"ci=2;ct=SalesDemo;cp=1;cv=Yes;cn=Company;\" -i:\"{siteConfig.InstanceName}\" -h:\"{siteConfig.InstancePath}\\\" -w:\"{siteConfig.IisWebSite}\" -v:\"{siteConfig.InstanceName}\" -po:\"{siteConfig.IisAppPool}\" -op:\"Forced\"";
        return args;
    }

    private static string GetSiteDeleteCmdArgs(ISiteConfiguration siteConfig)
    {
        var args = $"-cm:\"DeleteSite\" -i:\"{siteConfig.InstanceName}\" -op:\"Forced\"";
        return args;
    }

    private static string GetSiteUpdateCmdArgs(ISiteConfiguration siteConfig)
    {
        var args = $"-cm:\"UpgradeSite\" -i:\"{siteConfig.InstanceName}\" -op:\"Forced\"";
        return args;
    }

    private static string GetDatabaseUpdateCmdArgs(ISiteConfiguration siteConfig)
    {
        var args =
            $"-cm:\"DBMaint\" -s:\"{siteConfig.SqlServerName}\" -d:\"{siteConfig.DbName}\" -n:\"False\" -b:\"True\" -op:\"Forced\"";
        return args;
    }
}