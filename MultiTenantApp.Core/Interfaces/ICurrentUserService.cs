namespace MultiTenantApp.Core.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? TenantId { get; }
    bool IsAuthenticated { get; }
}
