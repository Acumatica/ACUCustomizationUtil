using ACUCustomizationUtils.Extensions;
using Serilog;
using Serilog.Events;

namespace ACUCustomizationUtils.Builders.Log;
/// <summary>
/// This class is the point of building an application Logger
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public static class SerilogBuilder
{
    public static void Build()
    {
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("acu-log.txt", rollingInterval: RollingInterval.Day, shared: true)
            .CreateLogger();
    }

    public static ILogger BuildSilent(Type contextType)
    {
        var logger = Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.File("acu-log.txt", rollingInterval: RollingInterval.Day, shared: true)
            .CreateLogger();

        return logger.ForContext(contextType);
    }

    public static string Format(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var error = message.IsErrorInfo();
        var level = error ? "ERR" : "INF";

        return $"[{timestamp} {level}] {message}";
    }
}