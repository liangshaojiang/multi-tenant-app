namespace MultiTenantApp.Core.Entities;

public class Role : BaseEntity
{
    public string RoleName { get; set; } = null!;
    public string? RoleCode { get; set; }
    public byte RoleType { get; set; }
    public byte DataScopeType { get; set; }
    public int Sort { get; set; }
    public bool Status { get; set; }
    public string? Remark { get; set; }
}
