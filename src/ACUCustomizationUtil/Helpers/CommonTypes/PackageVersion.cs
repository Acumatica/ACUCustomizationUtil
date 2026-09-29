using System.Globalization;
using System.Reflection;

using ACUCustomizationUtils.Common;

namespace ACUCustomizationUtils.Helpers.CommonTypes;

/// <summary>
/// Represents customization version in two forms - optimized for <see cref="AssemblyVersionAttribute"/> and package
/// </summary>
public sealed class PackageVersion
{
    private const string IsvVersionFormat = "yyyy.MM.dd.HHmm";

    /// <summary>
    /// Instantiates new <see cref="PackageVersion"/>
    /// </summary>
    /// <param name="makeMode">Selected versioning mode</param>
    public PackageVersion(string? makeMode)
    {
        AssemblyComponent = GenerateAssemblyComponent(DateTime.Now);
        PackageComponent = makeMode switch
        {
            Messages.MakeModeQA => AssemblyComponent,
            Messages.MakeModeISV => ExpandToFourSegments(AssemblyComponent),
            _ => string.Empty,
        };
    }

    /// <summary>
    /// Date component used in package name
    /// </summary>
    public string PackageComponent { get; init; }

    /// <summary>
    /// Assembly component used in <see cref="AssemblyVersionAttribute"/> of the assembly
    /// </summary>
    public string AssemblyComponent { get; init; }

    /// <summary>
    /// True if version component for the package has been generated
    /// </summary>
    public bool IsVersioned => PackageComponent.Length > 0;

    /// <summary>
    /// Whether the make mode produces versioned packages
    /// </summary>
    /// <param name="makeMode">Selected versioning mode</param>
    public static bool IsVersionedMakeMode(string? makeMode)
    {
        return makeMode is Messages.MakeModeQA or Messages.MakeModeISV;
    }

    /// <summary>
    /// Date based version component of <paramref name="timestamp"/> in "yyDDD.HHmm" format
    /// (last 2 segments of assembly/package version)
    /// </summary>
    /// <param name="timestamp">Date to stamp into version</param>
    public static string GenerateAssemblyComponent(DateTime timestamp)
    {
        DateTime firstDate = new(timestamp.Year, 1, 1);
        string days = Math.Truncate((timestamp - firstDate).TotalDays).ToString("000", CultureInfo.InvariantCulture);
        string year = timestamp.ToString("yy", CultureInfo.InvariantCulture);
        string time = timestamp.ToString("HHmm", CultureInfo.InvariantCulture);

        return $"{year}{days}.{time}";
    }

    /// <summary>
    /// Expands the "yyDDD.HHmm" date component into the "yyyy.MM.dd.HHmm" format required for ISV packages
    /// </summary>
    /// <param name="dateVersion">Date component from assembly version</param>
    private static string ExpandToFourSegments(string dateVersion)
    {
        if (string.IsNullOrWhiteSpace(dateVersion) || dateVersion.Length != 10 || dateVersion[5] != '.')
            throw new ArgumentException(
                $"Version component is not in yyDDD.HHmm format: {dateVersion}",
                nameof(dateVersion)
            );

        // January 1st of the build year, moved by the days, hours and minutes from the version
        DateTime date = DateTime
            .ParseExact($"{dateVersion[..2]}0101", "yyMMdd", CultureInfo.InvariantCulture)
            .AddDays(int.Parse(dateVersion[2..5], CultureInfo.InvariantCulture))
            .AddHours(int.Parse(dateVersion[6..8], CultureInfo.InvariantCulture))
            .AddMinutes(int.Parse(dateVersion[8..10], CultureInfo.InvariantCulture));

        return date.ToString(IsvVersionFormat, CultureInfo.InvariantCulture);
    }
}
