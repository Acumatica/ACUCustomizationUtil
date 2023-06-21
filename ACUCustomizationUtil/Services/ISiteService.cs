using ACUCustomizationUtils.Configuration;

namespace ACUCustomizationUtils.Services;

public interface ISiteService
{
    Task InstallSite(IAcuConfiguration acuConfiguration);
    Task UpdateSite(IAcuConfiguration acuConfiguration);
    Task UpdateDatabase(IAcuConfiguration config);
    Task DeleteSite(IAcuConfiguration acuConfiguration);
}