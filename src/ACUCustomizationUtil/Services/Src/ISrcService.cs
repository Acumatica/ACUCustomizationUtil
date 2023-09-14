using ACUCustomizationUtils.Configuration.ACU;

namespace ACUCustomizationUtils.Services.Src;

public interface ISrcService
{
    Task GetProjectSource(IAcuConfiguration config);
    Task MakeProjectFromSource(IAcuConfiguration config);
    Task CompileSolution(IAcuConfiguration config);
}