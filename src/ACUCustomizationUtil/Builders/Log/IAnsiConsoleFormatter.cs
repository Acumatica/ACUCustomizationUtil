using Serilog.Events;

using Spectre.Console;

namespace ACUCustomizationUtils.Builders.Log;

/// <summary>
/// Formats log events for <see cref="AnsiConsoleSink"/>
/// </summary>
public interface IAnsiConsoleFormatter
{
    /// <summary>
    /// Format the log event into the output.
    /// </summary>
    /// <param name="logEvent">The event to format.</param>
    /// <param name="output">The output.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="logEvent"/> is <code>null</code></exception>
    /// <exception cref="ArgumentNullException">When <paramref name="output"/> is <code>null</code></exception>
    void Format(LogEvent logEvent, Paragraph output);
}
