using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultiTenantApp.Core.Entities;
using MultiTenantApp.Core.Models;
using MultiTenantApp.Infrastructure.Persistence;

namespace MultiTenantApp.Infrastructure.Security;

public class AuthService
{
    private readonly AppDbContext _dbContext;
    private readonly JwtOptions _jwtOptions;

    public AuthService(AppDbContext dbContext, IOptions<JwtOptions> jwtOptions)
    {
        _dbContext = dbContext;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, string? host, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AccountName == request.AccountName && !x.CancelFlag, cancellationToken);

        if (user is null || !PasswordMatches(request.Password, user.Password))
        {
            throw new InvalidOperationException("账号或密码错误");
        }

        return await BuildLoginResponseAsync(user, request.TenantId, host, cancellationToken);
    }

    public async Task<LoginResponse> SwitchTenantAsync(string userId, string tenantId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId && !x.CancelFlag, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("用户不存在");
        }

        return await BuildLoginResponseAsync(user, tenantId, null, cancellationToken);
    }

    private async Task<LoginResponse> BuildLoginResponseAsync(User user, string? requestedTenantId, string? host, CancellationToken cancellationToken)
    {
        var tenantLinks = await (from userTenant in _dbContext.UserTenants.AsNoTracking()
                                 join tenant in _dbContext.Tenants.AsNoTracking() on userTenant.TenantId equals tenant.Id
                                 where userTenant.UserId == user.Id && !userTenant.CancelFlag && tenant.Enable
                                 select new TenantLoginLink(userTenant, tenant))
            .ToListAsync(cancellationToken);

        if (tenantLinks.Count == 0)
        {
            throw new InvalidOperationException("当前用户未关联任何租户");
        }

        var tenantOptions = tenantLinks
            .Select(x => new TenantOption
            {
                TenantId = x.Tenant.Id,
                TenantName = x.Tenant.Name,
                ShortName = x.Tenant.ShortName
            })
            .ToList();

        var selectedTenant = SelectTenant(requestedTenantId, host, tenantLinks);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes);
        var accessToken = GenerateToken(user, selectedTenant.Tenant, expiresAt);

        return new LoginResponse
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            AccountName = user.AccountName,
            RealName = user.RealName,
            TenantId = selectedTenant.Tenant.Id,
            TenantName = selectedTenant.Tenant.Name,
            Tenants = tenantOptions
        };
    }

    private static bool PasswordMatches(string rawPassword, string hashedPassword)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(rawPassword, hashedPassword);
        }
        catch
        {
            return false;
        }
    }

    private string GenerateToken(User user, Tenant tenant, DateTime expiresAt)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.AccountName),
            new("real_name", user.RealName),
            new("tenant_id", tenant.Id),
            new("tenant_name", tenant.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static TenantLoginLink SelectTenant(string? requestedTenantId, string? host, List<TenantLoginLink> tenantLinks)
    {
        if (!string.IsNullOrWhiteSpace(requestedTenantId))
        {
            var matched = tenantLinks.FirstOrDefault(x => x.Tenant.Id == requestedTenantId);
            if (matched is null)
            {
                throw new InvalidOperationException("租户不存在或无权访问");
            }
            return matched;
        }

        if (!string.IsNullOrWhiteSpace(host))
        {
            var hostMatched = tenantLinks.FirstOrDefault(x => (x.Tenant.Hosts ?? string.Empty).Contains(host, StringComparison.OrdinalIgnoreCase));
            if (hostMatched is not null)
            {
                return hostMatched;
            }
        }

        return tenantLinks[0];
    }

    private sealed record TenantLoginLink(UserTenant UserTenant, Tenant Tenant);
}
