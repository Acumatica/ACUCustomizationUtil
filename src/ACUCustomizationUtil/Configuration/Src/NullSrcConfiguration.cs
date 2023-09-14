namespace ACUCustomizationUtils.Configuration.Src;

public class NullSrcConfiguration : SrcConfigurationBase
{
    public static ISrcConfiguration Instance { get; } = new NullSrcConfiguration();
    public override bool IsNotNull => false;
}