using MyAPI.Models.Requests;
using MyAPI.Models.Responses;

namespace MyAPI.Services.Profile
{
    public interface IProfileService
    {
        Task<ApiResponse<object>> GetMyProfileAsync(string username);

        Task<ApiResponse<object>> UpdateProfileAsync(string username, UpdateProfileRequest req);
    }
}