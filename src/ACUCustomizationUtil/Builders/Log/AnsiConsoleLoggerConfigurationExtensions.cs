using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

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
    /// <returns>Target <see cref="LoggerConfiguration"/> for daisy chaining</returns>
    public static LoggerConfiguration AnsiConsole(
        this LoggerSinkConfiguration sinkConfiguration,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
        string outputTemplate = AnsiConsoleSink.DefaultOutputTemplate,
        IFormatProvider? formatProvider = null,
        LoggingLevelSwitch? levelSwitch = null
    )
    {
        ArgumentNullException.ThrowIfNull(outputTemplate);
        AnsiConsoleFormatter formatter = new(outputTemplate, formatProvider);

        ArgumentNullException.ThrowIfNull(sinkConfiguration);
        ArgumentNullException.ThrowIfNull(formatter);

        AnsiConsoleSink sink = new(formatter);

        return sinkConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
    }
}
