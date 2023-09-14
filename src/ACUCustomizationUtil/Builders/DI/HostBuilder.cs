using ACUCustomizationUtils.Builders.Commands;
using ACUCustomizationUtils.Services.ERP;
using ACUCustomizationUtils.Services.Package;
using ACUCustomizationUtils.Services.Site;
using ACUCustomizationUtils.Services.Src;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ACUCustomizationUtils.Builders.DI;

/// <summary>
/// This class is the point of building an application DI
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public static class HostBuilder
{
    public static IHost Build(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddTransient<IErpService, ErpService>();
                services.AddTransient<ISiteService, SiteService>();
                services.AddTransient<IPackageService, PackageService>();
                services.AddTransient<ISrcService, SrcService>();
                services.AddSingleton<ErpCommandBuilder>();
                services.AddSingleton<SiteCommandBuilder>();
                services.AddSingleton<PackageCommandBuilder>();
                services.AddSingleton<CodeCommandBuilder>();
            })
            .UseSerilog()
            .Build();
    }
}