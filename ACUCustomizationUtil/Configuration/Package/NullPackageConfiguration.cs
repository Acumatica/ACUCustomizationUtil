using ACUCustomizationUtils.Configuration.Erp;

namespace ACUCustomizationUtils.Configuration.Package;

public class NullPackageConfiguration : PackageConfigurationBase
{
    public static IPackageConfiguration Instance { get; } = new NullPackageConfiguration();
    public override bool IsNotNull => false;
}