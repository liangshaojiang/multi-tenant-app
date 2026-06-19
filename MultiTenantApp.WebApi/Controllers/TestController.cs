using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantApp.WebApi.Models;

namespace MultiTenantApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpPost("bcrypt-verify")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<object>> TestBcrypt([FromBody] TestBcryptRequest request)
    {
        try
        {
            var result = BCrypt.Net.BCrypt.Verify(request.Password, request.Hash);
            return Ok(ApiResponse<object>.Ok(new
            {
                matches = result,
                password = request.Password,
                hash = request.Hash
            }));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse<object>.Ok(new
            {
                matches = false,
                error = ex.Message,
                password = request.Password,
                hash = request.Hash
            }));
        }
    }

    [HttpPost("bcrypt-hash")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<object>> GenerateHash([FromBody] GenerateHashRequest request)
    {
        try
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var verify = BCrypt.Net.BCrypt.Verify(request.Password, hash);
            return Ok(ApiResponse<object>.Ok(new
            {
                password = request.Password,
                hash = hash,
                verifyWorks = verify
            }));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse<object>.Fail(ex.Message));
        }
    }
}

public class TestBcryptRequest
{
    public string Password { get; set; } = null!;
    public string Hash { get; set; } = null!;
}

public class GenerateHashRequest
{
    public string Password { get; set; } = null!;
}
