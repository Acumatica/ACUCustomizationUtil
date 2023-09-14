using System.Text.Json.Serialization;
using ACUCustomizationUtils.Configuration.Erp;
using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Configuration.Site;
using ACUCustomizationUtils.Configuration.Src;
using ACUCustomizationUtils.JSON;

namespace ACUCustomizationUtils.Configuration.ACU;

[JsonConverter(typeof(AcuConfigurationConverter))]
public interface IAcuConfiguration
{
    IErpConfiguration Erp { get; set; }
    ISiteConfiguration Site { get; init; }
    IPackageConfiguration Pkg { get; init; }
    ISrcConfiguration Src { get; init; }

    [JsonIgnore] bool IsNotNull { get; }
    void OnDeserialized();
}