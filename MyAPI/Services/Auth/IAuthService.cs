using MyAPI.Models.Entities;
using MyAPI.Models.Requests;
using MyAPI.Models.Responses;
using System.Security.Claims;

namespace MyAPI.Services.Auth
{
    public interface IAuthService
    {
        // =========================
        // 1. Authentication
        // =========================
        Task<ApiResponse<object>> LoginAsync(LoginRequest req, string deviceName, string ip);
        Task<ApiResponse<object>> RefreshTokenAsync(string refreshToken, string deviceName, string ip);
        Task<ApiResponse<object>> RevokeTokenAsync(string refreshToken, string currentUser, bool isAdmin);

        // =========================
        // 2. User Management
        // =========================
        Task<ApiResponse<object>> CreateUserAsync(CreateUserRequest req, ClaimsPrincipal currentUser);
        Task<ApiResponse<object>> DeleteUserAsync(string username, string currentUser, bool isAdmin);
        Task<User?> GetUserByUsernameAsync(string username);

        // =========================
        // 3. OTP & Registration
        // =========================
        Task<ApiResponse<object>> SendOtpAsync(SendOtpRequest req);
        Task<ApiResponse<object>> VerifyOtpAsync(VerifyOtpRequest req);
        Task<ApiResponse<object>> VerifyOtpForDeviceAsync(VerifyOtpRequest req);
        Task<ApiResponse<object>> RegisterAsync(RegisterRequest req);

        // =========================
        // 4. Device Management
        // =========================
        Task<UserDevice?> GetDeviceByRefreshTokenAsync(string token);
        Task<IEnumerable<object>> GetUserDevicesAsync(Guid userId);
    }
}