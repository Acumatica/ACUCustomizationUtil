namespace ACUCustomizationUtils.Configuration.Erp;

public class ErpNullConfiguration : ErpConfigurationBase
{
    public static IErpConfiguration Instance { get; } = new ErpNullConfiguration();
    public override bool IsNotNull => false;
}