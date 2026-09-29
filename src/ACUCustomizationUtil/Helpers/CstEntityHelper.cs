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
    private readonly string _packageFrontEndSourceDir;
    private readonly string _siteRootDir;
    public static readonly string FrontEndSourceRelativePath = "FrontendSources\\screen\\src\\development\\";

    public CstEntityHelper(IAcuConfiguration config)
    {
        _packageSourceDir = config.Src.PkgSourceDirectory!;
        _packageSourceProjectDir = Path.Combine(_packageSourceDir, "_project");
        _packageFrontEndSourceDir = Path.Combine(_packageSourceDir, FrontEndSourceRelativePath);
        _siteRootDir = config.Site.InstancePath!;
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
    #endregion Private methods
}
