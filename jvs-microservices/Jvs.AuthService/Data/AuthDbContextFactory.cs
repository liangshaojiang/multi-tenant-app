using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Jvs.AuthService.Data;

public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        optionsBuilder.UseMySql("server=127.0.0.1;port=3306;database=jvs_auth;user=root;password=123456;CharSet=utf8mb4;AllowPublicKeyRetrieval=True;SslMode=None",
            ServerVersion.AutoDetect("server=127.0.0.1;port=3306;database=jvs_auth;user=root;password=123456;CharSet=utf8mb4;AllowPublicKeyRetrieval=True;SslMode=None"));
        return new AuthDbContext(optionsBuilder.Options);
    }
}
