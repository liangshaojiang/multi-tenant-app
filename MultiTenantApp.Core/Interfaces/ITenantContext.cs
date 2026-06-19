namespace MultiTenantApp.Core.Interfaces;

public interface ITenantContext
{
    string? TenantId { get; set; }
    string? UserId { get; set; }
    void Clear();
}
