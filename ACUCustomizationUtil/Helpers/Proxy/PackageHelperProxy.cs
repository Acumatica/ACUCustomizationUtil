using ACUCustomizationUtils.Configuration;

namespace ACUCustomizationUtils.Helpers.Proxy;

public class PackageHelperProxy : IPackageHelperProxy
{
    public virtual string GetPackageName(IAcuConfiguration configuration)
    {
        return PackageHelper.GetPackageName(configuration);
    }

    public virtual string? GetPackageDescription(IAcuConfiguration configuration)
    {
        return PackageHelper.GetPackageDescription(configuration);
    }

    public void MakePackage(IAcuConfiguration configuration)
    {
        var packageSourceDir = configuration.Code.PkgSourceDirectory!;
        var erpVersion = configuration.Erp.Version!;
        var level = int.TryParse(configuration.Code.PkgLevel, out var l) ? l : 0;
        var packageDestinationDir = configuration.Package.PackageDirectory!;
        var packageName = GetPackageName(configuration);
        var packageFileName = Path.Combine(packageDestinationDir, packageName);
        var description = configuration.Code.PkgDescription ?? GetPackageDescription(configuration);

        PackageHelper.MakePackage(packageSourceDir, packageFileName, erpVersion, description, level);
    }
}