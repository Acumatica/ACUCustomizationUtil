#nullable enable

using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Helpers;
using ACUCustomizationUtils.Validators.Erp;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace ACUCustomizationUtils.Services.ERP;

/// <summary>
/// This class contains methods for handle ERP subcommands
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public class ErpService : IErpService
{
    #region Services

    private readonly ILogger<ErpService> _logger;

    #endregion

    public ErpService(ILogger<ErpService> logger)
    {
        _logger = logger;
    }

    #region Action methods

    /// <summary>
    /// Download ERP from distribution site
    /// </summary>
    /// <returns></returns>
    public async Task DownloadErp(IAcuConfiguration config)
    {
        WebClient.ProgressChangedHandler OnDownloadProgressChanged(StatusContext ctx)
        {
            return (size, downloaded, percentage) => { ctx.Status($"Progress: {downloaded}/{size} {percentage}%"); };
        }

        _logger.LogInformation("Execute DownloadErp action");
        try
        {
            await AnsiConsole.Status().StartAsync("Download ERP installation", async ctx =>
            {
                _logger.LogInformation("Reading configuration");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Erp));

                _logger.LogInformation("Validate configuration");
                ErpValidator.ValidateForDownload(config.Erp);

                _logger.LogInformation("Downloading installation file");
                ctx.Status("Downloading in progress, please wait ...");
                using var client = new WebClient(config.Erp.Url!, config.Erp.InstallationFilePath!);
                client.ProgressChanged += OnDownloadProgressChanged(ctx);
                await client.DownloadFileAsync();
            });
            _logger.LogInformation("Download complete");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Download installation file: action error!");
        }

        _logger.LogInformation("Download ERP installation complete");
    }

    /// <summary>
    /// Install ERP
    /// </summary>
    /// <returns></returns>
    public async Task InstallErp(IAcuConfiguration config)
    {
        string GetErpInstallCmdArgs(string file, string dir)
        {
            return $"/a  {file} /qn /quiet targetdir={dir}";
        }

        _logger.LogInformation("Execute InstallErp action");
        try
        {
            await AnsiConsole.Status().StartAsync("Install ERP", async ctx =>
            {
                _logger.LogInformation("Reading configuration");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Erp));

                _logger.LogInformation("Validate configuration");
                ErpValidator.ValidateForInstall(config.Erp);

                _logger.LogInformation("Starting installing ERP to {Dir}", config.Erp.InstallationDirectory);
                ctx.Status("Installation in progress, please wait ...");
                var processArgs = GetErpInstallCmdArgs(config.Erp.InstallationFilePath!,
                    config.Erp.InstallationDirectory!);
                var processHelper = new ProcessHelper(Messages.Msiexec, processArgs, ctx);
                await processHelper.Execute();
                await Task.Run(() => { File.Delete(config.Erp.InstallationFilePath!); });
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Install ERP: action error!");
        }

        _logger.LogInformation("Installation complete");
    }

    /// <summary>
    /// Uninstall erp from target directory
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task UninstallErp(IAcuConfiguration config)
    {
        _logger.LogInformation("Execute UninstallErp action");
        try
        {
            await AnsiConsole.Status().StartAsync("Uninstalling ERP", async ctx =>
            {
                _logger.LogInformation("Reading configuration");
                ConfigurationHelper.PrintConfiguration(config, _logger, nameof(IAcuConfiguration.Erp));

                _logger.LogInformation("Validate configuration");
                ErpValidator.ValidateForDelete(config.Erp);

                _logger.LogInformation("Start uninstalling ERP from {Dir}", config.Erp.InstallationDirectory);
                ctx.Status("Uninstall ERP in progress, please wait ...");
                await Task.Run(() => { Directory.Delete(config.Erp.InstallationDirectory!, true); });
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Uninstall ERP: action error!");
        }

        _logger.LogInformation("Uninstall complete");
    }

    #endregion Action methods
}