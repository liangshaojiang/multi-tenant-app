using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Jvs.AuthService.Models;
using Jvs.AuthService.Services;
using Jvs.Shared.Extensions;
using Jvs.Shared.Models;

namespace Jvs.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthDomainService _authService;

    public AuthController(AuthDomainService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        try
        {
            var response = await _authService.LoginAsync(request.AccountName, request.Password, request.CompanyId, Request.Host.Host, ct);
            return Ok(ApiResponse<LoginResponse>.Ok(response, "登录成功"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<LoginResponse>.Fail(ex.Message));
        }
    }

    [HttpPost("switch-company")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> SwitchCompany([FromBody] SwitchCompanyRequest request, CancellationToken ct)
    {
        try
        {
            var userId = User.GetUserId() ?? throw new InvalidOperationException("登录已失效");
            var response = await _authService.SwitchCompanyAsync(userId, request.CompanyId, ct);
            return Ok(ApiResponse<LoginResponse>.Ok(response, "切换成功"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<LoginResponse>.Fail(ex.Message));
        }
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
    {
        try
        {
            var response = await _authService.RefreshAsync(request.RefreshToken, ct);
            return Ok(ApiResponse<LoginResponse>.Ok(response, "刷新成功"));
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(ApiResponse<LoginResponse>.Fail(ex.Message));
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserInfo>>> Me(CancellationToken ct)
    {
        try
        {
            var userId = User.GetUserId() ?? throw new InvalidOperationException("登录已失效");
            var tenantId = User.GetTenantId() ?? throw new InvalidOperationException("租户信息缺失");
            var info = await _authService.GetUserInfoAsync(userId, tenantId, ct);
            return Ok(ApiResponse<UserInfo>.Ok(info));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<UserInfo>.Fail(ex.Message));
        }
    }

    [HttpGet("users")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<UserListItem>>>> GetUsers(CancellationToken ct)
    {
        try
        {
            var tenantId = User.GetTenantId() ?? throw new InvalidOperationException("租户信息缺失");
            var users = await _authService.GetUsersAsync(tenantId, ct);
            return Ok(ApiResponse<List<UserListItem>>.Ok(users));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<List<UserListItem>>.Fail(ex.Message));
        }
    }

    [HttpPost("users")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserListItem>>> CreateUser([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        try
        {
            var tenantId = User.GetTenantId() ?? throw new InvalidOperationException("租户信息缺失");
            var userId = User.GetUserId() ?? throw new InvalidOperationException("登录已失效");
            var user = await _authService.CreateUserAsync(request, tenantId, userId, ct);
            return Ok(ApiResponse<UserListItem>.Ok(user, "创建成功"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<UserListItem>.Fail(ex.Message));
        }
    }

    [HttpGet("companies")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<Jvs.AuthService.Entities.Company>>>> GetCompanies(CancellationToken ct)
    {
        try
        {
            var tenantId = User.GetTenantId() ?? throw new InvalidOperationException("租户信息缺失");
            var companies = await _authService.GetCompaniesAsync(tenantId, ct);
            return Ok(ApiResponse<List<Jvs.AuthService.Entities.Company>>.Ok(companies));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<List<Jvs.AuthService.Entities.Company>>.Fail(ex.Message));
        }
    }

    [HttpPost("companies")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<Jvs.AuthService.Entities.Company>>> CreateCompany([FromBody] Jvs.AuthService.Entities.Company company, CancellationToken ct)
    {
        try
        {
            var tenantId = User.GetTenantId() ?? throw new InvalidOperationException("租户信息缺失");
            var userId = User.GetUserId() ?? throw new InvalidOperationException("登录已失效");
            var created = await _authService.CreateCompanyAsync(company, tenantId, userId, ct);
            return Ok(ApiResponse<Jvs.AuthService.Entities.Company>.Ok(created, "创建成功"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<Jvs.AuthService.Entities.Company>.Fail(ex.Message));
        }
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public ActionResult Health()
    {
        return Ok(new { service = "Jvs.AuthService", status = "healthy", time = DateTime.UtcNow });
    }
}
