using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Jvs.Shared.Authentication;

namespace Jvs.Shared.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(JwtSettings.SectionName);
        var settings = new JwtSettings
        {
            Issuer = section["Issuer"] ?? "Jvs.AuthService",
            Audience = section["Audience"] ?? "Jvs.Client",
            SecretKey = section["SecretKey"] ?? "",
            PrivateKey = section["PrivateKey"],
            PublicKey = section["PublicKey"],
            ExpireMinutes = int.TryParse(section["ExpireMinutes"], out var exp) ? exp : 120,
            RefreshExpireDays = int.TryParse(section["RefreshExpireDays"], out var refExp) ? refExp : 7,
        };

        services.Configure<JwtSettings>(section);

        SecurityKey signingKey;
        if (settings.UseRsa)
        {
            // RSA 非对称模式：验签用公钥（云端放私钥签，内网放公钥验）
            var rsa = RSA.Create();
            rsa.ImportFromPem(settings.PublicKey!.ToCharArray());
            signingKey = new RsaSecurityKey(rsa) { KeyId = "rsa-prod" };
        }
        else
        {
            // HMAC 对称模式（开发环境向后兼容）
            signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = signingKey,
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role
                };
            });

        services.AddAuthorization();

        return services;
    }
}
