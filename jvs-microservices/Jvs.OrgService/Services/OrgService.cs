using Jvs.OrgService.Data;
using Jvs.OrgService.Entities;
using Microsoft.EntityFrameworkCore;
using Jvs.Shared.Extensions;

namespace Jvs.OrgService.Services;

public class DepartmentService
{
    private readonly OrgDbContext _dbContext;

    public DepartmentService(OrgDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Department>> GetListAsync(string tenantId, CancellationToken ct = default)
    {
        _dbContext.SetTenantId(tenantId);
        return await _dbContext.Departments
            .AsNoTracking()
            .OrderBy(x => x.Sort)
            .ThenBy(x => x.CreateTime)
            .ToListAsync(ct);
    }

    public async Task<int> CountCompaniesAsync(string tenantId, CancellationToken ct = default)
    {
        _dbContext.SetTenantId(tenantId);
        return await _dbContext.Departments
            .AsNoTracking()
            .CountAsync(x => x.DeptType == 2, ct);
    }

    public async Task<Department?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await _dbContext.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Department> CreateAsync(Department entity, string tenantId, string userId, CancellationToken ct = default)
    {
        _dbContext.SetTenantId(tenantId);
        entity.Id = Guid.NewGuid().ToString("N");
        entity.TenantId = tenantId;
        entity.CreateById = userId;
        entity.UpdateById = userId;
        entity.CreateTime = DateTime.UtcNow;
        entity.UpdateTime = DateTime.UtcNow;

        _dbContext.Departments.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<Department> UpdateAsync(Department entity, string userId, CancellationToken ct = default)
    {
        entity.UpdateById = userId;
        entity.UpdateTime = DateTime.UtcNow;
        _dbContext.Departments.Update(entity);
        await _dbContext.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Departments.FindAsync(id);
        if (entity != null)
        {
            _dbContext.Departments.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}

public class RoleService
{
    private readonly OrgDbContext _dbContext;

    public RoleService(OrgDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Role>> GetListAsync(string tenantId, CancellationToken ct = default)
    {
        _dbContext.SetTenantId(tenantId);
        return await _dbContext.Roles
            .AsNoTracking()
            .OrderBy(x => x.Sort)
            .ThenBy(x => x.CreateTime)
            .ToListAsync(ct);
    }

    public async Task<Role?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await _dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Role> CreateAsync(Role entity, string tenantId, string userId, CancellationToken ct = default)
    {
        _dbContext.SetTenantId(tenantId);
        entity.Id = Guid.NewGuid().ToString("N");
        entity.TenantId = tenantId;
        entity.CreateById = userId;
        entity.UpdateById = userId;
        entity.CreateTime = DateTime.UtcNow;
        entity.UpdateTime = DateTime.UtcNow;

        _dbContext.Roles.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<Role> UpdateAsync(Role entity, string userId, CancellationToken ct = default)
    {
        entity.UpdateById = userId;
        entity.UpdateTime = DateTime.UtcNow;
        _dbContext.Roles.Update(entity);
        await _dbContext.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Roles.FindAsync(id);
        if (entity != null)
        {
            _dbContext.Roles.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
