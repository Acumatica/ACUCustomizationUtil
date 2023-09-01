using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;

namespace ACUCustomizationUtils.Services.Site;

public interface ISiteService
{
    Task InstallSite(IAcuConfiguration acuConfiguration);
    Task UpdateSite(IAcuConfiguration acuConfiguration);
    Task UpdateDatabase(IAcuConfiguration config);
    Task DeleteSite(IAcuConfiguration acuConfiguration);
}