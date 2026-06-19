namespace MultiTenantApp.Core.Entities;

public class UserRole
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string RoleId { get; set; } = null!;
    public string TenantId { get; set; } = null!;
}
