using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using ACUCustomizationUtils.Configuration.Code;
using ACUCustomizationUtils.Configuration.Erp;
using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Configuration.Site;

namespace ACUCustomizationUtils.Configuration.ACU;
/// <summary>
/// POCO configuration class for acu util 
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public abstract class AcuConfigurationBase : IJsonOnDeserialized, IAcuConfiguration
{
    [JsonConstructor]
    protected AcuConfigurationBase()
    {
        Erp = new ErpConfiguration();
        Site = new SiteConfiguration();
        Package = new PackageConfiguration();
        Code = new CodeConfiguration();
    }

    public IErpConfiguration Erp { get; set; }
    public ISiteConfiguration Site { get; init; }
    public IPackageConfiguration Package { get; init; }
    public ICodeConfiguration Code { get; init; }
    public abstract bool IsNotNull { get; }


    [OnDeserialized]
    public virtual void OnDeserialized()
    {
        Erp.SetDefaultValues(this);
        Site.SetDefaultValues(this);
        Package.SetDefaultValues(this);
        Code.SetDefaultValues(this);
    }
}