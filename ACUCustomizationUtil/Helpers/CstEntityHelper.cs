using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Extensions;
using ACUCustomizationUtils.Helpers.CommonTypes;

namespace ACUCustomizationUtils.Helpers;

[SuppressMessage("GeneratedRegex", "SYSLIB1045:Convert to \'GeneratedRegexAttribute\'.")]
public class CstEntityHelper
{
    private readonly List<string> _handledDlls;

    private readonly string _packageSourceDir;
    private readonly string _packageSourceProjectDir;
    private readonly string _packageSourceBinDir;
    private readonly string _siteRootDir;
    private readonly string _packageName;

    public CstEntityHelper(IAcuConfiguration config)
    {
        _handledDlls = new List<string>();

        _packageSourceDir = config.Code.PkgSourceDirectory!;
        _packageSourceBinDir = Path.Combine(_packageSourceDir, "Bin");
        _packageSourceProjectDir = Path.Combine(_packageSourceDir, "_project");
        _siteRootDir = config.Site.SitePhysicalPath!;
        _packageName = config.Package.PackageName!;
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
                new XAttribute("description", projectEntity.Description!)));
        fileName.TryCheckFileDirectory();
        xDoc.Save(fileName);
    }

    public string? GetPackageFileVersion()
    {
        var dllFile = Directory.GetFiles(_packageSourceBinDir, $"{_packageName}.dll");
        FileVersionInfo? versionInfo = null;
        if (dllFile.Any())
        {
            versionInfo = FileVersionInfo.GetVersionInfo(dllFile.First());
        }

        return versionInfo?.FileVersion;
    }

    public string GetPackageDateVersion() => DateTime.Now.ToString("yyyy.mm.dd");

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
                if (fi.Extension.Contains("dll"))
                {
                    _handledDlls.Add(fi.Name);
                }
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

    public void SaveHandledDlls()
    {
        if (_handledDlls.Count <= 0) return;
        if (!Directory.Exists(_packageSourceBinDir))
        {
            throw new Exception($"Directory {_packageSourceBinDir} does not exists");
        }

        File.WriteAllLines(_packageSourceBinDir, _handledDlls);
    }

    #endregion Private methods
}