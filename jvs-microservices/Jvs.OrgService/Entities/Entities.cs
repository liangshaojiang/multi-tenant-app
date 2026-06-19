namespace Jvs.OrgService.Entities;

public abstract class TenantEntity
{
    public string TenantId { get; set; } = "";
}

public abstract class BaseEntity : TenantEntity
{
    public string Id { get; set; } = "";
    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public string? CreateById { get; set; }
    public string? UpdateById { get; set; }
}

public class Department : BaseEntity
{
    public string? ParentId { get; set; }
    public string DeptName { get; set; } = null!;
    // 节点类型: 1=集团根 2=公司 3=部门
    public byte DeptType { get; set; } = 3;
    public string? TaxNumber { get; set; }
    public string? SecondTaxNumber { get; set; }
    public string? SecondDeptName { get; set; }
    public int Sort { get; set; }
    public string? LeaderUserId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool Status { get; set; } = true;
}

public class Role : BaseEntity
{
    public string RoleName { get; set; } = null!;
    public string? RoleCode { get; set; }
    public byte RoleType { get; set; } = 1;
    public byte DataScopeType { get; set; } = 5;
    public int Sort { get; set; }
    public bool Status { get; set; } = true;
    public string? Remark { get; set; }
}

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
    public bool Visible { get; set; } = true;
    public bool Status { get; set; } = true;
}

public class RolePermission
{
    public string Id { get; set; } = "";
    public string RoleId { get; set; } = "";
    public string PermissionId { get; set; } = "";
    public string TenantId { get; set; } = "";
}

public class UserRole
{
    public string Id { get; set; } = "";
    public string UserId { get; set; } = "";
    public string RoleId { get; set; } = "";
    public string TenantId { get; set; } = "";
}
