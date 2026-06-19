namespace MultiTenantApp.Core.Interfaces;

public interface ITenantResolver
{
    Task<string?> ResolveTenantIdAsync(string host, CancellationToken cancellationToken = default);
}
