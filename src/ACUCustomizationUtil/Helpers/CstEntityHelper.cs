using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
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
    private readonly string _siteRootDir;
    private readonly string? _erpVersion;
    private readonly string? _dllName;
    private readonly string? _versionFilePath;

    public CstEntityHelper(IAcuConfiguration config)
    {
        _packageSourceDir = config.Src.PkgSourceDirectory!;
        _packageSourceBinDir = Path.Combine(_packageSourceDir, "Bin");
        _packageSourceProjectDir = Path.Combine(_packageSourceDir, "_project");
        _siteRootDir = config.Site.InstancePath!;
        _erpVersion = config.Erp.ErpVersion;
        _dllName = config.Src.MsBuildAssemblyName;
        _versionFilePath = config.Src.AssemblyInfoPath;
    }

    #region Public methods

    public void HandleCustomizationsEntity(CustomizationProjectEntity entity)
    {
        if (entity.Type == "File")
        {
            HandleFileEntity(entity);
        }
        else
        {
            HandleContentEntity(entity);
        }
    }

    public void ClearProjectDirectory()
    {
        var directoryInfo = new DirectoryInfo(_packageSourceDir);
        var childDirs = directoryInfo.GetDirectories();
        var childFiles = directoryInfo.GetFiles();

        foreach (var file in childFiles)
        {
            file.Delete();
        }

        foreach (var dir in childDirs)
        {
            dir.Delete(true);
        }
    }

    public void SaveProjectMetadata(CustomizationProject projectEntity)
    {
        var fileName = Path.Combine(_packageSourceProjectDir, "ProjectMetadata.xml");
        var xDoc = new XDocument(
            new XElement("project",
                new XAttribute("name", projectEntity.Name!),
                new XAttribute("level", projectEntity.Level.GetValueOrDefault()),
                new XAttribute("description", projectEntity.Description ?? string.Empty)));
        fileName.TryCheckFileDirectory();
        xDoc.Save(fileName);
    }

    public string? GetPackageFileVersion()
    {
        if (File.Exists(_versionFilePath))
        {
            var versionContent = File.ReadAllText(_versionFilePath);
            var version = ExtractVersion(versionContent) ?? throw new Exception($"Version.cs file does not contain a valid version");
            var versionParts = version.Split('.');
            if (versionParts.Length != 4)
                throw new Exception($"Version.cs file does not contain a correct version format: {version}");

            return $"{versionParts[2]}.{versionParts[3]}";
        }

        var dllPkgFiles = Directory.GetFiles(_packageSourceBinDir, _dllName
                                                                   ?? throw new InvalidOperationException("Customization dll name MUST be configured"));
        var dllAnyFiles = Directory.GetFiles(_packageSourceBinDir, $"*.dll");
        var dllFile = dllPkgFiles.Any() ? dllPkgFiles.First() : dllAnyFiles.Any() ? dllAnyFiles.First() : null;
        if (dllFile == null) throw new Exception($"Assembly (dll) file for customization not found");
        var fv = FileVersionInfo.GetVersionInfo(dllFile).FileVersion;
        if (fv == null || fv.Split('.').Length != 4)
            throw new Exception($"Assembly (dll) file for customization does not contain correct version: {fv ?? "version is null"}");
        var fvArr = fv.Split('.');
        return $"{fvArr[2]}.{fvArr[3]}";
    }

    public string GetPackageDateVersion() => DateTime.Now.ToString("yyyy.MM.dd");

    #endregion Public methods

    #region Private methods

    private void HandleContentEntity(CustomizationProjectEntity entity)
    {
        try
        {
            var entityName = Regex.Replace(entity.Name!, "\\W", "_").Trim('_') + ".xml";
            var entityPath = Path.Combine(_packageSourceProjectDir, entityName);
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
        var fileFullName = entity.Name!.Replace("File#", "");
        var sourcePagePath = Path.Combine(_siteRootDir, fileFullName);
        var destinationPath = Path.Combine(_packageSourceDir, fileFullName);
        try
        {
            destinationPath.TryCheckFileDirectory();
            var fi = new FileInfo(sourcePagePath);
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
            throw new Exception($"Error copy entity {entity.Name} from {sourcePagePath} to {destinationPath}", e);
        }
    }

    private static string? ExtractVersion(string content)
    {
        var match = Regex.Match(content, @"\[assembly:\s*AssemblyVersion\(""([^""]+)""\)\]");
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