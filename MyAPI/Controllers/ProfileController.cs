using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Models.Requests;
using MyAPI.Services.Profile;

namespace MyAPI.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _service;

        public ProfileController(IProfileService service)
        {
            _service = service;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var result = await _service.GetMyProfileAsync(username);

            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest req)
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var result = await _service.UpdateProfileAsync(username, req);

            return Ok(result);
        }
    }
}