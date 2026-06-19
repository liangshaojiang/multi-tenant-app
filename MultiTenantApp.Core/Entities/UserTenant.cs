namespace MultiTenantApp.Core.Entities;

public class UserTenant
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string TenantId { get; set; } = null!;
    public string? RealName { get; set; }
    public string? Phone { get; set; }
    public string? DeptId { get; set; }
    public string? DeptName { get; set; }
    public string? JobId { get; set; }
    public string? JobName { get; set; }
    public string? EmployeeNo { get; set; }
    public int Level { get; set; }
    public bool CancelFlag { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
