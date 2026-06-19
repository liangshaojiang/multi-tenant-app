namespace MultiTenantApp.Core.Entities;

public class Department : BaseEntity
{
    public string? ParentId { get; set; }
    public string DeptName { get; set; } = null!;
    public int Sort { get; set; }
    public string? LeaderUserId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool Status { get; set; }
}
