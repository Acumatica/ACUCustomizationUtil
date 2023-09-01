using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;

namespace ACUCustomizationUtils.Services.Code;

public interface ICodeService
{
    Task GetProjectSource(IAcuConfiguration config);
    Task MakeProjectFromSource(IAcuConfiguration config);
    Task CompileSolution(IAcuConfiguration config);
}