using MyAPI.Models.Entities;
using MyAPI.Models.Requests;
using MyAPI.Models.Responses;
using MyAPI.Repositories;

namespace MyAPI.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _repo;

        public ProfileService(IProfileRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<object>> GetMyProfileAsync(string username)
        {
            var user = await _repo.GetUserWithProfileAsync(username);

            if (user == null)
                return ApiResponse<object>.Fail("User not found");

            return ApiResponse<object>.Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                roles = user.UserRoles.Select(r => r.Role.Name),

                profile = new
                {
                    user.Profile?.FullName,
                    user.Profile?.Bio,
                    user.Profile?.AvatarUrl,
                    user.Profile?.Birthday,
                    user.Profile?.Gender,
                    user.Profile?.Phone,
                    user.Profile?.Address
                }
            }, "Get profile successful");
        }

        public async Task<ApiResponse<object>> UpdateProfileAsync(string username, UpdateProfileRequest req)
        {
            var user = await _repo.GetUserWithProfileAsync(username);

            if (user == null)
                return ApiResponse<object>.Fail("User not found");

            if (user.Profile == null)
            {
                user.Profile = new UserProfile
                {
                    UserId = user.Id
                };
            }

            user.Profile.FullName = req.FullName;
            user.Profile.Bio = req.Bio;
            user.Profile.AvatarUrl = req.AvatarUrl;
            user.Profile.Birthday = req.Birthday.HasValue ? DateTime.SpecifyKind(req.Birthday.Value, DateTimeKind.Utc) : null;
            user.Profile.Gender = req.Gender;
            user.Profile.Phone = req.Phone;
            user.Profile.Address = req.Address;

            await _repo.SaveAsync();
            return ApiResponse<object>.Ok(new { message = "Profile updated successfully" });
        }
    }
}