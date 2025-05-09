using ACUCustomizationUtils.Builders.Log;
using ACUCustomizationUtils.Extensions;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System.Diagnostics;
using ILogger = Serilog.ILogger;

namespace ACUCustomizationUtils.Helpers;

public class ProcessHelper
{
    private readonly string _file;
    private readonly string _arguments;
    private readonly StatusContext _ctx;
    private readonly ILogger _logger;
    private readonly TaskCompletionSource _tcs;

    public ProcessHelper(string file, string arguments, StatusContext ctx)
    {
        _file = file;
        _arguments = arguments;
        _ctx = ctx;
        _logger = SerilogBuilder.BuildSilent(typeof(ProcessHelper));
        _tcs = new TaskCompletionSource();
    }

    public Task Execute()
    {
        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = _file,
            Arguments = _arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        using Process process = new Process { StartInfo = processInfo };
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += (_, args) =>
        {
            PrintProgressStatus(_ctx, args);
        };
        process.ErrorDataReceived += (_, args) =>
        {
            if (string.IsNullOrWhiteSpace(args.Data))
                return;
            _logger.Error("{Error}", args.Data);
            PrintProgressStatus(_ctx, args);
        };

        process.Exited += (sender, _) =>
        {
            Process? proc = sender as Process;
            if (proc?.ExitCode != 0)
            {
                _tcs.SetException(new Exception($"Process executed with code {proc?.ExitCode}"));
            }
            else
            {
                _tcs.SetResult();
            }
        };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
        catch (Exception e)
        {
            _tcs.SetException(e);
        }

        return _tcs.Task;
    }

    private void PrintProgressStatus(StatusContext ctx, DataReceivedEventArgs args)
    {
        if (args.Data == null)
            return;
        string processMessage = args.Data;

        if (processMessage.IsProgressString())
        {
            string prn = processMessage.GetProgressString();
            ctx.Status(prn);
        }
        else if (processMessage.IsLoggerString())
        {
            AnsiConsole.WriteLine(processMessage);
            (LogLevel type, string message) = processMessage.GetLoggerString();
            LogMessage(type, message);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(processMessage))
                return;
            string message = SerilogBuilder.Format(processMessage);
            AnsiConsole.WriteLine(message);
            LogLevel type = processMessage.IsErrorInfo() ? LogLevel.Error : LogLevel.Information;
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
