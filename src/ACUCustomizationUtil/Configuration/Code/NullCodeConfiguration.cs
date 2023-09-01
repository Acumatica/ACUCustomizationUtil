namespace ACUCustomizationUtils.Configuration.Code;

public class NullCodeConfiguration : CodeConfigurationBase
{
    public static ICodeConfiguration Instance { get; } = new NullCodeConfiguration();
    public override bool IsNotNull => false;
}