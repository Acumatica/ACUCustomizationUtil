using ACUCustomizationUtils.Configuration;

namespace ACUCustomizationUtils.Services;

public interface IPackageService
{
    Task GetPackage(IAcuConfiguration config);
    Task PublishPackages(IAcuConfiguration config);
    Task UnpublishAllPackages(IAcuConfiguration config);
    Task UploadPackage(IAcuConfiguration config);
}