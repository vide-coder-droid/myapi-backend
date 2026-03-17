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
        private readonly EmailService _emailService;
        private readonly OtpService _otpService;

        public AuthService(IUserRepository repo, JwtService jwt, IConfiguration config, EmailService emailService, OtpService otpService)
        {
            _repo = repo;
            _jwt = jwt;
            _config = config;
            _emailService = emailService;
            _otpService = otpService;
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
            }, "Login successful");
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

        public async Task<ApiResponse<object>> SendOtpAsync(SendOtpRequest req)
        {
            var exist = await _repo.GetByEmailAsync(req.Email);
            if (exist != null)
                return ApiResponse<object>.Fail("Email đã tồn tại");

            var otp = await _otpService.GenerateOtpAsync(req.Email);

            _ = Task.Run(() => _emailService.SendOtpEmail(req.Email, otp));

            return ApiResponse<object>.Ok(null, "OTP sent");
        }

        public async Task<ApiResponse<object>> VerifyOtpAsync(VerifyOtpRequest req)
        {
            var valid = await _otpService.VerifyOtpAsync(req.Email, req.Otp);

            if (!valid)
                return ApiResponse<object>.Fail("OTP không đúng");

            var token = await _otpService.GenerateRegisterTokenAsync(req.Email);

            return ApiResponse<object>.Ok(new { token });
        }

        public async Task<ApiResponse<object>> RegisterAsync(RegisterRequest req)
        {
            var email = await _otpService.GetEmailFromTokenAsync(req.Token);

            if (email == null)
                return ApiResponse<object>.Fail("Chưa verify OTP");

            var exist = await _repo.GetUserAsync(req.Username);
            if (exist != null)
                return ApiResponse<object>.Fail("Username đã tồn tại");

            var user = new User
            {
                Username = req.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Email = email,
                IsActive = true
            };

            await _repo.AddUserAsync(user, "User");

            await _otpService.RemoveTokenAsync(req.Token);

            return ApiResponse<object>.Ok(null, "Account created");
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