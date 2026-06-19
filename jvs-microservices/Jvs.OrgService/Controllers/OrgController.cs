using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Jvs.OrgService.Entities;
using Jvs.OrgService.Services;
using Jvs.Shared.Extensions;
using Jvs.Shared.Models;

namespace Jvs.OrgService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly DepartmentService _departmentService;

    public DepartmentsController(DepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Department>>>> GetList(CancellationToken ct)
    {
        var tenantId = User.GetTenantId();
        if (string.IsNullOrEmpty(tenantId))
            return BadRequest(ApiResponse<List<Department>>.Fail("租户信息缺失"));

        var list = await _departmentService.GetListAsync(tenantId, ct);
        return Ok(ApiResponse<List<Department>>.Ok(list));
    }

    [HttpGet("tree")]
    public async Task<ActionResult<ApiResponse<List<DeptTreeNode>>>> GetTree(CancellationToken ct)
    {
        var tenantId = User.GetTenantId();
        if (string.IsNullOrEmpty(tenantId))
            return BadRequest(ApiResponse<List<DeptTreeNode>>.Fail("租户信息缺失"));

        var list = await _departmentService.GetListAsync(tenantId, ct);
        var tree = BuildTree(list, null);
        return Ok(ApiResponse<List<DeptTreeNode>>.Ok(tree));
    }

    [HttpGet("company-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetCompanyCount(CancellationToken ct)
    {
        var tenantId = User.GetTenantId();
        if (string.IsNullOrEmpty(tenantId))
            return BadRequest(ApiResponse<int>.Fail("租户信息缺失"));

        var count = await _departmentService.CountCompaniesAsync(tenantId, ct);
        return Ok(ApiResponse<int>.Ok(count));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Department>>> GetById(string id, CancellationToken ct)
    {
        var entity = await _departmentService.GetByIdAsync(id, ct);
        if (entity == null)
            return NotFound(ApiResponse<Department>.Fail("部门不存在"));
        return Ok(ApiResponse<Department>.Ok(entity));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Department>>> Create([FromBody] Department entity, CancellationToken ct)
    {
        var tenantId = User.GetTenantId();
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(userId))
            return BadRequest(ApiResponse<Department>.Fail("用户信息缺失"));

        if (entity.DeptType == 2)
        {
            var companyCount = await _departmentService.CountCompaniesAsync(tenantId, ct);
            if (companyCount >= 5)
                return BadRequest(ApiResponse<Department>.Fail("公司数量已达上限（5个）"));
        }

        var created = await _departmentService.CreateAsync(entity, tenantId, userId, ct);
        return Ok(ApiResponse<Department>.Ok(created, "创建成功"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Department>>> Update(string id, [FromBody] Department entity, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return BadRequest(ApiResponse<Department>.Fail("用户信息缺失"));

        entity.Id = id;
        var updated = await _departmentService.UpdateAsync(entity, userId, ct);
        return Ok(ApiResponse<Department>.Ok(updated, "更新成功"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(string id, CancellationToken ct)
    {
        await _departmentService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "删除成功"));
    }

    private static List<DeptTreeNode> BuildTree(List<Department> all, string? parentId)
    {
        return all
            .Where(x => x.ParentId == parentId)
            .Select(x => new DeptTreeNode
            {
                Id = x.Id,
                DeptName = x.DeptName,
                DeptType = x.DeptType,
                ParentId = x.ParentId,
                TaxNumber = x.TaxNumber,
                SecondTaxNumber = x.SecondTaxNumber,
                SecondDeptName = x.SecondDeptName,
                Sort = x.Sort,
                Status = x.Status,
                LeaderUserId = x.LeaderUserId,
                Phone = x.Phone,
                Email = x.Email,
                Children = BuildTree(all, x.Id)
            })
            .ToList();
    }
}

public class DeptTreeNode
{
    public string Id { get; set; } = "";
    public string DeptName { get; set; } = "";
    public byte DeptType { get; set; }
    public string? ParentId { get; set; }
    public string? TaxNumber { get; set; }
    public string? SecondTaxNumber { get; set; }
    public string? SecondDeptName { get; set; }
    public int Sort { get; set; }
    public bool Status { get; set; }
    public string? LeaderUserId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public List<DeptTreeNode> Children { get; set; } = new();
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly RoleService _roleService;

    public RolesController(RoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Role>>>> GetList(CancellationToken ct)
    {
        var tenantId = User.GetTenantId();
        if (string.IsNullOrEmpty(tenantId))
            return BadRequest(ApiResponse<List<Role>>.Fail("租户信息缺失"));

        var list = await _roleService.GetListAsync(tenantId, ct);
        return Ok(ApiResponse<List<Role>>.Ok(list));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Role>>> GetById(string id, CancellationToken ct)
    {
        var entity = await _roleService.GetByIdAsync(id, ct);
        if (entity == null)
            return NotFound(ApiResponse<Role>.Fail("角色不存在"));
        return Ok(ApiResponse<Role>.Ok(entity));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Role>>> Create([FromBody] Role entity, CancellationToken ct)
    {
        var tenantId = User.GetTenantId();
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(userId))
            return BadRequest(ApiResponse<Role>.Fail("用户信息缺失"));

        var created = await _roleService.CreateAsync(entity, tenantId, userId, ct);
        return Ok(ApiResponse<Role>.Ok(created, "创建成功"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Role>>> Update(string id, [FromBody] Role entity, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return BadRequest(ApiResponse<Role>.Fail("用户信息缺失"));

        entity.Id = id;
        var updated = await _roleService.UpdateAsync(entity, userId, ct);
        return Ok(ApiResponse<Role>.Ok(updated, "更新成功"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(string id, CancellationToken ct)
    {
        await _roleService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "删除成功"));
    }
}
