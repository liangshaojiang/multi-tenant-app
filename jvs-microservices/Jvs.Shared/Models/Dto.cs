namespace Jvs.Shared.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "ok") => new()
    {
        Success = true,
        Message = message,
        Data = data
    };

    public static ApiResponse<T> Fail(string message) => new()
    {
        Success = false,
        Message = message,
        Data = default
    };
}

public class TenantInfo
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ShortName { get; set; }
    public bool Enable { get; set; }
}

public class UserInfo
{
    public string Id { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string RealName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string TenantId { get; set; } = null!;
    public string TenantName { get; set; } = null!;
    public List<TenantInfo> Tenants { get; set; } = new();
}
