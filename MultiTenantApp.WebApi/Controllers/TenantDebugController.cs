using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenantApp.Core.Entities;
using MultiTenantApp.Core.Interfaces;
using MultiTenantApp.Core.Models;
using MultiTenantApp.Infrastructure.Persistence;
using MultiTenantApp.WebApi.Models;

namespace MultiTenantApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantDebugController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public TenantDebugController(AppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    [HttpGet("departments")]
    public async Task<ActionResult<ApiResponse<object>>> Departments(CancellationToken cancellationToken)
    {
        var departments = await _dbContext.Departments
            .AsNoTracking()
            .OrderBy(x => x.Sort)
            .Select(x => new
            {
                x.Id,
                x.TenantId,
                x.DeptName,
                x.ParentId
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(departments));
    }

    [HttpPost("departments")]
    public async Task<ActionResult<ApiResponse<object>>> CreateDepartment([FromBody] DepartmentCreateRequest request, CancellationToken cancellationToken)
    {
        var department = new Department
        {
            Id = Guid.NewGuid().ToString("N"),
            DeptName = request.DeptName,
            ParentId = request.ParentId,
            Sort = request.Sort,
            Status = true,
            CreateById = _currentUserService.UserId,
            TenantId = _currentUserService.TenantId ?? string.Empty,
        };

        _dbContext.Departments.Add(department);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new
        {
            department.Id,
            department.DeptName,
            department.TenantId,
        }, "创建成功"));
    }

    [HttpGet("roles")]
    public async Task<ActionResult<ApiResponse<object>>> Roles(CancellationToken cancellationToken)
    {
        var roles = await _dbContext.Roles
            .AsNoTracking()
            .OrderBy(x => x.Sort)
            .Select(x => new
            {
                x.Id,
                x.TenantId,
                x.RoleName,
                x.RoleCode
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(roles));
    }

    [HttpPost("roles")]
    public async Task<ActionResult<ApiResponse<object>>> CreateRole([FromBody] RoleCreateRequest request, CancellationToken cancellationToken)
    {
        var role = new Role
        {
            Id = Guid.NewGuid().ToString("N"),
            RoleName = request.RoleName,
            RoleCode = request.RoleCode,
            Sort = request.Sort,
            RoleType = 1,
            DataScopeType = 5,
            Status = true,
            CreateById = _currentUserService.UserId,
            TenantId = _currentUserService.TenantId ?? string.Empty,
        };

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new
        {
            role.Id,
            role.RoleName,
            role.RoleCode,
            role.TenantId,
        }, "创建成功"));
    }
}
