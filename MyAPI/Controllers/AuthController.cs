using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Services.Auth;
using MyAPI.Models.Requests;

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
        var result = await _service.LoginAsync(req);

        if (!result.Success)
            return Unauthorized(result);

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