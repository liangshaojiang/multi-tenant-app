namespace MultiTenantApp.Core.Models;

public class DepartmentCreateRequest
{
    public string DeptName { get; set; } = null!;
    public string? ParentId { get; set; }
    public int Sort { get; set; }
}
