using System.Security.Claims;
using Jvs.Shared.Authentication;

namespace Jvs.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetTenantId(this ClaimsPrincipal user)
        => user.FindFirst(AuthConstants.TenantIdClaim)?.Value;

    public static string? GetCompanyId(this ClaimsPrincipal user)
        => user.FindFirst(AuthConstants.CompanyIdClaim)?.Value;

    public static string? GetUserId(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
