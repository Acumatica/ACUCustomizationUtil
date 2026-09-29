using System.Globalization;

using ACUCustomizationUtils.Common;

namespace ACUCustomizationUtils.Helpers.CommonTypes;

/// <summary>
/// Package version component and the source it was resolved from
/// </summary>
public sealed class PackageVersion
{
    private const string IsvVersionFormat = "yyyy.MM.dd.HHmm";

    /// <summary>
    /// Instantiates new <see cref="PackageVersion"/>
    /// </summary>
    /// <param name="value">Version formatted for package (archive)</param>
    /// <param name="dateVersion">Date component of assembly version</param>
    /// <param name="source">Where the version was resolved from</param>
    private PackageVersion(string value, string dateVersion, PackageVersionSource source)
    {
        Value = value;
        DateVersion = dateVersion;
        Source = source;
    }

    /// <summary>
    /// Version as used in the package name and in the assembly metadata attribute
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Date based version component shared with the assembly version
    /// </summary>
    public string DateVersion { get; }

    /// <summary>
    /// Where the version was resolved from.
    /// </summary>
    public PackageVersionSource Source { get; }

    /// <summary>
    /// Whether the make mode produces versioned packages
    /// </summary>
    /// <param name="makeMode">Selected versioning mode</param>
    public static bool IsVersionedMakeMode(string? makeMode)
    {
        return makeMode is Messages.MakeModeQA or Messages.MakeModeISV;
    }

    /// <summary>
    /// Generates a new version from the current time for a QA/ISV make mode
    /// </summary>
    /// <param name="makeMode">Selected versioning mode</param>
    public static PackageVersion Generate(string? makeMode)
    {
        return Generate(makeMode, DateTime.Now);
    }

    /// <summary>
    /// Generates the version of a build started at <paramref name="timestamp"/> for a QA/ISV make mode
    /// </summary>
    /// <param name="makeMode">Selected versioning mode</param>
    /// <param name="timestamp">Date to stamp into version</param>
    public static PackageVersion Generate(string? makeMode, DateTime timestamp)
    {
        string dateVersion = GetDateVersion(timestamp);
        return new PackageVersion(Format(makeMode, dateVersion), dateVersion, PackageVersionSource.Generated);
    }

    /// <summary>
    /// Restores the version from the date component ("yyDDD.HHmm") of an assembly
    /// </summary>
    /// <param name="makeMode">Selected versioning mode.</param>
    /// <param name="dateVersion">Date component from assembly version</param>
    public static PackageVersion FromAssembly(string? makeMode, string dateVersion)
    {
        return new PackageVersion(Format(makeMode, dateVersion), dateVersion, PackageVersionSource.Assembly);
    }

    /// <summary>
    /// Date based version component of <paramref name="timestamp"/> in "yyDDD.HHmm" format
    /// (last 2 segments of assembly/package version)
    /// </summary>
    /// <param name="timestamp">Date to stamp into version</param>
    public static string GetDateVersion(DateTime timestamp)
    {
        DateTime firstDate = new(timestamp.Year, 1, 1);
        string days = Math.Truncate((timestamp - firstDate).TotalDays).ToString("000", CultureInfo.InvariantCulture);
        string year = timestamp.ToString("yy", CultureInfo.InvariantCulture);
        string time = timestamp.ToString("HHmm", CultureInfo.InvariantCulture);

        return $"{year}{days}.{time}";
    }

    /// <summary>
    /// Formats <paramref name="dateVersion"/> into format expected for <paramref name="makeMode"/>
    /// </summary>
    /// <param name="makeMode">Selected versioning mode</param>
    /// <param name="dateVersion">Date component from assembly version</param>
    /// <returns>Version string in a format fit for <paramref name="makeMode"/></returns>
    private static string Format(string? makeMode, string dateVersion)
    {
        return makeMode switch
        {
            Messages.MakeModeQA => dateVersion,
            Messages.MakeModeISV => ExpandToFourSegments(dateVersion),
            _ => throw new ArgumentException(
                $"Make mode {makeMode ?? Messages.MakeModeBase} has no package version",
                nameof(makeMode)
            ),
        };
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
