namespace MultiTenantApp.Core.Entities;

public abstract class TenantEntity
{
    public string TenantId { get; set; } = null!;
}
