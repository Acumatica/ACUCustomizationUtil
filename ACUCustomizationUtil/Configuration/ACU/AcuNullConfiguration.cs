namespace ACUCustomizationUtils.Configuration.ACU;

public class AcuNullConfiguration : AcuConfigurationBase
{
    public static AcuNullConfiguration Instance { get; } = new();
    public override bool IsNotNull => false;
}