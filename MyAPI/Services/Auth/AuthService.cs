using MyAPI.Models.Entities;
using MyAPI.Models.Requests;
using MyAPI.Models.Responses;
using MyAPI.Repositories;
using System.Security.Claims;

namespace MyAPI.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly JwtService _jwt;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository repo, JwtService jwt, IConfiguration config)
        {
            _repo = repo;
            _jwt = jwt;
            _config = config;
        }

        public async Task<ApiResponse<object>> LoginAsync(LoginRequest req)
        {
            var user = await _repo.GetUserAsync(req.Username);

            if (user == null)
                return ApiResponse<object>.Fail("Invalid username or password");

            if (!user.IsActive)
                return ApiResponse<object>.Fail("Account disabled");

            bool valid = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);

            if (!valid)
                return ApiResponse<object>.Fail("Invalid username or password");

            var token = _jwt.GenerateToken(user);

            int expire = int.Parse(_config["Jwt:ExpireSeconds"] ?? "7200");

            return ApiResponse<object>.Ok(new
            {
                accessToken = token,
                expiresIn = expire,
                user = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    avatar = user.Profile?.AvatarUrl,
                    roles = user.UserRoles.Select(r => r.Role.Name)
                }
            });
        }

        public async Task<ApiResponse<object>> CreateUserAsync(CreateUserRequest req, ClaimsPrincipal currentUser)
        {
            string role = "User"; // mặc định

            if (!string.IsNullOrEmpty(req.Role) && req.Role == "Admin")
            {
                var currentRole = currentUser.FindFirst(ClaimTypes.Role)?.Value;

                if (currentRole != "Admin")
                {
                    return ApiResponse<object>.Fail("Only admin can create admin account");
                }

                role = "Admin";
            }

            var user = new User
            {
                Username = req.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Email = req.Email,
                IsActive = true
            };

            await _repo.AddUserAsync(user, role);

            return ApiResponse<object>.Ok(new { message = "User created" });
        }

        public async Task<ApiResponse<object>> RegisterAsync(RegisterRequest req)
        {
            var exist = await _repo.GetUserAsync(req.Username);

            if (exist != null)
                return ApiResponse<object>.Fail("Username already exists");

            var user = new User
            {
                Username = req.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Email = req.Email,
                IsActive = true
            };

            await _repo.AddUserAsync(user, "User");

            return ApiResponse<object>.Ok(new { message = "Account created" });
        }

        public async Task<ApiResponse<object>> DeleteUserAsync(string username, string currentUser, bool isAdmin)
        {
            if (!isAdmin && username != currentUser)
                return ApiResponse<object>.Fail("Forbidden");

            if (isAdmin && username == currentUser)
                return ApiResponse<object>.Fail("Admin cannot delete self");

            var user = await _repo.GetUserAsync(username);

            if (user == null)
                return ApiResponse<object>.Fail("User not found");

            await _repo.DisableUserAsync(user);
            return ApiResponse<object>.Ok(new { message = "User disabled" });
        }
    }
}