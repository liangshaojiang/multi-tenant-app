namespace Jvs.AuthService.Entities;

public class User
{
    public string Id { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string RealName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? HeadImg { get; set; }
    public byte? Sex { get; set; }
    public DateTime? Birthday { get; set; }
    public bool CancelFlag { get; set; }
    public byte UserType { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
    public string? CreateById { get; set; }
    public string? UpdateById { get; set; }
}

public class Tenant
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ShortName { get; set; }
    public string? Logo { get; set; }
    public string? Icon { get; set; }
    public string AdminUserId { get; set; } = null!;
    public string? ParentId { get; set; }
    public bool Enable { get; set; }
    public string? Hosts { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}

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
