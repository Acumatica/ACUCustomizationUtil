namespace ACUCustomizationUtils.Helpers.CommonTypes;

public class CustomizationProjectEntity
{
    public int CompanyID { get; set; }
    public Guid ObjectID { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Label { get; set; }
    public Guid ProjectID { get; set; }
    public Guid? ProjectRevisionID { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
    public Guid? LastRevisionID { get; set; }
    public bool IsDisabled { get; set; }
    public Guid? ParentID { get; set; }
    public Guid CreatedByID { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public Guid LastModifiedByID { get; set; }
    public DateTime LastModifiedDateTime { get; set; }
    public Guid? NoteID { get; set; }
}