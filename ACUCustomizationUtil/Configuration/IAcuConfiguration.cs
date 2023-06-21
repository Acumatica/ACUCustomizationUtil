using System.Text.Json.Serialization;
using ACUCustomizationUtils.Configuration.Code;
using ACUCustomizationUtils.Configuration.Erp;
using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Configuration.Site;
using ACUCustomizationUtils.JSON;

namespace ACUCustomizationUtils.Configuration;

[JsonConverter(typeof(AcuConfigurationConverter))]
public interface IAcuConfiguration
{
    IErpConfiguration Erp { get; set; }
    ISiteConfiguration Site { get; init; }
    IPackageConfiguration Package { get; init; }
    ICodeConfiguration Code { get; init; }

    [JsonIgnore] bool IsNotNull { get; }
    void OnDeserialized();
}