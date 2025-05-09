using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace ACUCustomizationUtils.Extensions;

public static class StringExtensions
{
    public static string FirstCharToUpper(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1)),
        };

    public static string GetProgressString(this string? processMessage)
    {
        const string emptyProgress = "Progress ##";
        if (processMessage == null)
            return emptyProgress;
        int index = processMessage.IndexOf("Progress", StringComparison.Ordinal);
        int length = processMessage.Length - index;
        string prn = processMessage.Substring(index, length).TrimEnd();

        return prn;
    }

    public static bool IsProgressString(this string? processMessage)
    {
        return processMessage != null && processMessage.Contains("Progress");
    }

    public static (LogLevel type, string message) GetLoggerString(this string processMessage)
    {
        int index = processMessage.StartsWith("   at")
            ? 0
            : processMessage.IndexOf(']') + 1;
        bool err = processMessage.IsErrorInfo();
        int length = processMessage.Length - index;
        string prn = processMessage.Substring(index, length).TrimStart().TrimEnd();

        return err ? (LogLevel.Error, prn) : (LogLevel.Information, prn);
    }

    public static bool IsLoggerString(this string? processMessage)
    {
        return processMessage != null
            && (
                processMessage.StartsWith('[') && processMessage.Contains(']')
                || processMessage.Contains("Exception")
                || processMessage.StartsWith("   at")
            );
    }

    public static bool IsErrorInfo(this string? processMessage)
    {
        return processMessage != null
            && (
                processMessage.Contains("FAILED")
                || processMessage.Contains("ERR")
                || processMessage.Contains("Exception")
                || processMessage.StartsWith("   at")
            );
    }

    public static string? NormalizeEnvVariables(this string? value)
    {
        const string pattern = @"\%+\w+\%";
        if (value != null && Regex.IsMatch(value, pattern))
        {
            string envName = Regex.Match(value, pattern).Value.Trim('%');
            string? envValue =
                Environment.GetEnvironmentVariable(envName, EnvironmentVariableTarget.User)
                ?? Environment.GetEnvironmentVariable(envName, EnvironmentVariableTarget.Machine);
            if (envValue != null)
            {
                value = Regex.Replace(value, pattern, envValue);
            }
            else
            {
                throw new ArgumentNullException(
                    $"System or User environment variable {envName} is not found!"
                );
            }
        }

        return value;
    }

    public static string EnsureTrailingSlash(this string input)
    {
        return input.EndsWith('/') ? input : input + '/';
    }
}
