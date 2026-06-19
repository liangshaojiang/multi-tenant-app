namespace MultiTenantApp.Core.Models;

public class RoleCreateRequest
{
    public string RoleName { get; set; } = null!;
    public string RoleCode { get; set; } = null!;
    public int Sort { get; set; }
}
