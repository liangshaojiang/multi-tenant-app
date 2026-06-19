namespace MultiTenantApp.Core.Entities;

public abstract class BaseEntity : TenantEntity
{
    public string Id { get; set; } = null!;
    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public string? CreateById { get; set; }
    public string? UpdateById { get; set; }
    public string? DeptId { get; set; }
    public string? JobId { get; set; }
}
