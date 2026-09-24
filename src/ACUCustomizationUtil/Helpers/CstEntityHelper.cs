using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Extensions;
using ACUCustomizationUtils.Helpers.CommonTypes;

namespace ACUCustomizationUtils.Helpers;

public class CstEntityHelper
{
    private readonly string _packageSourceDir;
    private readonly string _packageSourceProjectDir;
    private readonly string _packageSourceBinDir;
    private readonly string _packageFrontEndSourceDir;
    private readonly string _siteRootDir;
    private readonly string? _erpVersion;
    private readonly string? _dllName;
    private readonly string? _versionFilePath;
    private readonly string? _pkgVersion;
    public static readonly string FrontEndSourceRelativePath = "FrontendSources\\screen\\src\\development\\";

    public CstEntityHelper(IAcuConfiguration config)
    {
        _packageSourceDir = config.Src.PkgSourceDirectory!;
        _packageSourceBinDir = Path.Combine(_packageSourceDir, "Bin");
        _packageSourceProjectDir = Path.Combine(_packageSourceDir, "_project");
        _packageFrontEndSourceDir = Path.Combine(_packageSourceDir, FrontEndSourceRelativePath);
        _siteRootDir = config.Site.InstancePath!;
        _erpVersion = config.Erp.ErpVersion;
        _dllName = config.Src.MsBuildAssemblyName;
        _versionFilePath = config.Src.AssemblyInfoPath;
        _pkgVersion = config.Src.PkgVersion;
    }

    #region Public methods

    public void HandleCustomizationsEntity(CustomizationProjectEntity entity, DatabaseHelper dataHelper)
    {
        switch (entity.Type)
        {
            case "File":
                HandleFileEntity(entity);
                break;
            case "PerTenantFile":
                HandlePerTenantFileEntity(entity, dataHelper);
                break;
            default:
                HandleContentEntity(entity);
                break;
        }
    }

    public void ClearProjectDirectory()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(_packageSourceDir);
        DirectoryInfo[] childDirs = directoryInfo.GetDirectories();
        FileInfo[] childFiles = directoryInfo.GetFiles();

        foreach (FileInfo file in childFiles)
        {
            file.Delete();
        }

        foreach (DirectoryInfo dir in childDirs)
        {
            dir.Delete(true);
        }
    }

    public void SaveProjectMetadata(CustomizationProject projectEntity)
    {
        string fileName = Path.Combine(_packageSourceProjectDir, "ProjectMetadata.xml");
        XDocument xDoc = new XDocument(
            new XElement(
                "project",
                new XAttribute("name", projectEntity.Name!),
                new XAttribute("level", projectEntity.Level.GetValueOrDefault()),
                new XAttribute("description", projectEntity.Description ?? string.Empty)
            )
        );
        fileName.TryCheckFileDirectory();
        xDoc.Save(fileName);
    }

    /// <summary>
    /// Resolve package version component ("yyDDD.HHmm") in order:
    /// configuration (pkgVersion), AssemblyInfo file, customization assembly (dll) in package Bin directory.
    /// If none of them is available (customization without external library), the version is generated
    /// from the current date in the same format as for the external library build.
    /// </summary>
    public PackageVersion GetPackageVersion()
    {
        if (!string.IsNullOrWhiteSpace(_pkgVersion))
            return new PackageVersion(GetLastTwoSegments(_pkgVersion), PackageVersionSource.Configuration);

        if (File.Exists(_versionFilePath))
            return new PackageVersion(GetAssemblyInfoVersion(_versionFilePath), PackageVersionSource.AssemblyInfo);

        string? dllFile = FindPackageAssemblyFile();
        if (dllFile != null)
            return new PackageVersion(GetAssemblyFileVersion(dllFile), PackageVersionSource.Assembly);

        return new PackageVersion(MetaDataHelper.GetDateVersion(), PackageVersionSource.Generated);
    }
    #endregion Public methods

    #region Private methods

    private void HandleContentEntity(CustomizationProjectEntity entity)
    {
        try
        {
            string entityName = Regex.Replace(entity.Name!, "\\W", "_").Trim('_') + ".xml";
            string entityPath = Path.Combine(_packageSourceProjectDir, entityName);
            entityPath.TryCheckFileDirectory();
            File.WriteAllText(entityPath, entity.Content);
        }
        catch (Exception e)
        {
            throw new Exception($"Error write package entity source {entity.Name}", e);
        }
    }
 
    private void HandleFileEntity(CustomizationProjectEntity entity)
    {
        string fileFullName = entity.Name!.Replace("File#", "");
        string sourcePagePath = Path.Combine(_siteRootDir, fileFullName);
        string destinationPath = Path.Combine(_packageSourceDir, fileFullName);
        try
        {
            destinationPath.TryCheckFileDirectory();
            FileInfo fi = new FileInfo(sourcePagePath);
            if (fi.Exists)
            {
                File.Copy(sourcePagePath, destinationPath, true);
            }
            else
            {
                throw new Exception($"File {sourcePagePath} does not exists");
            }
        }
        catch (Exception e)
        {
            throw new Exception(
                $"Error copy entity {entity.Name} from {sourcePagePath} to {destinationPath}",
                e
            );
        }
    }
    private void HandlePerTenantFileEntity(CustomizationProjectEntity entity, DatabaseHelper dataHelper)
    {
        var fileData = GetPerTenantFileAsync(entity, dataHelper).GetAwaiter().GetResult();

        string fileFullName = entity.Name!.Replace("PerTenantFile#", "");
        string destinationPath = Path.Combine(_packageFrontEndSourceDir, fileFullName);
        try
        {
            destinationPath.TryCheckFileDirectory();
            File.WriteAllBytes(destinationPath, fileData);
        }
        catch (Exception e)
        {
            throw new Exception($"Error copy entity {entity.Name} to {destinationPath}", e);
        }
    }

    private async Task<byte[]> GetPerTenantFileAsync(CustomizationProjectEntity entity, DatabaseHelper dataHelper)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(entity.Content);
        var fileID = xmlDoc.DocumentElement?.GetAttribute("FileID");

        var uploadFile = await dataHelper.GetUploadFile(fileID);
        if (uploadFile == null)
            throw new Exception($"File with ID={fileID} not found in UploadFile table");

        var getFile = await dataHelper.GetUploadFileRevision(fileID, uploadFile.LastRevisionID);
        if (getFile == null)
            throw new Exception($"File with ID={fileID} not found in UploadFileRevision table");

        return getFile.Data;
    }

    private static string GetAssemblyInfoVersion(string versionFilePath)
    {
        string versionContent = File.ReadAllText(versionFilePath);
        string version =
            ExtractVersion(versionContent)
            ?? throw new Exception($"Version.cs file does not contain a valid version");
        if (version.Split('.').Length != 4)
            throw new Exception(
                $"Version.cs file does not contain a correct version format: {version}"
            );

        return GetLastTwoSegments(version);
    }

    private string? FindPackageAssemblyFile()
    {
        if (!Directory.Exists(_packageSourceBinDir))
            return null;

        string[] dllPkgFiles = string.IsNullOrEmpty(_dllName)
            ? []
            : Directory.GetFiles(_packageSourceBinDir, _dllName);
        string[] dllAnyFiles = Directory.GetFiles(_packageSourceBinDir, "*.dll");

        return dllPkgFiles.Length > 0 ? dllPkgFiles.First()
            : dllAnyFiles.Length > 0 ? dllAnyFiles.First()
            : null;
    }

    private static string GetAssemblyFileVersion(string dllFile)
    {
        string? fv = FileVersionInfo.GetVersionInfo(dllFile).FileVersion;
        if (fv == null || fv.Split('.').Length != 4)
            throw new Exception(
                $"Assembly (dll) file for customization does not contain correct version: {fv ?? "version is null"}"
            );

        return GetLastTwoSegments(fv);
    }

    private static string GetLastTwoSegments(string version)
    {
        string[] versionParts = version.Split('.');
        return versionParts.Length > 2
            ? $"{versionParts[^2]}.{versionParts[^1]}"
            : version;
    }

    private static string? ExtractVersion(string content)
    {
        Match match = Regex.Match(content, @"\[assembly:\s*AssemblyVersion\(""([^""]+)""\)\]");
        if (match is { Success: true, Groups.Count: > 1 })
        {
            return match.Groups[1].Value;
        }

        match = Regex.Match(content, @"\[assembly:\s*AssemblyFileVersion\(""([^""]+)""\)\]");
        if (match is { Success: true, Groups.Count: > 1 })
        {
            return match.Groups[1].Value;
        }

        return null;
    }

    #endregion Private methods
}
