using MyAPI.Models.Requests;
using MyAPI.Models.Responses;
using System.Security.Claims;

namespace MyAPI.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<object>> LoginAsync(LoginRequest req);

        Task<ApiResponse<object>> CreateUserAsync(CreateUserRequest req, ClaimsPrincipal currentUser);
        Task<ApiResponse<object>> RegisterAsync(RegisterRequest req);
        Task<ApiResponse<object>> DeleteUserAsync(string username, string currentUser, bool isAdmin);
    }
}