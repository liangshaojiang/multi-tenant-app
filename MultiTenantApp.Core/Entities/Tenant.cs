namespace MultiTenantApp.Core.Entities;

public class Tenant
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ShortName { get; set; }
    public string? Logo { get; set; }
    public string? Icon { get; set; }
    public string AdminUserId { get; set; } = null!;
    public string? ParentId { get; set; }
    public bool Enable { get; set; }
    public string? Hosts { get; set; }
    public string? ConnectionString { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
