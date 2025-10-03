namespace ACUCustomizationUtils.Helpers.CommonTypes;

public class UploadFile
{
    public int CompanyID { get; set; }
    public Guid? FileID { get; set; }
    public string Name { get; set; }
    public Guid? CreatedByID { get; set; }
    public DateTime? CreatedDateTime { get; set; }
    public bool? Versioned { get; set; }
    public Guid? CheckedOutBy { get; set; }
    public string CheckedOutComment { get; set; }
    public int? LastRevisionID { get; set; }
    public Guid? PrimaryPageID { get; set; }
    public string PrimaryScreenID { get; set; }
    public bool? IsHidden { get; set; }
    public bool? Synchronizable { get; set; }
    public string SourceType { get; set; }
    public string SourceUri { get; set; }
    public string SourceLogin { get; set; }
    public string SourcePassword { get; set; }
    public bool? SourceIsFolder { get; set; }
    public string SourceMask { get; set; }
    public string SourceNamingFormat { get; set; }
    public DateTime? SourceLastExportDate { get; set; }
    public DateTime? SourceLastImportDate { get; set; }
    public bool? IsPublic { get; set; }
    public Guid? NoteID { get; set; }
    public byte[] CompanyMask { get; set; }
    public byte[] tstamp { get; set; }
    public int? RecordSourceID { get; set; }
    public string SshCertificateName { get; set; }
    public bool? IsSystem { get; set; }
    public bool? IsAccessRightsFromEntities { get; set; }
}

