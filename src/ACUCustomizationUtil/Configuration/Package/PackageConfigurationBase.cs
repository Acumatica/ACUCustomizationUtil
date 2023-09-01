﻿using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Extensions;

namespace ACUCustomizationUtils.Configuration.Package;
/// <summary>
/// POCO configuration class for acu util (Package section)
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public abstract class PackageConfigurationBase : IPackageConfiguration
{
    public Uri? Url { get; set; }
    public string? Login { get; set; }
    public string? Password { get; set; }
    public string? Tenant { get; set; }
    public string? PackageName { get; set; }
    public string? PackageDirectory { get; set; }
    public string? PackageFilePath { get; private set; }
    public abstract bool IsNotNull { get; }

    public IPackageConfiguration SetDefaultValues(IAcuConfiguration configuration)
    {
        if (!string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Tenant))
        {
            Login = $"{Login}@{Tenant}";
        }

        PackageDirectory = PackageDirectory.TryGetFullDirectoryPath();
        if (PackageName != null && PackageDirectory != null)
        {
            var file = PackageName!.EndsWith(".zip") ? PackageName : $"{PackageName}.zip";
            PackageFilePath = Path.Combine(PackageDirectory, file);
        }
        

        return this;
    }
}