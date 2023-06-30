using System.Diagnostics;
using ACUCustomizationUtils.Builders.Log;
using ACUCustomizationUtils.Extensions;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using ILogger = Serilog.ILogger;

namespace ACUCustomizationUtils.Helpers;

public class ProcessHelper
{
    private readonly string _file;
    private readonly string _arguments;
    private readonly StatusContext _ctx;
    private readonly ILogger _logger;

    public ProcessHelper(string file, string arguments, StatusContext ctx)
    {
        _file = file;
        _arguments = arguments;
        _ctx = ctx;
        _logger = SerilogBuilder.BuildSilent(typeof(ProcessHelper));
    }

    public async Task Execute()
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = _file,
            Arguments = _arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = new Process
        {
            StartInfo = processInfo
        };
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += (_, args) => { PrintProgressStatus(_ctx, args); };
        process.ErrorDataReceived += (_, args) =>
        {
            _logger.Error("{Error}", args.Data);
            PrintProgressStatus(_ctx, args);
        };
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();
    }

    private void PrintProgressStatus(StatusContext ctx, DataReceivedEventArgs args)
    {
        if (args.Data == null) return;
        var processMessage = args.Data;

        if (processMessage.IsProgressString())
        {
            var prn = processMessage.GetProgressString();
            ctx.Status(prn);
        }
        else if (processMessage.IsLoggerString())
        {
            AnsiConsole.WriteLine(processMessage);
            var (type, message) = processMessage.GetLoggerString();
            LogMessage(type, message);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(processMessage)) return;
            var message = SerilogBuilder.Format(processMessage);
            AnsiConsole.WriteLine(message);
            var type = processMessage.IsErrorInfo() ? LogLevel.Error : LogLevel.Information;
            LogMessage(type, processMessage);
        }
    }

    private void LogMessage(LogLevel type, string message)
    {
        if (type == LogLevel.Error)
            _logger.Error("{Error}", message);
        else
            _logger.Information("{Info}", message);
    }
}