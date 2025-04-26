using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Extensions;

namespace ACUCustomizationUtils.Helpers;

public class PackageHelper
{
    private const string ProjectXMLFilename = "project.xml";
    private readonly string _packageSourceDir;
    private readonly string _erpVersion;
    private readonly int _level;
    private readonly string _packageFileName;
    private readonly string? _description;
    private readonly MetaDataHelper _metaDataHelper;

    #region Public members

    public PackageHelper(IAcuConfiguration configuration)
    {
        _packageSourceDir = configuration.Src.PkgSourceDirectory!;
        _erpVersion = configuration.Erp.ErpVersion!;
        _level = int.TryParse(configuration.Src.PkgLevel, out var l) ? l : 0;
        var packageDestinationDir = configuration.Pkg.PkgDirectory!;
        var packageName = GetPackageName(configuration);
        _packageFileName = Path.Combine(packageDestinationDir, packageName);
        _description = configuration.Src.PkgDescription ?? GetPackageDescription(configuration);
        _metaDataHelper = new MetaDataHelper(configuration);
    }

    public void MakePackage()
    {
        ValidateCustomizationPath(_packageSourceDir);
        ValidateProjectVersion(_erpVersion);
        ValidatePackagePath(_packageFileName);

        //Create project.xml file
        var projectXml = new XmlDocument();
        var customizationNode = projectXml.CreateElement("Customization");

        customizationNode.SetAttribute("level", _level.ToString());
        customizationNode.SetAttribute("description", _description);
        customizationNode.SetAttribute("product-version", _erpVersion);

        // Append all .xml files to project.xml
        var projectDir = Path.Combine(_packageSourceDir, "_project");
        if (Directory.Exists(projectDir))
        {
            foreach (var file in Directory.GetFiles(projectDir, "*.xml"))
            {
                if (file.EndsWith("ProjectMetadata.xml"))
                    continue;

                var currentFileXml = new XmlDocument();
                currentFileXml.Load(file);
                if (currentFileXml.DocumentElement == null)
                    throw new Exception("project.xml empty");
                customizationNode.AppendChild(
                    projectXml.ImportNode(currentFileXml.DocumentElement, true)
                );
            }
        }

        projectXml.AppendChild(customizationNode);
        projectXml.Save(ProjectXMLFilename);

        //Create the zip file for package
        using (var fileStream = new FileStream(_packageFileName, FileMode.Create))
        using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create))
        {
            //Add all files from package source directory to the archive
            AddFilesToZipArchive(_packageSourceDir, archive, customizationNode);

            //Add project.xml to the archive
            archive.CreateEntryFromFile(ProjectXMLFilename, Path.GetFileName(ProjectXMLFilename));

            // Add metadata file to the archive
            File.WriteAllText(
                MetaDataHelper.MetadataFileName,
                _metaDataHelper.CreateMetadataJson()
            );
            archive.CreateEntryFromFile(
                MetaDataHelper.MetadataFileName,
                Path.GetFileName(MetaDataHelper.MetadataFileName)
            );
        }

        File.Delete(ProjectXMLFilename);
        File.Delete(MetaDataHelper.MetadataFileName);
    }

    #endregion Public members

    #region Private members

    #region Validators

    private void ValidateCustomizationPath(string customizationPath)
    {
        var res = new DirectoryInfo(customizationPath).Exists;
        if (!res)
            throw new ArgumentException($"{customizationPath} do not found");
    }

    private void ValidateProjectVersion(string erpVersion)
    {
        var regex = new Regex("^\\d{2}\\.\\d{3}.\\d{4}$");
        var match = regex.IsMatch(erpVersion);
        if (!match)
            throw new ArgumentException("ERP Version should be in the form: 00.000.0000");
    }

    private void ValidatePackagePath(string? packageFileName)
    {
        ArgumentNullException.ThrowIfNull(packageFileName);
        packageFileName.TryCheckFileDirectory();
    }

    #endregion

    #region Add files to zip archive
    private void AddFilesToZipArchive(string path, ZipArchive archive, XmlNode customizationNode)
    {
        if (File.Exists(path))
        {
            // Single file
            ProcessFile(path);
        }
        else if (Directory.Exists(path))
        {
            // Directory
            ProcessDirectory(path);
        }
        else
        {
            throw new ArgumentException($"{path} is not a valid file or directory.");
        }

        // Recursive processing of directories
        void ProcessDirectory(string targetDirectory)
        {
            if (
                targetDirectory.EndsWith(
                    Path.Combine(Path.DirectorySeparatorChar.ToString(), "_project")
                )
            )
                return;

            foreach (var fileName in Directory.GetFiles(targetDirectory))
            {
                ProcessFile(fileName);
            }

            foreach (var subdirectory in Directory.GetDirectories(targetDirectory))
            {
                ProcessDirectory(subdirectory);
            }
        }

        // Processing of individual files
        void ProcessFile(string file)
        {
            if (file.EndsWith("bin.config", StringComparison.OrdinalIgnoreCase))
                return;

            var fileInfo = new FileInfo(file);

            // Get the relative path inside the archive
            string arcDir = string.Empty;
            if (
                fileInfo.Directory != null
                && fileInfo.Directory.FullName.StartsWith(_packageSourceDir)
            )
            {
                arcDir = fileInfo
                    .Directory.FullName.Substring(_packageSourceDir.Length)
                    .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }

            var arcFileName = Path.Combine(arcDir, fileInfo.Name).Replace('\\', '/');

            // Add the file to the archive
            archive.CreateEntryFromFile(file, arcFileName);

            // Add a reference to the XML
            var fileElement = customizationNode.OwnerDocument!.CreateElement("File");
            fileElement.SetAttribute("AppRelativePath", arcFileName);
            customizationNode.AppendChild(fileElement);
        }
    }

    private static string GetPackageName(IAcuConfiguration config)
    {
        var cstHelper = new CstEntityHelper(config);
        var fileVersion = cstHelper.GetPackageFileVersion();
        var dateVersion = cstHelper.GetPackageDateVersion();
        var makeMode = config.Src.MakeMode ?? Messages.MakeModeBase;

        var packageName = makeMode switch
        {
            Messages.MakeModeBase => $"{config.Pkg.PkgName}.zip",
            Messages.MakeModeQA =>
                $"{config.Pkg.PkgName}[{config.Erp.ErpVersion}][{fileVersion}].zip",
            Messages.MakeModeISV =>
                $"{config.Pkg.PkgName}[{config.Erp.ErpVersion}][{dateVersion}].zip",
            _ => $"{config.Pkg.PkgName}.zip",
        };

        return packageName;
    }

    private static string GetPackageDescription(IAcuConfiguration config)
    {
        return $"Release {config.Erp.ErpVersion} (build date: {DateTime.UtcNow.ToUniversalTime()})";
    }

    #endregion Add files to zip archive
    #endregion Private members
}
