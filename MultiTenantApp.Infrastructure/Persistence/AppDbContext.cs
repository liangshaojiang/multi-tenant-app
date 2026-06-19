using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MultiTenantApp.Core.Entities;
using MultiTenantApp.Core.Interfaces;

namespace MultiTenantApp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<UserTenant> UserTenants => Set<UserTenant>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("sys_user");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.AccountName).IsUnique();
            entity.Property(x => x.CancelFlag).HasColumnName("cancel_flag");
            entity.Property(x => x.UserType).HasColumnName("user_type");
            entity.Property(x => x.AccountName).HasColumnName("account_name");
            entity.Property(x => x.RealName).HasColumnName("real_name");
            entity.Property(x => x.HeadImg).HasColumnName("head_img");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.UpdateTime).HasColumnName("update_time");
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("sys_tenant");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ShortName).HasColumnName("short_name");
            entity.Property(x => x.AdminUserId).HasColumnName("admin_user_id");
            entity.Property(x => x.ParentId).HasColumnName("parent_id");
            entity.Property(x => x.ConnectionString).HasColumnName("connection_string");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.UpdateTime).HasColumnName("update_time");
        });

        modelBuilder.Entity<UserTenant>(entity =>
        {
            entity.ToTable("sys_user_tenant");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.UserId, x.TenantId }).IsUnique();
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.TenantId).HasColumnName("tenant_id");
            entity.Property(x => x.RealName).HasColumnName("real_name");
            entity.Property(x => x.Phone).HasColumnName("Phone");
            entity.Property(x => x.DeptId).HasColumnName("dept_id");
            entity.Property(x => x.DeptName).HasColumnName("dept_name");
            entity.Property(x => x.JobId).HasColumnName("job_id");
            entity.Property(x => x.JobName).HasColumnName("job_name");
            entity.Property(x => x.EmployeeNo).HasColumnName("employee_no");
            entity.Property(x => x.CancelFlag).HasColumnName("cancel_flag");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.UpdateTime).HasColumnName("update_time");
        });

        ConfigureTenantEntity<Department>(modelBuilder, "sys_dept");
        ConfigureTenantEntity<Role>(modelBuilder, "sys_role");
        ConfigureTenantEntity<Permission>(modelBuilder, "sys_permission");

        modelBuilder.Entity<Department>(entity =>
        {
            entity.Property(x => x.ParentId).HasColumnName("parent_id");
            entity.Property(x => x.DeptName).HasColumnName("dept_name");
            entity.Property(x => x.LeaderUserId).HasColumnName("leader_user_id");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(x => x.RoleName).HasColumnName("role_name");
            entity.Property(x => x.RoleCode).HasColumnName("role_code");
            entity.Property(x => x.RoleType).HasColumnName("role_type");
            entity.Property(x => x.DataScopeType).HasColumnName("data_scope_type");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.Property(x => x.ParentId).HasColumnName("parent_id");
            entity.Property(x => x.PermissionName).HasColumnName("permission_name");
            entity.Property(x => x.PermissionCode).HasColumnName("permission_code");
            entity.Property(x => x.PermissionType).HasColumnName("permission_type");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("sys_user_role");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.UserId, x.RoleId, x.TenantId }).IsUnique();
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.RoleId).HasColumnName("role_id");
            entity.Property(x => x.TenantId).HasColumnName("tenant_id");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("sys_role_permission");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.RoleId).HasColumnName("role_id");
            entity.Property(x => x.PermissionId).HasColumnName("permission_id");
            entity.Property(x => x.TenantId).HasColumnName("tenant_id");
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTenantInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyTenantInfo();
        return base.SaveChanges();
    }

    private void ApplyTenantInfo()
    {
        foreach (var entry in ChangeTracker.Entries<TenantEntity>())
        {
            if (entry.State == EntityState.Added && string.IsNullOrWhiteSpace(entry.Entity.TenantId))
            {
                entry.Entity.TenantId = _tenantContext.TenantId ?? string.Empty;
            }
        }

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreateTime = DateTime.UtcNow;
                entry.Entity.CreateById ??= _tenantContext.UserId;
            }
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdateTime = DateTime.UtcNow;
                entry.Entity.UpdateById = _tenantContext.UserId;
            }
        }
    }

    private void ConfigureTenantEntity<TEntity>(ModelBuilder modelBuilder, string tableName) where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>(entity =>
        {
            entity.ToTable(tableName);
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.UpdateTime).HasColumnName("update_time");
            entity.Property(x => x.CreateById).HasColumnName("create_by_id");
            entity.Property(x => x.UpdateById).HasColumnName("update_by_id");
            entity.Ignore(x => x.DeptId);
            entity.Ignore(x => x.JobId);
            entity.HasQueryFilter(x => _tenantContext.TenantId == null || x.TenantId == _tenantContext.TenantId);
        });
    }
}
