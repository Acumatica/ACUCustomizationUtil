using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;

namespace ACUCustomizationUtils.Services.Package;

public interface IPackageService
{
    Task GetPackage(IAcuConfiguration config);
    Task PublishPackages(IAcuConfiguration config);
    Task UnpublishAllPackages(IAcuConfiguration config);
    Task UploadPackage(IAcuConfiguration config);
}