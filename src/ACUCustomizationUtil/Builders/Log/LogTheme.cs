using Serilog.Sinks.SystemConsole.Themes;

using Spectre.Console;

namespace ACUCustomizationUtils.Builders.Log;

/// <summary>
/// Styles of log event elements rendered by <see cref="AnsiConsoleFormatter"/>
/// </summary>
/// <param name="styles">Style per element. Elements left out are rendered with <see cref="Style.Plain"/></param>
public sealed class LogTheme(IReadOnlyDictionary<ConsoleThemeStyle, Style> styles)
{
    private readonly IReadOnlyDictionary<ConsoleThemeStyle, Style> _styles = styles ?? throw new ArgumentNullException(nameof(styles));

    /// <summary>
    /// Style of <paramref name="style"/> element, <see cref="Style.Plain"/> if the theme leaves it out
    /// </summary>
    public Style this[ConsoleThemeStyle style] => _styles.GetValueOrDefault(style, Style.Plain);

    /// <summary>
    /// <see cref="SystemConsoleTheme.Literate"/> theme for <see cref="AnsiConsole.Console"/>
    /// </summary>
    /// <remarks>
    /// Replaces <see cref="SystemConsoleThemeStyle"/> with <see cref="Style"/>; the rest is kept identical
    /// </remarks>
    public static LogTheme Literate { get; } = new(new Dictionary<ConsoleThemeStyle, Style>
    {
        [ConsoleThemeStyle.Text] = new(Color.White),
        [ConsoleThemeStyle.SecondaryText] = new(Color.Silver),
        [ConsoleThemeStyle.TertiaryText] = new(Color.Grey),
        [ConsoleThemeStyle.Invalid] = new(Color.Yellow),
        [ConsoleThemeStyle.Null] = new(Color.Blue),
        [ConsoleThemeStyle.Name] = new(Color.Silver),
        [ConsoleThemeStyle.String] = new(Color.Aqua),
        [ConsoleThemeStyle.Number] = new(Color.Fuchsia),
        [ConsoleThemeStyle.Boolean] = new(Color.Blue),
        [ConsoleThemeStyle.Scalar] = new(Color.Lime),
        [ConsoleThemeStyle.LevelVerbose] = new(Color.Silver),
        [ConsoleThemeStyle.LevelDebug] = new(Color.Silver),
        [ConsoleThemeStyle.LevelInformation] = new(Color.White),
        [ConsoleThemeStyle.LevelWarning] = new(Color.Yellow),
        [ConsoleThemeStyle.LevelError] = new(Color.White, Color.Red),
        [ConsoleThemeStyle.LevelFatal] = new(Color.White, Color.Red),
    });
}
