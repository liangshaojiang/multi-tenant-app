namespace MultiTenantApp.Core.Entities;

public class RolePermission
{
    public string Id { get; set; } = null!;
    public string RoleId { get; set; } = null!;
    public string PermissionId { get; set; } = null!;
    public string TenantId { get; set; } = null!;
}
