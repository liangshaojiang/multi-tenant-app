using Microsoft.EntityFrameworkCore;
using Jvs.AuthService.Entities;

namespace Jvs.AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<UserTenant> UserTenants => Set<UserTenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("auth_user");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.AccountName).IsUnique();
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.UpdateTime).HasColumnName("update_time");
            entity.Property(x => x.CreateById).HasColumnName("create_by_id");
            entity.Property(x => x.UpdateById).HasColumnName("update_by_id");
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("auth_tenant");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ShortName).HasColumnName("short_name");
            entity.Property(x => x.AdminUserId).HasColumnName("admin_user_id");
            entity.Property(x => x.ParentId).HasColumnName("parent_id");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.UpdateTime).HasColumnName("update_time");
        });

        modelBuilder.Entity<UserTenant>(entity =>
        {
            entity.ToTable("auth_user_tenant");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.UserId, x.TenantId }).IsUnique();
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.TenantId).HasColumnName("tenant_id");
            entity.Property(x => x.RealName).HasColumnName("real_name");
            entity.Property(x => x.Phone).HasColumnName("phone");
            entity.Property(x => x.DeptId).HasColumnName("dept_id");
            entity.Property(x => x.DeptName).HasColumnName("dept_name");
            entity.Property(x => x.JobId).HasColumnName("job_id");
            entity.Property(x => x.JobName).HasColumnName("job_name");
            entity.Property(x => x.EmployeeNo).HasColumnName("employee_no");
            entity.Property(x => x.CancelFlag).HasColumnName("cancel_flag");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.UpdateTime).HasColumnName("update_time");
        });
    }
}
