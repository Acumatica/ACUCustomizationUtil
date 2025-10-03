namespace ACUCustomizationUtils.Helpers.CommonTypes;

public class UploadFileRevision
{
    public int CompanyID { get; set; }
    public virtual Guid? FileID { get; set; }
    public virtual int? FileRevisionID { get; set; }
    public virtual byte[] Data { get; set; }
    public virtual int? Size { get; set; }
    public virtual Guid? CreatedByID { get; set; }
    public virtual DateTime? CreatedDateTime { get; set; }
    public virtual string Comment { get; set; }
    public virtual string OriginalName { get; set; }
    public virtual DateTime? OriginalTimestamp { get; set; }
    public Guid? BlobHandler { get; set; }
    public int RecordSourceID { get; set; }
    public bool IsDrawingLogCurrentFile { get; set; }
}

