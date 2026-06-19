namespace MultiTenantApp.Core.Entities;

public class Permission : BaseEntity
{
    public string? ParentId { get; set; }
    public string PermissionName { get; set; } = null!;
    public string? PermissionCode { get; set; }
    public byte PermissionType { get; set; }
    public string? Path { get; set; }
    public string? Component { get; set; }
    public string? Icon { get; set; }
    public int Sort { get; set; }
    public bool Visible { get; set; }
    public bool Status { get; set; }
}
