using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantApp.Core.Interfaces;
using MultiTenantApp.Core.Models;
using MultiTenantApp.Infrastructure.Security;
using MultiTenantApp.WebApi.Models;

namespace MultiTenantApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(AuthService authService, ICurrentUserService currentUserService)
    {
        _authService = authService;
        _currentUserService = currentUserService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.LoginAsync(request, Request.Host.Host, cancellationToken);
            return Ok(ApiResponse<LoginResponse>.Ok(response, "登录成功"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<LoginResponse>.Fail(ex.Message));
        }
    }

    [HttpPost("switch-tenant")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> SwitchTenant([FromBody] SwitchTenantRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_currentUserService.UserId))
            {
                return Unauthorized(ApiResponse<LoginResponse>.Fail("登录已失效"));
            }

            var response = await _authService.SwitchTenantAsync(_currentUserService.UserId, request.TenantId, cancellationToken);
            return Ok(ApiResponse<LoginResponse>.Ok(response, "切换成功"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<LoginResponse>.Fail(ex.Message));
        }
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<ApiResponse<object>> Me()
    {
        return Ok(ApiResponse<object>.Ok(new
        {
            UserId = _currentUserService.UserId,
            TenantId = _currentUserService.TenantId,
            IsAuthenticated = _currentUserService.IsAuthenticated
        }));
    }
}
