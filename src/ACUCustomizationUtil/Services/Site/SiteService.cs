﻿using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Configuration.Site;
using ACUCustomizationUtils.Helpers;
using ACUCustomizationUtils.Validators.Site;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace ACUCustomizationUtils.Services.Site;

/// <summary>
///     This class contains methods for handle Site subcommands
/// </summary>
/// <remarks>
///     Authored by Aleksej Slusar
///     email: aleksej.slusar@sprinterra.com
///     Copyright Sprinterra(c) 2023
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
        // var processSuccess = true;
        // Exception? processException = null;
        try
        {
            await AnsiConsole.Status().StartAsync("Install Acumatica instance", async ctx =>
            {
                _logger.LogInformation("Reading configuration");
                ctx.Status("Reading configuration ...");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Site));

                _logger.LogInformation("Validate configuration");
                ctx.Status("Validate configuration ...");
                SiteValidator.ValidateForInstall(config.Site);
                SiteValidator.ValidateForInstallV(config);

                //Install
                _logger.LogInformation("Installing new Acumatica instance {Instance}", config.Site.InstanceName);
                ctx.Status("Installation in progress, please wait ...");
                var processArgs = GetSiteInstallCmdArgs(config.Site);
                var processHelper = new ProcessHelper(config.Site.AcumaticaToolPath!, processArgs, ctx);
                await processHelper.Execute();
                
                //Fix DB
                ctx.Status("Update some database values, please wait ...");
                var dbHelper = new DatabaseHelper(config);
                _logger.LogInformation("Reset admin passwords to default for new Acumatica instance");
                _logger.LogInformation("Check & create server login for IIS DefaultAppPool");
                await dbHelper.UpdateAdminPasswordDefault();
                await dbHelper.UpdateServerLoginDefault();
            });
            _logger.LogInformation("InstallSite action success");
        }
        catch (Exception? e)
        {
            // processSuccess = false;
            // processException = e;
            
            _logger.LogError(e, "InstallSite action NOT success!");
        }

        // if (processSuccess)
        //     _logger.LogInformation("InstallSite action success");
        // else
        //     _logger.LogError(processException, "InstallSite action NOT success!");
    }

    public async Task UpdateSite(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute UpdateSite action");

        await AnsiConsole.Status().StartAsync("Delete Acumatica instance", async ctx =>
        {
            try
            {
                _logger.LogInformation("Reading configuration");
                ctx.Status("Reading configuration ...");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Site));

                _logger.LogInformation("Validate configuration");
                ctx.Status("Validate configuration ...");
                SiteValidator.ValidateForUpdate(config.Site);

                _logger.LogInformation("Updating Acumatica instance {Instance}", config.Site.InstanceName);
                ctx.Status("Updating in progress, please wait ...");
                var processArgs = GetSiteUpdateCmdArgs(config.Site);
                var processHelper = new ProcessHelper(config.Site.AcumaticaToolPath!, processArgs, ctx);
                await processHelper.Execute();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Update Acumatica instance: action error!");
            }

            _logger.LogInformation("UpdateSite action complete");

        });
    }

    public async Task UpdateDatabase(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute UpdateDatabase action");
        try
        {
            await AnsiConsole.Status().StartAsync("Update Acumatica database", async ctx =>
            {
                _logger.LogInformation("Reading configuration");
                ctx.Status("Reading configuration ...");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Site));

                _logger.LogInformation("Validate configuration");
                ctx.Status("Validate configuration ...");
                SiteValidator.ValidateForUpdate(config.Site);

                _logger.LogInformation("Updating Acumatica database {Database}", config.Site.DbName);
                ctx.Status("Update in progress, please wait ...");
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
                _logger.LogInformation("Reading configuration");
                ctx.Status("Reading configuration ...");
                ConfigurationHelper.PrintConfiguration(config, _logger);

                _logger.LogInformation("Validate configuration");
                ctx.Status("Validate configuration ...");
                SiteValidator.ValidateForDelete(config.Site);

                _logger.LogInformation("Deleting Acumatica instance");
                ctx.Status("Deletion in progress, please wait ...");
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
            $"-cm:\"NewInstance\" -s:\"{siteConfig.SqlServerName}\" -d:\"{siteConfig.DbName}\" -c:\"ci=1;ct=;cn=;\" -c:\"ci=2;ct=SalesDemo;cp=1;cv=Yes;cn=Company;\" -i:\"{siteConfig.InstanceName}\" -h:\"{siteConfig.InstancePath}\" -w:\"{siteConfig.IisWebSite}\" -v:\"{siteConfig.InstanceName}\" -po:\"{siteConfig.IisAppPool}\" -op:\"Forced\"";
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