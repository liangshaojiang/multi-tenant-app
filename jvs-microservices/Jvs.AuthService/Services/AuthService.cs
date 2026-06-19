using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Jvs.AuthService.Data;
using Jvs.AuthService.Entities;
using Jvs.AuthService.Models;
using Jvs.Shared.Authentication;
using Jvs.Shared.Models;

namespace Jvs.AuthService.Services;

public class AuthDomainService
{
    private readonly AuthDbContext _dbContext;
    private readonly JwtSettings _jwtSettings;

    public AuthDomainService(AuthDbContext dbContext, IOptions<JwtSettings> jwtSettings)
    {
        _dbContext = dbContext;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponse> LoginAsync(string accountName, string password, string? companyId, string? host, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.AccountName == accountName && !x.CancelFlag, ct);

        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            throw new InvalidOperationException("账号或密码错误");

        return await BuildLoginResponseAsync(user, companyId, host, ct);
    }

    public async Task<LoginResponse> SwitchCompanyAsync(string userId, string companyId, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId && !x.CancelFlag, ct);

        if (user is null)
            throw new InvalidOperationException("用户不存在");

        return await BuildLoginResponseAsync(user, companyId, null, ct);
    }

    public async Task<LoginResponse> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshToken && x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow, ct);

        if (token is null)
            throw new InvalidOperationException("登录已过期");

        var user = await _dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == token.UserId && !x.CancelFlag, ct);

        if (user is null)
            throw new InvalidOperationException("用户不存在");

        token.RevokedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(ct);

        return await BuildLoginResponseAsync(user, token.CompanyId, null, ct);
    }

    public async Task<UserInfo> GetUserInfoAsync(string userId, string tenantId, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId && !x.CancelFlag, ct);

        if (user is null)
            throw new InvalidOperationException("用户不存在");

        var tenantLinks = await (from ut in _dbContext.UserTenants.AsNoTracking()
                                  join t in _dbContext.Tenants.AsNoTracking() on ut.TenantId equals t.Id
                                  where ut.UserId == userId && !ut.CancelFlag && t.Enable
                                  select new { ut, t })
            .ToListAsync(ct);

        var currentTenant = tenantLinks.FirstOrDefault(x => x.t.Id == tenantId) ?? tenantLinks.FirstOrDefault();

        return new UserInfo
        {
            Id = user.Id,
            AccountName = user.AccountName,
            RealName = user.RealName,
            Phone = user.Phone,
            Email = user.Email,
            TenantId = currentTenant?.t.Id ?? tenantId,
            TenantName = currentTenant?.t.Name ?? "",
            Tenants = tenantLinks.Select(x => new TenantInfo
            {
                Id = x.t.Id,
                Name = x.t.Name,
                ShortName = x.t.ShortName,
                Enable = x.t.Enable
            }).ToList()
        };
    }

    public async Task<List<UserListItem>> GetUsersAsync(string tenantId, CancellationToken ct = default)
    {
        return await (from ut in _dbContext.UserTenants.AsNoTracking()
                      join u in _dbContext.Users.AsNoTracking() on ut.UserId equals u.Id
                      where ut.TenantId == tenantId && !ut.CancelFlag && !u.CancelFlag
                      orderby u.CreateTime descending
                      select new UserListItem
                      {
                          Id = u.Id,
                          AccountName = u.AccountName,
                          RealName = ut.RealName ?? u.RealName,
                          Phone = ut.Phone ?? u.Phone,
                          Email = ut.Email ?? u.Email,
                          CancelFlag = u.CancelFlag,
                          UserType = u.UserType,
                          DeptId = ut.DeptId,
                          DeptName = ut.DeptName
                      }).ToListAsync(ct);
    }

    public async Task<UserListItem> CreateUserAsync(CreateUserRequest request, string tenantId, string currentUserId, CancellationToken ct = default)
    {
        var exists = await _dbContext.Users.AnyAsync(x => x.AccountName == request.AccountName, ct);
        if (exists)
            throw new InvalidOperationException("账号已存在");

        var departmentName = string.Empty;
        if (!string.IsNullOrWhiteSpace(request.DeptId))
        {
            departmentName = await _dbContext.UserTenants.AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.DeptId == request.DeptId)
                .Select(x => x.DeptName ?? string.Empty)
                .FirstOrDefaultAsync(ct);
        }

        var user = new User
        {
            Id = Guid.NewGuid().ToString("N"),
            AccountName = request.AccountName,
            RealName = request.RealName,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Phone = request.Phone,
            Email = request.Email,
            CancelFlag = false,
            UserType = 1,
            CreateTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow,
            CreateById = currentUserId,
            UpdateById = currentUserId
        };

        var userTenant = new UserTenant
        {
            Id = Guid.NewGuid().ToString("N"),
            UserId = user.Id,
            TenantId = tenantId,
            RealName = request.RealName,
            Phone = request.Phone,
            Email = request.Email,
            DeptId = request.DeptId,
            DeptName = string.IsNullOrWhiteSpace(departmentName) ? null : departmentName,
            CancelFlag = false,
            CreateTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        _dbContext.UserTenants.Add(userTenant);
        await _dbContext.SaveChangesAsync(ct);

        return new UserListItem
        {
            Id = user.Id,
            AccountName = user.AccountName,
            RealName = userTenant.RealName ?? user.RealName,
            Phone = userTenant.Phone ?? user.Phone,
            Email = userTenant.Email ?? user.Email,
            CancelFlag = user.CancelFlag,
            UserType = user.UserType,
            DeptId = userTenant.DeptId,
            DeptName = userTenant.DeptName
        };
    }

    public async Task<List<Company>> GetCompaniesAsync(string tenantId, CancellationToken ct = default)
    {
        return await _dbContext.Companies.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Sort)
            .ThenBy(x => x.CreateTime)
            .ToListAsync(ct);
    }

    public async Task<Company> CreateCompanyAsync(Company company, string tenantId, string currentUserId, CancellationToken ct = default)
    {
        company.Id = Guid.NewGuid().ToString("N");
        company.TenantId = tenantId;
        company.Enable = true;
        company.CreateById = currentUserId;
        company.UpdateById = currentUserId;
        company.CreateTime = DateTime.UtcNow;
        company.UpdateTime = DateTime.UtcNow;

        _dbContext.Companies.Add(company);

        // 创建公司的人自动获得该公司访问权
        _dbContext.UserCompanies.Add(new UserCompany
        {
            Id = Guid.NewGuid().ToString("N"),
            UserId = currentUserId,
            CompanyId = company.Id,
            TenantId = tenantId,
            CancelFlag = false,
            CreateTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync(ct);
        return company;
    }

    private async Task<LoginResponse> BuildLoginResponseAsync(User user, string? requestedCompanyId, string? host, CancellationToken ct)
    {
        // 查出用户能访问的所有公司，连带它们所属的租户（可跨云端/内网）
        var accessible = await (from uc in _dbContext.UserCompanies.AsNoTracking()
                                join c in _dbContext.Companies.AsNoTracking() on uc.CompanyId equals c.Id
                                join t in _dbContext.Tenants.AsNoTracking() on c.TenantId equals t.Id
                                where uc.UserId == user.Id && !uc.CancelFlag && c.Enable && t.Enable
                                select new { c, t })
            .ToListAsync(ct);

        if (accessible.Count == 0)
            throw new InvalidOperationException("当前用户未关联任何公司");

        var selected = SelectCompany(requestedCompanyId, host, accessible.Select(x => (x.c, x.t)).ToList());
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes);
        var token = GenerateToken(user, selected.t, selected.c, expiresAt);
        var refreshToken = await CreateRefreshTokenAsync(user.Id, selected.c.Id, ct);

        var groups = accessible
            .GroupBy(x => new { x.t.Id, x.t.Name, x.t.DeploymentType, x.t.GatewayUrl })
            .Select(g => new TenantCompanyGroup
            {
                TenantId = g.Key.Id,
                TenantName = g.Key.Name,
                DeploymentType = g.Key.DeploymentType,
                GatewayUrl = g.Key.GatewayUrl,
                Companies = g.Select(x => new CompanyOption
                {
                    CompanyId = x.c.Id,
                    CompanyName = x.c.CompanyName,
                    ShortName = x.c.ShortName
                }).ToList()
            })
            .ToList();

        return new LoginResponse
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            AccountName = user.AccountName,
            RealName = user.RealName,
            TenantId = selected.t.Id,
            TenantName = selected.t.Name,
            CompanyId = selected.c.Id,
            CompanyName = selected.c.CompanyName,
            CompanyGroups = groups
        };
    }

    private async Task<string> CreateRefreshTokenAsync(string userId, string companyId, CancellationToken ct)
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(bytes);

        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid().ToString("N"),
            UserId = userId,
            CompanyId = companyId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshExpireDays),
            CreateTime = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync(ct);
        return token;
    }

    private string GenerateToken(User user, Tenant tenant, Company company, DateTime expiresAt)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.AccountName),
            new("real_name", user.RealName),
            new(AuthConstants.TenantIdClaim, tenant.Id),
            new(AuthConstants.TenantNameClaim, tenant.Name),
            new(AuthConstants.CompanyIdClaim, company.Id),
            new(AuthConstants.CompanyNameClaim, company.CompanyName),
        };

        SigningCredentials credentials;
        if (_jwtSettings.UseRsa && !string.IsNullOrEmpty(_jwtSettings.PrivateKey))
        {
            // RSA 非对称签名（云端用私钥签）
            var rsa = RSA.Create();
            rsa.ImportFromPem(_jwtSettings.PrivateKey.ToCharArray());
            credentials = new SigningCredentials(new RsaSecurityKey(rsa) { KeyId = "rsa-prod" }, SecurityAlgorithms.RsaSha256);
        }
        else
        {
            // HMAC 对称签名（开发环境向后兼容）
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static (Company c, Tenant t) SelectCompany(string? requestedCompanyId, string? host, List<(Company c, Tenant t)> links)
    {
        if (!string.IsNullOrWhiteSpace(requestedCompanyId))
        {
            var matched = links.FirstOrDefault(x => x.c.Id == requestedCompanyId);
            if (matched != default)
                return matched;
            throw new InvalidOperationException("公司不存在或无权访问");
        }

        if (!string.IsNullOrWhiteSpace(host))
        {
            var hostMatched = links.FirstOrDefault(x => (x.t.Hosts ?? "").Contains(host, StringComparison.OrdinalIgnoreCase));
            if (hostMatched != default)
                return hostMatched;
        }

        return links[0];
    }
}

public class LoginRequest
{
    public string AccountName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? CompanyId { get; set; }
}

public class SwitchCompanyRequest
{
    public string CompanyId { get; set; } = null!;
}

public class RefreshRequest
{
    public string RefreshToken { get; set; } = null!;
}

public class LoginResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public string UserId { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string RealName { get; set; } = null!;
    public string TenantId { get; set; } = null!;
    public string TenantName { get; set; } = null!;
    public string CompanyId { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    // 按租户分组的可访问公司列表
    public List<TenantCompanyGroup> CompanyGroups { get; set; } = new();
}

public class TenantCompanyGroup
{
    public string TenantId { get; set; } = null!;
    public string TenantName { get; set; } = null!;
    public string DeploymentType { get; set; } = "cloud";
    public string? GatewayUrl { get; set; }
    public List<CompanyOption> Companies { get; set; } = new();
}

public class CompanyOption
{
    public string CompanyId { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string? ShortName { get; set; }
}
