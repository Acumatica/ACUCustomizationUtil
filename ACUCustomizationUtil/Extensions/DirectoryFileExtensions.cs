using System.Text.RegularExpressions;

namespace ACUCustomizationUtils.Extensions;

public static class DirectoryFileExtensions
{
    public static void TryCheckCreateDirectory(this string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public static void TryCheckFileDirectory(this string? fullFileName)
    {
        if (fullFileName == null) return;
        var file = new FileInfo(fullFileName);
        file.DirectoryName?.TryCheckCreateDirectory();
    }

    public static string? TryGetFullDirectoryPath(this string? path)
    {
        if (path != null)
            return path.PathIsAbsolute() == false ? Path.Combine(Environment.CurrentDirectory, path) : path;
        return null;
    }

    private static bool PathIsAbsolute(this string? path)
    {
        var regex = new Regex("/^(?:[A-Za-z]:)?\\/");
        return path != null && regex.IsMatch(path);
    }
}