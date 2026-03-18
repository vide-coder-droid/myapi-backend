using MyAPI.Models.Entities;
using MyAPI.Models.Requests;
using MyAPI.Models.Responses;
using System.Security.Claims;

namespace MyAPI.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<object>> LoginAsync(LoginRequest req, string deviceName, string ip);

        Task<UserDevice?> GetDeviceByRefreshTokenAsync(string token);

        Task<ApiResponse<object>> RefreshTokenAsync(string refreshToken, string deviceName, string ip);

        Task<ApiResponse<object>> RevokeTokenAsync(string refreshToken, string currentUser, bool isAdmin);

        Task<IEnumerable<object>> GetUserDevicesAsync(Guid userId);

        Task<ApiResponse<object>> CreateUserAsync(CreateUserRequest req, ClaimsPrincipal currentUser);

        Task<ApiResponse<object>> SendOtpAsync(SendOtpRequest req);

        Task<ApiResponse<object>> VerifyOtpAsync(VerifyOtpRequest req);

        Task<ApiResponse<object>> RegisterAsync(RegisterRequest req);

        Task<ApiResponse<object>> DeleteUserAsync(string username, string currentUser, bool isAdmin);

        Task<User?> GetUserByUsernameAsync(string username);
        Task<ApiResponse<object>> VerifyOtpForDeviceAsync(VerifyOtpRequest req);
        
    }
}