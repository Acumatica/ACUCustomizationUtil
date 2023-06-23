using ACUCustomizationUtils.Configuration;

namespace ACUCustomizationUtils.Helpers.Proxy;

public interface IPackageHelperProxy
{
    string GetPackageName(IAcuConfiguration configuration);
    string? GetPackageDescription(IAcuConfiguration configuration);
    void MakePackage(IAcuConfiguration configuration);
}