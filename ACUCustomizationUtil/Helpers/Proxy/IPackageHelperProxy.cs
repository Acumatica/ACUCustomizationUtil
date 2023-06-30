using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;

namespace ACUCustomizationUtils.Helpers.Proxy;

public interface IPackageHelperProxy
{
    string GetPackageName(IAcuConfiguration configuration);
    string? GetPackageDescription(IAcuConfiguration configuration);
    void MakePackage(IAcuConfiguration configuration);
}