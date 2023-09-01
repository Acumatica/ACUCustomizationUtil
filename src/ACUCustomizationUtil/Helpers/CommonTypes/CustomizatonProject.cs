namespace ACUCustomizationUtils.Helpers.CommonTypes;

public class CustomizationProject
{
    public int CompanyID { get; set; }
    public Guid ProjID { get; set; }
    public string? Name { get; set; }
    public bool IsWorking { get; set; }
    public string? Description { get; set; }
    public Guid CreatedByID { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public Guid LastModifiedByID { get; set; }
    public DateTime LastModifiedDateTime { get; set; }
    public Guid? ParentID { get; set; }
    public int? Level { get; set; }
    public Guid? SnapshotID { get; set; }
}