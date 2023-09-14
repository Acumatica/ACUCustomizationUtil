using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using ACUCustomizationUtils.Configuration.Erp;
using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Configuration.Site;
using ACUCustomizationUtils.Configuration.Src;

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
        Pkg = new PackageConfiguration();
        Src = new SrcConfiguration();
    }

    public IErpConfiguration Erp { get; set; }
    public ISiteConfiguration Site { get; init; }
    public IPackageConfiguration Pkg { get; init; }
    public ISrcConfiguration Src { get; init; }
    public abstract bool IsNotNull { get; }


    [OnDeserialized]
    public virtual void OnDeserialized()
    {
        Erp.SetDefaultValues(this);
        Site.SetDefaultValues(this);
        Pkg.SetDefaultValues(this);
        Src.SetDefaultValues(this);
    }
}