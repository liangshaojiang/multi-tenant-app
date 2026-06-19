using Jvs.AuthService.Data;
using Jvs.AuthService.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jvs.AuthService.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AuthDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        // 创建租户（platform 云端，tenant_002 标记为内网自建）
        var platformTenant = new Tenant
        {
            Id = "platform",
            Name = "平台租户",
            ShortName = "平台",
            AdminUserId = "1",
            Enable = true,
            Hosts = "[\"localhost\"]",
            DeploymentType = "cloud",
            CreateTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };

        var tenant1 = new Tenant
        {
            Id = "tenant_001",
            Name = "测试企业A",
            ShortName = "企业A",
            AdminUserId = "2",
            ParentId = "platform",
            Enable = true,
            Hosts = "[\"tenant1.localhost\"]",
            DeploymentType = "cloud",
            CreateTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };

        var tenant2 = new Tenant
        {
            Id = "tenant_002",
            Name = "测试企业B（内网）",
            ShortName = "企业B",
            AdminUserId = "3",
            ParentId = "platform",
            Enable = true,
            Hosts = "[\"tenant2.localhost\"]",
            DeploymentType = "on_premise",
            GatewayUrl = "http://192.168.1.100:5200",
            CreateTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };

        context.Tenants.AddRange(platformTenant, tenant1, tenant2);

        // 创建用户 (密码: 123456)
        var adminUser = new User
        {
            Id = "1",
            AccountName = "admin",
            RealName = "系统管理员",
            Password = "$2a$11$QdZUvfpJ1e7myINPcnVtiOfzx0DMvwjcK3KAR5DF7XLy2nb6Fisty",
            Phone = "13800138000",
            Email = "admin@example.com",
            CancelFlag = false,
            UserType = 1,
            CreateTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };

        var tenant1Admin = new User
        {
            Id = "2",
            AccountName = "tenant1_admin",
            RealName = "租户1管理员",
            Password = "$2a$11$QdZUvfpJ1e7myINPcnVtiOfzx0DMvwjcK3KAR5DF7XLy2nb6Fisty",
            Phone = "13800138001",
            Email = "tenant1@example.com",
            CancelFlag = false,
            UserType = 1,
            CreateTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };

        var tenant2Admin = new User
        {
            Id = "3",
            AccountName = "tenant2_admin",
            RealName = "租户2管理员",
            Password = "$2a$11$QdZUvfpJ1e7myINPcnVtiOfzx0DMvwjcK3KAR5DF7XLy2nb6Fisty",
            Phone = "13800138002",
            Email = "tenant2@example.com",
            CancelFlag = false,
            UserType = 1,
            CreateTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };

        var normalUser = new User
        {
            Id = "4",
            AccountName = "user1",
            RealName = "普通用户1",
            Password = "$2a$11$QdZUvfpJ1e7myINPcnVtiOfzx0DMvwjcK3KAR5DF7XLy2nb6Fisty",
            Phone = "13800138003",
            Email = "user1@example.com",
            CancelFlag = false,
            UserType = 1,
            CreateTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };

        context.Users.AddRange(adminUser, tenant1Admin, tenant2Admin, normalUser);

        // 创建用户租户关系（保留，用户信息按租户维护）
        var userTenants = new List<UserTenant>
        {
            new() { Id = "ut_1", UserId = "1", TenantId = "platform", RealName = "系统管理员", Phone = "13800138000", CancelFlag = false, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
            new() { Id = "ut_1b", UserId = "1", TenantId = "tenant_002", RealName = "系统管理员", Phone = "13800138000", CancelFlag = false, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
            new() { Id = "ut_2", UserId = "2", TenantId = "tenant_001", RealName = "租户1管理员", Phone = "13800138001", CancelFlag = false, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
            new() { Id = "ut_3", UserId = "3", TenantId = "tenant_002", RealName = "租户2管理员", Phone = "13800138002", CancelFlag = false, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
            new() { Id = "ut_4", UserId = "4", TenantId = "tenant_001", RealName = "普通用户1", Phone = "13800138003", CancelFlag = false, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
        };

        context.UserTenants.AddRange(userTenants);

        // 创建公司（每个租户下有公司，admin 跨云端+内网两个租户）
        var companies = new List<Company>
        {
            new() { Id = "comp_p1", TenantId = "platform", CompanyName = "平台运营主体", ShortName = "平台主体", TaxNumber = "91110000PLAT0001X", Enable = true, Sort = 0, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
            new() { Id = "comp_a1", TenantId = "tenant_001", CompanyName = "企业A北京公司", ShortName = "A-北京", TaxNumber = "91110000A0000001X", Enable = true, Sort = 0, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
            new() { Id = "comp_a2", TenantId = "tenant_001", CompanyName = "企业A上海公司", ShortName = "A-上海", TaxNumber = "91310000A0000002X", Enable = true, Sort = 1, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
            new() { Id = "comp_b1", TenantId = "tenant_002", CompanyName = "企业B杭州公司（内网）", ShortName = "B-杭州", TaxNumber = "91330000B0000001X", Enable = true, Sort = 0, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
        };

        context.Companies.AddRange(companies);

        // 用户公司关联：admin 同时访问云端平台主体 + 内网企业B杭州公司（跨租户）
        var userCompanies = new List<UserCompany>
        {
            new() { Id = "ucp_1", UserId = "1", CompanyId = "comp_p1", TenantId = "platform", CancelFlag = false, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
            new() { Id = "ucp_2", UserId = "1", CompanyId = "comp_b1", TenantId = "tenant_002", CancelFlag = false, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
            new() { Id = "ucp_3", UserId = "2", CompanyId = "comp_a1", TenantId = "tenant_001", CancelFlag = false, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
            new() { Id = "ucp_4", UserId = "2", CompanyId = "comp_a2", TenantId = "tenant_001", CancelFlag = false, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
            new() { Id = "ucp_5", UserId = "3", CompanyId = "comp_b1", TenantId = "tenant_002", CancelFlag = false, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
            new() { Id = "ucp_6", UserId = "4", CompanyId = "comp_a1", TenantId = "tenant_001", CancelFlag = false, CreateTime = DateTime.UtcNow, UpdateTime = DateTime.UtcNow },
        };

        context.UserCompanies.AddRange(userCompanies);

        await context.SaveChangesAsync();
    }
}
