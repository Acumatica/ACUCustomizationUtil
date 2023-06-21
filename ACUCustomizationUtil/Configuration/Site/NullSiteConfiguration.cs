namespace ACUCustomizationUtils.Configuration.Site;

public class NullSiteConfiguration : SiteConfigurationBase
{
    public static ISiteConfiguration Instance { get; } = new NullSiteConfiguration();
    public override bool IsNotNull => false;
}