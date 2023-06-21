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

    public static void TryCheckFileDirectory(this string fullFileName)
    {
        var file = new FileInfo(fullFileName);
        file.DirectoryName?.TryCheckCreateDirectory();
    }
}