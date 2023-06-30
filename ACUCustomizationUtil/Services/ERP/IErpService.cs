using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;

namespace ACUCustomizationUtils.Services.ERP;

public interface IErpService
{
    Task DownloadErp(IAcuConfiguration config);

    Task InstallErp(IAcuConfiguration config);

    Task UninstallErp(IAcuConfiguration config);
}