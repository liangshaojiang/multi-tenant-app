namespace Jvs.Shared.Authentication;

public class JwtSettings
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = "Jvs.AuthService";
    public string Audience { get; set; } = "Jvs.Client";
    // HMAC 模式（向后兼容，开发用）
    public string SecretKey { get; set; } = "";
    // RSA 非对称模式（生产用）
    public string? PrivateKey { get; set; }
    public string? PublicKey { get; set; }
    public int ExpireMinutes { get; set; } = 120;
    public int RefreshExpireDays { get; set; } = 7;

    public bool UseRsa => !string.IsNullOrEmpty(PrivateKey) || !string.IsNullOrEmpty(PublicKey);
}

public static class AuthConstants
{
    public const string TenantIdClaim = "tenant_id";
    public const string TenantNameClaim = "tenant_name";
    public const string CompanyIdClaim = "company_id";
    public const string CompanyNameClaim = "company_name";
}
