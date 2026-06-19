using System.Security.Claims;
using MultiTenantApp.Core.Interfaces;

namespace MultiTenantApp.Infrastructure.Security;

public class TenantContext : ITenantContext
{
    private static readonly AsyncLocal<string?> TenantIdHolder = new();
    private static readonly AsyncLocal<string?> UserIdHolder = new();

    public string? TenantId
    {
        get => TenantIdHolder.Value;
        set => TenantIdHolder.Value = value;
    }

    public string? UserId
    {
        get => UserIdHolder.Value;
        set => UserIdHolder.Value = value;
    }

    public void Clear()
    {
        TenantIdHolder.Value = null;
        UserIdHolder.Value = null;
    }
}
