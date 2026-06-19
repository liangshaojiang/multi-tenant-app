using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MultiTenantApp.Core.Interfaces;
using MultiTenantApp.Infrastructure.Persistence;

namespace MultiTenantApp.Infrastructure.Tenants;

public class TenantResolver : ITenantResolver
{
    private readonly AppDbContext _dbContext;

    public TenantResolver(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> ResolveTenantIdAsync(string host, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            return null;
        }

        var tenants = await _dbContext.Tenants
            .AsNoTracking()
            .Where(x => x.Enable)
            .ToListAsync(cancellationToken);

        foreach (var tenant in tenants)
        {
            if (string.IsNullOrWhiteSpace(tenant.Hosts))
            {
                continue;
            }

            var hosts = JsonSerializer.Deserialize<List<string>>(tenant.Hosts) ?? new List<string>();
            if (hosts.Any(x => string.Equals(x, host, StringComparison.OrdinalIgnoreCase)))
            {
                return tenant.Id;
            }
        }

        return null;
    }
}
