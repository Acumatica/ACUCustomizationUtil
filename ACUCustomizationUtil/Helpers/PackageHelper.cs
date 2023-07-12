using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml;
using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Extensions;

namespace ACUCustomizationUtils.Helpers;

public class PackageHelper
{
    private readonly string _packageSourceDir;
    private readonly string _erpVersion;
    private readonly int _level;
    private string _packageFileName;
    private readonly string? _description;

    #region Public members

    public PackageHelper(IAcuConfiguration configuration)
    {
        _packageSourceDir = configuration.Code.PkgSourceDirectory!;
        _erpVersion = configuration.Erp.ErpVersion!;
        _level = int.TryParse(configuration.Code.PkgLevel, out var l) ? l : 0;
        var packageDestinationDir = configuration.Package.PackageDirectory!;
        var packageName = GetPackageName(configuration);
        _packageFileName = Path.Combine(packageDestinationDir, packageName);
        _description = configuration.Code.PkgDescription ?? GetPackageDescription(configuration);
    }
    
    public void MakePackage()
    {
        ValidateCustomizationPath(_packageSourceDir);
        ValidateProjectVersion(_erpVersion);
        ValidatePackagePath(_packageFileName);

        var projectXml = new XmlDocument();
        var customizationNode = projectXml.CreateElement("Customization");

        customizationNode.SetAttribute("level", _level.ToString());
        customizationNode.SetAttribute("description", _description);
        customizationNode.SetAttribute("product-version", _erpVersion);

        // Append all .xml files to project.xml
        var projectDir = Path.Combine(_packageSourceDir, "_project");
        if (new DirectoryInfo(projectDir).Exists)
            foreach (var file in Directory.GetFiles(Path.Combine(_packageSourceDir, "_project"), "*.xml"))
            {
                if (file.EndsWith("ProjectMetadata.xml")) continue;

                var currentFileXml = new XmlDocument();
                currentFileXml.Load(file);

                if (currentFileXml.DocumentElement == null) throw new Exception("project.xml empty");

                customizationNode.AppendChild(projectXml.ImportNode(currentFileXml.DocumentElement, true));
            }

        if (_packageFileName == null) throw new ArgumentNullException(nameof(_packageFileName));
        using FileStream zipFileStream = new(_packageFileName, FileMode.Create);
        using ZipArchive archive = new(zipFileStream, ZipArchiveMode.Create, true);
        AddFilesToZipArchive(_packageSourceDir, archive, customizationNode);

        projectXml.AppendChild(customizationNode);
        var projectFile = archive.CreateEntry("project.xml", CompressionLevel.Optimal);
        using StreamWriter streamWriter = new(projectFile.Open());
        projectXml.Save(streamWriter);
    }

    #endregion Public members

    #region Private members

    #region Validators

    private void ValidateCustomizationPath(string customizationPath)
    {
        var res = new DirectoryInfo(customizationPath).Exists;
        if (!res) throw new ArgumentException($"{customizationPath} do not found");
    }

    private void ValidateProjectVersion(string erpVersion)
    {
        var regex = new Regex("^\\d{2}\\.\\d{3}.\\d{4}$");
        var match = regex.IsMatch(erpVersion);
        if (!match) throw new ArgumentException("ERP Version should be in the form: 00.000.0000");
    }

    private void ValidatePackagePath(string? packageFilename)
    {
        packageFilename.TryCheckFileDirectory();
    }

    #endregion Validators

    private void AddFilesToZipArchive(string customizationPath, ZipArchive archive, XmlNode customizationNode,
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
            
            //Calculate relative parent directory name for the next level
            parentDir = parentDir != null ? Path.Combine(parentDir, new DirectoryInfo(directory).Name) : new DirectoryInfo(directory).Name;
            if (!directory.EndsWith(parentDir))
                parentDir = new DirectoryInfo(directory).Name;
            
            AddFilesToZipArchive(directory, archive, customizationNode, parentDir);
        }
    }

    private static string GetPackageName(IAcuConfiguration config)
    {
        var cstHelper = new CstEntityHelper(config);
        var fileVersion = cstHelper.GetPackageFileVersion();
        var dateVersion = CstEntityHelper.GetPackageDateVersion();
        var nawErpVersion = cstHelper.GetPackageNawErpVersion();
        var nawDateVersion = CstEntityHelper.GetPackageNawDateVersion();
        
        var packageName = config.Code.MakeMode switch
        {
            Messages.MakeModeBase => $"{config.Package.PackageName}.zip",
            Messages.MakeModeQA => $"{config.Package.PackageName}[{config.Erp.ErpVersion}][{fileVersion}].zip",
            Messages.MakeModeISV => $"{config.Package.PackageName}[{config.Erp.ErpVersion}][{dateVersion}].zip",
            Messages.MakeModeNAW => $"{config.Package.PackageName}{nawErpVersion}v{nawDateVersion}.zip",
            _ => $"{config.Package.PackageName}.zip"
        };

        return packageName;
    }

    private static string GetPackageDescription(IAcuConfiguration config)
    {
        return $"Release {config.Erp.ErpVersion} (build date: {DateTime.UtcNow.ToUniversalTime()})";
    }

    #endregion Private members
}