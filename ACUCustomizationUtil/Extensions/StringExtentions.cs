using Microsoft.Extensions.Logging;

namespace ACUCustomizationUtils.Extensions;

public static class StringExtensions
{
    public static string FirstCharToUpper(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };

    public static string GetProgressString(this string? processMessage)
    {
        const string emptyProgress = "Progress ##";
        if (processMessage == null) return emptyProgress;
        var index = processMessage.IndexOf("Progress", StringComparison.Ordinal);
        var length = processMessage.Length - index;
        var prn = processMessage.Substring(index, length).TrimEnd();

        return prn;
    }

    public static bool IsProgressString(this string? processMessage)
    {
        return processMessage != null && processMessage.Contains("Progress");
    }

    public static (LogLevel type, string message) GetLoggerString(this string processMessage)
    {
        var index = processMessage.StartsWith("   at") ? 0 : processMessage.IndexOf("]", StringComparison.Ordinal) + 1;
        var err = processMessage.IsErrorInfo();
        var length = processMessage.Length - index;
        var prn = processMessage.Substring(index, length).TrimStart().TrimEnd();

        return err ? (LogLevel.Error, prn) : (LogLevel.Information, prn);
    }

    public static bool IsLoggerString(this string? processMessage)
    {
        return processMessage != null &&
               (processMessage.StartsWith('[') && processMessage.Contains(']') ||
                processMessage.Contains("Exception") || processMessage.StartsWith("   at"));
    }

    public static bool IsErrorInfo(this string? processMessage)
    {
        return processMessage != null &&
               (processMessage.Contains("FAILED")
                || processMessage.Contains("ERR")
                || processMessage.Contains("Exception")
                || processMessage.StartsWith("   at"));
    }
}