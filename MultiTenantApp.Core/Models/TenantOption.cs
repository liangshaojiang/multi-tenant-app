namespace MultiTenantApp.Core.Models;

public class TenantOption
{
    public string TenantId { get; set; } = null!;
    public string TenantName { get; set; } = null!;
    public string? ShortName { get; set; }
}
