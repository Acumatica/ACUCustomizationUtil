using ACUCustomizationUtils.Configuration;

namespace ACUCustomizationUtils.Services;

public interface IErpService
{
    Task DownloadErp(IAcuConfiguration config);

    Task InstallErp(IAcuConfiguration config);

    Task UninstallErp(IAcuConfiguration config);
}