using Jvs.Shared.Models;

namespace Jvs.AuthService.Models;

public class UserListItem
{
    public string Id { get; set; } = "";
    public string AccountName { get; set; } = "";
    public string RealName { get; set; } = "";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool CancelFlag { get; set; }
    public byte UserType { get; set; }
    public string? DeptId { get; set; }
    public string? DeptName { get; set; }
}

public class CreateUserRequest
{
    public string AccountName { get; set; } = "";
    public string RealName { get; set; } = "";
    public string Password { get; set; } = "";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? DeptId { get; set; }
}
