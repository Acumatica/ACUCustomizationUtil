using ACUCustomizationUtils.Configuration;

namespace ACUCustomizationUtils.Helpers.ProxyServices;

public interface IPackageHelperProxy
{
    string GetPackageName(IAcuConfiguration configuration);
    string? GetPackageDescription(IAcuConfiguration configuration);
    void MakePackage(IAcuConfiguration configuration);
}