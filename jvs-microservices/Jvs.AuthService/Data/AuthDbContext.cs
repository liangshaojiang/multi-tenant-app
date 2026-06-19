using Microsoft.EntityFrameworkCore;
using Jvs.AuthService.Entities;

namespace Jvs.AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<UserTenant> UserTenants => Set<UserTenant>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<UserCompany> UserCompanies => Set<UserCompany>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

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
            entity.Property(x => x.DeploymentType).HasColumnName("deployment_type");
            entity.Property(x => x.GatewayUrl).HasColumnName("gateway_url");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.UpdateTime).HasColumnName("update_time");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("auth_company");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.TenantId);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id");
            entity.Property(x => x.CompanyName).HasColumnName("company_name");
            entity.Property(x => x.ShortName).HasColumnName("short_name");
            entity.Property(x => x.TaxNumber).HasColumnName("tax_number");
            entity.Property(x => x.SecondTaxNumber).HasColumnName("second_tax_number");
            entity.Property(x => x.SecondCompanyName).HasColumnName("second_company_name");
            entity.Property(x => x.Address).HasColumnName("address");
            entity.Property(x => x.Phone).HasColumnName("phone");
            entity.Property(x => x.BankName).HasColumnName("bank_name");
            entity.Property(x => x.BankAccount).HasColumnName("bank_account");
            entity.Property(x => x.Enable).HasColumnName("enable");
            entity.Property(x => x.Sort).HasColumnName("sort");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.UpdateTime).HasColumnName("update_time");
            entity.Property(x => x.CreateById).HasColumnName("create_by_id");
            entity.Property(x => x.UpdateById).HasColumnName("update_by_id");
        });

        modelBuilder.Entity<UserCompany>(entity =>
        {
            entity.ToTable("auth_user_company");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.UserId, x.CompanyId }).IsUnique();
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.CompanyId).HasColumnName("company_id");
            entity.Property(x => x.TenantId).HasColumnName("tenant_id");
            entity.Property(x => x.CancelFlag).HasColumnName("cancel_flag");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.UpdateTime).HasColumnName("update_time");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("auth_refresh_token");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Token).IsUnique();
            entity.HasIndex(x => x.UserId);
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.CompanyId).HasColumnName("company_id");
            entity.Property(x => x.Token).HasColumnName("token");
            entity.Property(x => x.ExpiresAt).HasColumnName("expires_at");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.RevokedAt).HasColumnName("revoked_at");
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
            entity.Property(x => x.Email).HasColumnName("email");
            entity.Property(x => x.CancelFlag).HasColumnName("cancel_flag");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.UpdateTime).HasColumnName("update_time");
        });
    }
}
