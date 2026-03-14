using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Models.Entities;
using MyAPI.Models.Requests;
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
    [AllowAnonymous]
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


    [Authorize(Roles = "Admin")]
    [HttpPost("create-user")]
    public IActionResult CreateUser([FromBody] CreateUserRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
        {
            return BadRequest("Username and Password required");
        }

        // check user tồn tại
        var existing = _repo.GetByUsername(req.Username);
        if (existing != null)
        {
            return BadRequest("User already exists");
        }

        var user = new User
        {
            Username = req.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Email = req.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _repo.AddUser(user, req.Role);

        return Ok(new
        {
            success = true,
            message = "User created"
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete-user/{username}")]
    public IActionResult DeleteUser(string username)
    {
        var user = _repo.GetByUsername(username);

        if (user == null)
        {
            return NotFound(new
            {
                success = false,
                message = "User not found"
            });
        }

        _repo.DeleteUser(user);

        return Ok(new
        {
            success = true,
            message = "User deleted"
        });
    }
}