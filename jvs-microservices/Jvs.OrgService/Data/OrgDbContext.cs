using Microsoft.EntityFrameworkCore;
using Jvs.OrgService.Entities;

namespace Jvs.OrgService.Data;

public class OrgDbContext : DbContext
{
    public OrgDbContext(DbContextOptions<OrgDbContext> options) : base(options) { }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("org_dept");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.ParentId);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.UpdateTime).HasColumnName("update_time");
            entity.Property(x => x.CreateById).HasColumnName("create_by_id");
            entity.Property(x => x.UpdateById).HasColumnName("update_by_id");
            entity.Property(x => x.ParentId).HasColumnName("parent_id");
            entity.Property(x => x.DeptName).HasColumnName("dept_name");
            entity.Property(x => x.DeptType).HasColumnName("dept_type");
            entity.Property(x => x.TaxNumber).HasColumnName("tax_number");
            entity.Property(x => x.SecondTaxNumber).HasColumnName("second_tax_number");
            entity.Property(x => x.SecondDeptName).HasColumnName("second_dept_name");
            entity.Property(x => x.LeaderUserId).HasColumnName("leader_user_id");
            entity.HasQueryFilter(x => x.TenantId == _tenantId || _tenantId == null);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("org_role");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.TenantId);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.UpdateTime).HasColumnName("update_time");
            entity.Property(x => x.CreateById).HasColumnName("create_by_id");
            entity.Property(x => x.UpdateById).HasColumnName("update_by_id");
            entity.Property(x => x.RoleName).HasColumnName("role_name");
            entity.Property(x => x.RoleCode).HasColumnName("role_code");
            entity.Property(x => x.RoleType).HasColumnName("role_type");
            entity.Property(x => x.DataScopeType).HasColumnName("data_scope_type");
            entity.HasQueryFilter(x => x.TenantId == _tenantId || _tenantId == null);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("org_permission");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.TenantId);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.UpdateTime).HasColumnName("update_time");
            entity.Property(x => x.CreateById).HasColumnName("create_by_id");
            entity.Property(x => x.UpdateById).HasColumnName("update_by_id");
            entity.Property(x => x.ParentId).HasColumnName("parent_id");
            entity.Property(x => x.PermissionName).HasColumnName("permission_name");
            entity.Property(x => x.PermissionCode).HasColumnName("permission_code");
            entity.Property(x => x.PermissionType).HasColumnName("permission_type");
            entity.HasQueryFilter(x => x.TenantId == _tenantId || _tenantId == null);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("org_role_permission");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.RoleId).HasColumnName("role_id");
            entity.Property(x => x.PermissionId).HasColumnName("permission_id");
            entity.Property(x => x.TenantId).HasColumnName("tenant_id");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("org_user_role");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.RoleId).HasColumnName("role_id");
            entity.Property(x => x.TenantId).HasColumnName("tenant_id");
        });
    }

    private string? _tenantId;

    public void SetTenantId(string? tenantId)
    {
        _tenantId = tenantId;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditInfo();
        return base.SaveChanges();
    }

    private void ApplyAuditInfo()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreateTime = DateTime.UtcNow;
                entry.Entity.UpdateTime = DateTime.UtcNow;
            }
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdateTime = DateTime.UtcNow;
            }
        }
    }
}
