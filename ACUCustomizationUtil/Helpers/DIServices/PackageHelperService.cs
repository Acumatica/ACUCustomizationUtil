using ACUCustomizationUtils.Configuration;

namespace ACUCustomizationUtils.Helpers.DIServices;

public class PackageHelperService : IPackageHelperService
{
    private readonly IAcuConfiguration _configuration;
    private string? _packageFileName;
    private readonly string _packageSourceDir;
    private readonly string _erpVersion;
    private string? _description;
    private readonly int _level;

    public PackageHelperService(IAcuConfiguration configuration)
    {
        _configuration = configuration;
        _packageSourceDir = configuration.Code.PkgSourceDirectory!;
        _erpVersion = configuration.Erp.Version!;
        _level = int.TryParse(configuration.Code.PkgLevel, out var l) ? l : 0;
    }

    public virtual string GetPackageName()
    {
        return PackageHelper.GetPackageName(_configuration);
    }

    public virtual string? GetPackageDescription()
    {
        return PackageHelper.GetPackageDescription(_configuration);
    }

    public void MakePackage()
    {
        var packageDestinationDir = _configuration.Package.PackageDirectory!;
        var packageName = GetPackageName();
        _packageFileName = Path.Combine(packageDestinationDir, packageName);
        _description = _configuration.Code.PkgDescription ?? GetPackageDescription();
        PackageHelper.MakePackage(_packageSourceDir, _packageFileName, _erpVersion, _description, _level);
    }
}

public class PackageHelperServiceNaw : PackageHelperService
{
    public PackageHelperServiceNaw(IAcuConfiguration configuration) : base(configuration)
    {
    }
    
}