using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace ACUCustomizationUtils.Builders.Log;

/// <summary>
/// Helpers to convert various entities into matching <see cref="ConsoleThemeStyle"/>
/// </summary>
public static class ConsoleThemeStyleExtensions
{
    /// <summary>
    /// Returns <see cref="ConsoleThemeStyle"/> fitting for this
    /// </summary>
    /// <param name="obj">Object to style</param>
    /// <returns>Matching <see cref="ConsoleThemeStyle"/></returns>
    public static ConsoleThemeStyle ToThemeStyle(this object? obj)
    {
        return obj switch
        {
            null    => ConsoleThemeStyle.Null,
            string  => ConsoleThemeStyle.String,
            bool    => ConsoleThemeStyle.Boolean,
            int or uint or long or ulong or decimal or byte or sbyte or short or ushort or float or double 
                    => ConsoleThemeStyle.Number,
            _       => ConsoleThemeStyle.Scalar,
        };
    }

    /// <summary>
    /// Returns <see cref="ConsoleThemeStyle"/> fitting for this
    /// </summary>
    /// <param name="level">Object to style</param>
    /// <returns>Matching <see cref="ConsoleThemeStyle"/></returns>
    public static ConsoleThemeStyle ToThemeStyle(this LogEventLevel level)
    {
        return level switch
        {
            LogEventLevel.Verbose       => ConsoleThemeStyle.LevelVerbose,
            LogEventLevel.Debug         => ConsoleThemeStyle.LevelDebug,
            LogEventLevel.Information   => ConsoleThemeStyle.LevelInformation,
            LogEventLevel.Warning       => ConsoleThemeStyle.LevelWarning,
            LogEventLevel.Error         => ConsoleThemeStyle.LevelError,
            _                           => ConsoleThemeStyle.LevelFatal,
        };
    }

}