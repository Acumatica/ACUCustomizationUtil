using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

using Spectre.Console;

namespace ACUCustomizationUtils.Builders.Log;

/// <summary>
/// Extends <see cref="LoggerConfiguration"/> with methods to add <see cref="AnsiConsole.Console" /> sinks
/// </summary>
public static class AnsiConsoleLoggerConfigurationExtensions
{
    /// <summary>
    /// Writes log events through <see cref="AnsiConsole.Console"/>, to avoid visual artifacts during parallel writes
    /// from other threads
    /// </summary>
    /// <param name="sinkConfiguration">Logger sink configuration</param>
    /// <param name="restrictedToMinimumLevel">The minimum level for events passed through the sink</param>
    /// <param name="outputTemplate">Message template describing the output format</param>
    /// <param name="formatProvider">Supplies culture-specific formatting information, or null</param>
    /// <param name="levelSwitch">A switch to change level at runtime.</param>
    /// <param name="levelStyle">Style to apply. If null, <see cref="AnsiConsoleSink.DefaultLevelStyle"/> is used</param>
    /// <returns>Target <see cref="LoggerConfiguration"/> for daisy chaining</returns>
    public static LoggerConfiguration AnsiConsole(
        this LoggerSinkConfiguration sinkConfiguration,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
        string outputTemplate = AnsiConsoleSink.DefaultOutputTemplate,
        IFormatProvider? formatProvider = null,
        LoggingLevelSwitch? levelSwitch = null,
        Func<LogEventLevel, Style>? levelStyle = null
    )
    {
        ArgumentNullException.ThrowIfNull(outputTemplate);
        MessageTemplateTextFormatter formatter = new(outputTemplate, formatProvider);

        return sinkConfiguration.AnsiConsole(
            formatter,
            restrictedToMinimumLevel,
            levelSwitch,
            levelStyle
        );
    }

    /// <summary>
    /// Write log events through <see cref="AnsiConsole.Console" />
    /// </summary>
    /// <param name="sinkConfiguration">Logger sink configuration</param>
    /// <param name="formatter">Formats log events in a textual representation</param>
    /// <param name="restrictedToMinimumLevel">The minimum level for events passed through the sink</param>
    /// <param name="levelSwitch">A switch to change level at runtime</param>
    /// <param name="levelStyle">Style to apply. If null, <see cref="AnsiConsoleSink.DefaultLevelStyle"/> is used</param>
    /// <returns>Target <see cref="LoggerConfiguration"/> for daisy chaining</returns>
    public static LoggerConfiguration AnsiConsole(
        this LoggerSinkConfiguration sinkConfiguration,
        ITextFormatter formatter,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
        LoggingLevelSwitch? levelSwitch = null,
        Func<LogEventLevel, Style>? levelStyle = null
    )
    {
        ArgumentNullException.ThrowIfNull(sinkConfiguration);
        ArgumentNullException.ThrowIfNull(formatter);

        AnsiConsoleSink sink = new(formatter, levelStyle);

        return sinkConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
    }
}
