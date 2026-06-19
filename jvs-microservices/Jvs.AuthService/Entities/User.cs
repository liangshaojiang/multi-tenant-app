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
    // 部署类型: cloud=云端SaaS, on_premise=内网自建
    public string DeploymentType { get; set; } = "cloud";
    // 内网部署时该租户的访问网关地址
    public string? GatewayUrl { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}

public class Company
{
    public string Id { get; set; } = null!;
    public string TenantId { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string? ShortName { get; set; }
    public string? TaxNumber { get; set; }
    public string? SecondTaxNumber { get; set; }
    public string? SecondCompanyName { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? BankName { get; set; }
    public string? BankAccount { get; set; }
    public bool Enable { get; set; } = true;
    public int Sort { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
    public string? CreateById { get; set; }
    public string? UpdateById { get; set; }
}

public class UserCompany
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string CompanyId { get; set; } = null!;
    public string TenantId { get; set; } = null!;
    public bool CancelFlag { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}

public class RefreshToken
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string CompanyId { get; set; } = null!;
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime? RevokedAt { get; set; }
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
    public string? Email { get; set; }
    public int Level { get; set; }
    public bool CancelFlag { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
