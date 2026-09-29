using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

using Spectre.Console;

namespace ACUCustomizationUtils.Builders.Log;

/// <summary>
/// Serilog sink that writes log events through <see cref="IAnsiConsole"/>
/// </summary>
/// <remarks>
/// Writes that go through the console render pipeline are seen by live displays
/// (<c>AnsiConsole.Status()</c>, <c>Progress()</c>, <c>Live()</c>): the display moves the cursor above
/// its region, prints the event and redraws the region below it. Direct <see cref="Console.Out"/>
/// writes bypass the pipeline and get overwritten by the next redraw of the live region
/// </remarks>
public sealed class AnsiConsoleSink : ILogEventSink, IDisposable
{
    public const string DefaultOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

    public bool IsDetached => _detached;
    private readonly ITextFormatter _formatter;
    private readonly Func<LogEventLevel, Style> _levelStyle;
    private volatile bool _detached;

    /// <summary>
    /// Instantiates new <see cref="AnsiConsoleSink"/>
    /// </summary>
    /// <param name="formatter">Formats log events in a textual representation</param>
    /// <param name="levelStyle">Style to apply. If null, <see cref="DefaultLevelStyle"/> is used</param>
    public AnsiConsoleSink(ITextFormatter formatter, Func<LogEventLevel, Style>? levelStyle = null
    )
    {
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        _levelStyle = levelStyle ?? DefaultLevelStyle;
    }

    /// <inheritdoc />
    public void Emit(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        if (_detached)
            return;

        using StringWriter writer = new();
        _formatter.Format(logEvent, writer);

        AnsiConsole.Console.Write(new Text(writer.ToString(), _levelStyle(logEvent.Level)));
    }

    /// <summary>
    /// Detaches the sink from the console: later events are dropped
    /// </summary>
    public void Dispose()
    {
        _detached = true;
    }

    /// <summary>
    /// Provides default styling per <paramref name="level"/>
    /// </summary>
    /// <param name="level">Event level</param>
    public static Style DefaultLevelStyle(LogEventLevel level)
    {
        return level switch
        {
            LogEventLevel.Verbose or LogEventLevel.Debug => new Style(Color.Grey),
            LogEventLevel.Warning => new Style(Color.Yellow),
            LogEventLevel.Error => new Style(Color.Red),
            LogEventLevel.Fatal => new Style(Color.Red, decoration: Decoration.Bold),
            _ => Style.Plain,
        };
    }
}
