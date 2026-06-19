namespace MultiTenantApp.Core.Models;

public class LoginRequest
{
    public string AccountName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? TenantId { get; set; }
}
