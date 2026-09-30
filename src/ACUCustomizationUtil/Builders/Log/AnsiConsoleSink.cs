using Serilog.Core;
using Serilog.Events;

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
    private readonly IAnsiConsoleFormatter _formatter;
    private volatile bool _detached;

    /// <summary>
    /// Instantiates new <see cref="AnsiConsoleSink"/>
    /// </summary>
    /// <param name="formatter">Formats log events in a textual representation</param>
    public AnsiConsoleSink(IAnsiConsoleFormatter formatter)
    {
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    /// <inheritdoc />
    public void Emit(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        if (_detached)
            return;

        Paragraph paragraph = new();
        _formatter.Format(logEvent, paragraph);

        AnsiConsole.Console.Write(paragraph);
    }

    /// <summary>
    /// Detaches the sink from the console: later events are dropped
    /// </summary>
    public void Dispose()
    {
        _detached = true;
    }
}
