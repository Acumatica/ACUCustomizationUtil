using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Extensions;

namespace ACUCustomizationUtils.Helpers;

[SuppressMessage("GeneratedRegex", "SYSLIB1045:Convert to \'GeneratedRegexAttribute\'.")]
[SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
public static class PackageHelper
{
    #region Public members

    public static void MakePackage(string customizationPath, string? packageFilename, string erpVersion,
        string? description, int? level)
    {
        ValidateCustomizationPath(customizationPath);
        ValidateProjectVersion(erpVersion);
        ValidatePackagePath(packageFilename);

        var projectXml = new XmlDocument();
        var customizationNode = projectXml.CreateElement("Customization");

        customizationNode.SetAttribute("level", level.ToString());
        customizationNode.SetAttribute("description", description);
        customizationNode.SetAttribute("product-version", erpVersion);

        // Append all .xml files to project.xml
        var projectDir = Path.Combine(customizationPath, "_project");
        if (new DirectoryInfo(projectDir).Exists)
            foreach (var file in Directory.GetFiles(Path.Combine(customizationPath, "_project"), "*.xml"))
            {
                if (file.EndsWith("ProjectMetadata.xml")) continue;

                var currentFileXml = new XmlDocument();
                currentFileXml.Load(file);

                if (currentFileXml.DocumentElement == null) throw new Exception("project.xml empty");

                customizationNode.AppendChild(projectXml.ImportNode(currentFileXml.DocumentElement, true));
            }

        if (packageFilename == null) throw new ArgumentNullException(nameof(packageFilename));
        using FileStream zipFileStream = new(packageFilename, FileMode.Create);
        using ZipArchive archive = new(zipFileStream, ZipArchiveMode.Create, true);
        AddFilesToZipArchive(customizationPath, archive, customizationNode);

        projectXml.AppendChild(customizationNode);
        var projectFile = archive.CreateEntry("project.xml", CompressionLevel.Optimal);
        using StreamWriter streamWriter = new(projectFile.Open());
        projectXml.Save(streamWriter);
    }

    #endregion Public members

    #region Private members

    #region Validators

    private static void ValidateCustomizationPath(string customizationPath)
    {
        var res = new DirectoryInfo(customizationPath).Exists;
        if (!res) throw new ArgumentException($"{customizationPath} do not found");
    }

    private static void ValidateProjectVersion(string erpVersion)
    {
        var regex = new Regex("^\\d{2}\\.\\d{3}.\\d{4}$");
        var match = regex.IsMatch(erpVersion);
        if (!match) throw new ArgumentException("ERP Version should be in the form: 00.000.0000");
    }

    private static void ValidatePackagePath(string? packageFilename)
    {
        packageFilename.TryCheckFileDirectory();
    }

    #endregion Validators

    private static void AddFilesToZipArchive(string customizationPath, ZipArchive archive, XmlNode customizationNode,
        string? parentDir = null)
    {
        foreach (var directory in Directory.GetDirectories(customizationPath))
        {
            if (directory.EndsWith(@"\_project")) continue;
            foreach (var file in Directory.GetFiles(directory))
            {
                if (file.EndsWith("bin.config")) continue;

                var fileInfo = new FileInfo(file);
                var dirInfo = fileInfo.Directory;
                var arcFileName = Path.Combine(parentDir ?? string.Empty, dirInfo!.Name, fileInfo.Name);
                archive.CreateEntryFromFile(file, arcFileName, CompressionLevel.Optimal);

                //Add reference to customization project as well
                var fileElement = customizationNode.OwnerDocument!.CreateElement("File");
                fileElement.SetAttribute("AppRelativePath", arcFileName);
                customizationNode.AppendChild(fileElement);
            }

            AddFilesToZipArchive(directory, archive, customizationNode, new DirectoryInfo(directory).Name);
        }
    }

    internal static string GetPackageName(IAcuConfiguration config)
    {
        var cstHelper = new CstEntityHelper(config);
        var fileVersion = cstHelper.GetPackageFileVersion();
        var dateVersion = cstHelper.GetPackageDateVersion();

        var qa = config.Code.MakeQA;
        var isv = config.Code.MakeISV;

        if (isv == true) return $"{config.Package.PackageName}[{config.Erp.ErpVersion}][{dateVersion}].zip";
        if (qa == true) return $"{config.Package.PackageName}[{config.Erp.ErpVersion}][{fileVersion}].zip";
        return $"{config.Package.PackageName}.zip";
    }

    internal static string? GetPackageDescription(IAcuConfiguration config)
    {
        return $"Release {config.Erp.ErpVersion} (build date: {DateTime.UtcNow.ToUniversalTime()})";
    }

    #endregion Private members
}