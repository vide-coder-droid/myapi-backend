using Microsoft.AspNetCore.Mvc;
using MyAPI.Models;
using MyAPI.Repositories;
using MyAPI.Services;

namespace MyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _repo;
    private readonly JwtService _jwt;

    public AuthController(IUserRepository repo, JwtService jwt)
    {
        _repo = repo;
        _jwt = jwt;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
        {
            return BadRequest(new
            {
                success = false,
                message = "Username or password is required"
            });
        }

        var user = _repo.GetUser(req.Username, req.Password);

        if (user == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "Invalid username or password"
            });
        }

        var accessToken = _jwt.GenerateToken(user);

        return Ok(new
        {
            accessToken = accessToken,
            expiresIn = 7200, // seconds = 2 hours
            success = true,
            message = "Login successful",
        });
    }
}