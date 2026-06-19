namespace MultiTenantApp.Core.Entities;

/// <summary>
/// 用户表（全局，不带租户ID）
/// </summary>
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
}
