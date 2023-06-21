﻿using ACUCustomizationUtils.Builders.Commands;
using ACUCustomizationUtils.Helpers.ProxyServices;
using ACUCustomizationUtils.Services;
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
                services.AddSingleton<IPackageHelperProxy, PackageHelperProxy>();
                services.AddTransient<IErpService, ErpService>();
                services.AddTransient<ISiteService, SiteService>();
                services.AddTransient<IPackageService, PackageService>();
                services.AddTransient<ICodeService, CodeService>();
                services.AddTransient<ConfigurationService>();
                services.AddSingleton<ErpCommandBuilder>();
                services.AddSingleton<SiteCommandBuilder>();
                services.AddSingleton<PackageCommandBuilder>();
                services.AddSingleton<CodeCommandBuilder>();
            })
            .UseSerilog()
            .Build();
    }
}