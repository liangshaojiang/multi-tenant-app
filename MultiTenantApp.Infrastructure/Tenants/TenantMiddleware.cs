using Microsoft.AspNetCore.Http;
using MultiTenantApp.Core.Interfaces;
using System.Security.Claims;

namespace MultiTenantApp.Infrastructure.Tenants;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantResolver tenantResolver, ITenantContext tenantContext)
    {
        string? tenantId = null;

        // 优先从 JWT Claims 中获取租户ID
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            tenantId = context.User.FindFirst("tenant_id")?.Value;
        }

        // 如果未登录或 token 里没有租户ID，则从 Host 解析
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            var host = context.Request.Host.Host;
            tenantId = await tenantResolver.ResolveTenantIdAsync(host, context.RequestAborted);
        }

        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            tenantContext.TenantId = tenantId;
            context.Items["TenantId"] = tenantId;
        }

        // 同时也把 UserId 放到 TenantContext
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                tenantContext.UserId = userId;
            }
        }

        try
        {
            await _next(context);
        }
        finally
        {
            tenantContext.Clear();
        }
    }
}
