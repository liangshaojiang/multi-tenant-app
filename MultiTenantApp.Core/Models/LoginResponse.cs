namespace MultiTenantApp.Core.Models;

public class LoginResponse
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public string UserId { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string RealName { get; set; } = null!;
    public string TenantId { get; set; } = null!;
    public string TenantName { get; set; } = null!;
    public IReadOnlyList<TenantOption> Tenants { get; set; } = Array.Empty<TenantOption>();
}
