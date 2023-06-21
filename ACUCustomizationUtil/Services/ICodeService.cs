using ACUCustomizationUtils.Configuration;

namespace ACUCustomizationUtils.Services;

public interface ICodeService
{
    Task GetProjectSource(IAcuConfiguration config);
    Task MakeProjectFromSource(IAcuConfiguration config);
    Task CompileSolution(IAcuConfiguration config);
}