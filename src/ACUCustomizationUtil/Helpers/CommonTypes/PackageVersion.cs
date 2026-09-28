namespace ACUCustomizationUtils.Helpers.CommonTypes;

/// <summary>
/// Package version component ("yyDDD.HHmm") and the source it was resolved from
/// </summary>
public class PackageVersion(string value, PackageVersionSource source)
{
    public string Value { get; } = value;
    public PackageVersionSource Source { get; } = source;
}
