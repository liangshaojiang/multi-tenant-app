namespace MultiTenantApp.Core.Models;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = "MultiTenantApp";
    public string Audience { get; set; } = "MultiTenantApp.Client";
    public string SecretKey { get; set; } = "ReplaceThisWithAStrongSecretKey1234567890";
    public int ExpireMinutes { get; set; } = 120;
}
