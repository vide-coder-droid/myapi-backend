using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Services.Auth;
using MyAPI.Models.Requests;
using MyAPI.Models.Responses;
using System.Security.Claims;

namespace MyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var deviceName = Request.Headers["User-Agent"].ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var result = await _service.LoginAsync(req, deviceName, ip);
        if (!result.Success) return Unauthorized(result);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("verify-otp-device")]
    public async Task<IActionResult> VerifyOtpForDevice(VerifyOtpRequest req)
    {
        var result = await _service.VerifyOtpForDeviceAsync(req);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var deviceName = Request.Headers["User-Agent"].ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var result = await _service.RefreshTokenAsync(refreshToken, deviceName, ip);
        if (!result.Success) return Unauthorized(result);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("sessions")]
    public async Task<IActionResult> Sessions()
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub")
               ?? User.FindFirstValue("username");

        if (string.IsNullOrEmpty(username))
            return BadRequest("User claim missing");

        var user = await _service.GetUserByUsernameAsync(username); // dùng service thay vì _userRepository
        if (user == null)
            return NotFound("User not found");

        var sessions = await _service.GetUserDevicesAsync(user.Id);
        return Ok(ApiResponse<object>.Ok(sessions));
    }

    [Authorize]
    [HttpPost("logout-device")]
    public async Task<IActionResult> LogoutDevice([FromBody] string refreshToken)
    {
        var currentUser = User.FindFirstValue("username")!;
        bool isAdmin = User.IsInRole("Admin");

        var result = await _service.RevokeTokenAsync(refreshToken, currentUser, isAdmin);
        return Ok(result);
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser(CreateUserRequest req)
    {
        var result = await _service.CreateUserAsync(req, User);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp(SendOtpRequest req)
    {
        var result = await _service.SendOtpAsync(req);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(VerifyOtpRequest req)
    {
        var result = await _service.VerifyOtpAsync(req);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var result = await _service.RegisterAsync(req);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("delete-user/{username}")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        var currentUser = User.Identity?.Name ?? "";
        var isAdmin = User.IsInRole("Admin");

        var result = await _service.DeleteUserAsync(username, currentUser, isAdmin);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}