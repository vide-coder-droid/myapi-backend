using MyAPI.Models.Entities;
using MyAPI.Models.Requests;
using MyAPI.Models.Responses;
using MyAPI.Repositories;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace MyAPI.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly JwtService _jwt;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;
        private readonly OtpService _otpService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IUserRepository repo, JwtService jwt, IConfiguration config, EmailService emailService, OtpService otpService, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _jwt = jwt;
            _config = config;
            _emailService = emailService;
            _otpService = otpService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            RandomNumberGenerator.Fill(randomBytes); // thay cho RNGCryptoServiceProvider
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<ApiResponse<object>> LoginAsync(LoginRequest req, string deviceName, string ip)
        {
            // 1. Lấy user
            var user = await _repo.GetUserAsync(req.Username);
            if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                return ApiResponse<object>.Fail("Invalid username or password");

            // 3. Kiểm tra thiết bị có tồn tại chưa
            var existingDevice = await _repo.GetUserDeviceByUserAndDeviceAsync(user.Id, deviceName, ip);

            if (existingDevice != null)
            {
                // Thiết bị cũ → update refresh token, last active
                existingDevice.RefreshToken = GenerateRefreshToken();
                existingDevice.ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshExpireDays"] ?? "7"));
                existingDevice.LastActive = DateTime.UtcNow;
                existingDevice.IsRevoked = false;
                await _repo.UpdateUserDeviceAsync(existingDevice);

                var accessToken = _jwt.GenerateToken(user);

                return ApiResponse<object>.Ok(new
                {
                    accessToken,
                    refreshToken = existingDevice.RefreshToken,
                    expiresIn = int.Parse(_config["Jwt:ExpireSeconds"] ?? "7200")
                }, "Login successful");
            }
            else
            {
                // Thiết bị mới → gửi OTP device
                return await SendOtpForDeviceAsync(user.Email);
            }
        }

        // Hàm tái sử dụng gửi OTP cho thiết bị mới
        private async Task<ApiResponse<object>> SendOtpForDeviceAsync(string email)
        {
            // Redis đã handle cooldown + attempt → tránh spam
            var otpResult = await _otpService.GenerateOtpAsync(email);
            if (!otpResult.Success)
                return ApiResponse<object>.Fail(otpResult.Message);

            // Gửi email OTP với type = device
            _ = Task.Run(() => _emailService.SendOtpEmail(email, otpResult.Otp!, type: "device"));

            return ApiResponse<object>.Ok(new
            {
                requireOtp = true,
                message = "OTP sent to your email to verify new device"
            });
        }

        public async Task<ApiResponse<object>> VerifyOtpForDeviceAsync(VerifyOtpRequest req)
        {
            // 1. Verify OTP
            var valid = await _otpService.VerifyOtpAsync(req.Email, req.Otp);
            if (!valid) return ApiResponse<object>.Fail("OTP không đúng");

            // 2. Lấy user
            var user = await _repo.GetByEmailAsync(req.Email);
            if (user == null) return ApiResponse<object>.Fail("User not found");

            // 3. Lấy device info
            var deviceName = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "unknown device";
            var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // 4. Tạo device mới trong DB
            var refreshToken = GenerateRefreshToken();
            var refreshExpireDays = int.Parse(_config["Jwt:RefreshExpireDays"] ?? "7");
            var device = new UserDevice
            {
                UserId = user.Id,
                DeviceName = deviceName,
                IpAddress = ip,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshExpireDays)
            };
            await _repo.AddUserDeviceAsync(device);

            // 5. Trả access token
            var accessToken = _jwt.GenerateToken(user);
            int expire = int.Parse(_config["Jwt:ExpireSeconds"] ?? "7200");

            return ApiResponse<object>.Ok(new
            {
                accessToken,
                refreshToken,
                expiresIn = expire
            }, "Device verified and saved, login successful");
        }

        public async Task<UserDevice?> GetDeviceByRefreshTokenAsync(string token)
        {
            return await _repo.GetUserDeviceByRefreshTokenAsync(token);
        }

        public string GenerateJwtForUser(User user) => _jwt.GenerateToken(user);

        public async Task<string> RotateRefreshTokenAsync(UserDevice device)
        {
            var newToken = GenerateRefreshToken();
            device.RefreshToken = newToken;
            device.ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshExpireDays"] ?? "7"));
            device.LastActive = DateTime.UtcNow;
            await _repo.RevokeDeviceAsync(device);
            return newToken;
        }

        public async Task<IEnumerable<object>> GetUserDevicesAsync(Guid userId)
        {
            var devices = await _repo.GetUserDevicesAsync(userId);
            return devices.Select(d => new { d.Id, d.DeviceName, d.IpAddress, d.LastActive, d.ExpiresAt });
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _repo.GetUserAsync(username);
        }

        public async Task<ApiResponse<object>> RefreshTokenAsync(string refreshToken, string deviceName, string ip)
        {
            var device = await _repo.GetUserDeviceByRefreshTokenAsync(refreshToken);
            if (device == null || device.ExpiresAt < DateTime.UtcNow)
                return ApiResponse<object>.Fail("Invalid or expired refresh token");

            var accessToken = GenerateJwtForUser(device.User);
            var newRefreshToken = await RotateRefreshTokenAsync(device);

            return ApiResponse<object>.Ok(new
            {
                accessToken,
                refreshToken = newRefreshToken,
                expiresIn = int.Parse(_config["Jwt:ExpireSeconds"] ?? "7200")
            });
        }

        public async Task<ApiResponse<object>> RevokeTokenAsync(string refreshToken, string currentUser, bool isAdmin)
        {
            var device = await _repo.GetUserDeviceByRefreshTokenAsync(refreshToken);
            if (device == null)
                return ApiResponse<object>.Fail("Device not found");

            if (!isAdmin && device.User.Username != currentUser)
                return ApiResponse<object>.Fail("Forbidden");

            await _repo.RevokeDeviceAsync(device);
            return ApiResponse<object>.Ok(null, "Token revoked");
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
            try
            {
                var exist = await _repo.GetByEmailAsync(req.Email);
                if (exist != null)
                    return ApiResponse<object>.Fail("Email đã tồn tại");

                var result = await _otpService.GenerateOtpAsync(req.Email);

                if (!result.Success)
                {
                    return ApiResponse<object>.Fail(result.Message);
                }

                _ = Task.Run(() => _emailService.SendOtpEmail(req.Email, result.Otp!));

                return ApiResponse<object>.Ok(null, "OTP sent");
            }
            catch
            {
                return ApiResponse<object>.Fail("Lỗi hệ thống, vui lòng thử lại");
            }
        }

        public async Task<ApiResponse<object>> VerifyOtpAsync(VerifyOtpRequest req)
        {
            var valid = await _otpService.VerifyOtpAsync(req.Email, req.Otp);

            if (!valid)
                return ApiResponse<object>.Fail("OTP không đúng");

            var token = await _otpService.GenerateRegisterTokenAsync(req.Email);

            return ApiResponse<object>.Ok(new { token }, "OTP verified successfully");
        }

        public async Task<ApiResponse<object>> RegisterAsync(RegisterRequest req)
        {
            var email = await _otpService.GetEmailFromTokenAsync(req.Token);

            if (email == null)
                return ApiResponse<object>.Fail("Chưa verify OTP");

            var exist = await _repo.GetUserAsync(req.Username);
            if (exist != null)
                return ApiResponse<object>.Fail("Username đã tồn tại");

            // 1. Tạo user mới
            var user = new User
            {
                Username = req.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Email = email,
                IsActive = true
            };

            await _repo.AddUserAsync(user, "User");

            // 2. Lấy thông tin thiết bị hiện tại từ HttpContext
            var deviceName = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "unknown device";
            var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // 3. Tạo device mặc định cho user mới
            var refreshToken = GenerateRefreshToken();
            var refreshExpireDays = int.Parse(_config["Jwt:RefreshExpireDays"] ?? "7");

            var device = new UserDevice
            {
                UserId = user.Id,
                DeviceName = deviceName,
                IpAddress = ip,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshExpireDays)
            };

            await _repo.AddUserDeviceAsync(device);

            // 4. Xóa token OTP
            await _otpService.RemoveTokenAsync(req.Token);

            // 5. Trả về access + refresh token luôn
            var accessToken = _jwt.GenerateToken(user);
            int expire = int.Parse(_config["Jwt:ExpireSeconds"] ?? "7200");

            return ApiResponse<object>.Ok(new
            {
                accessToken,
                refreshToken,
                expiresIn = expire
            }, "Account created and device added");
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