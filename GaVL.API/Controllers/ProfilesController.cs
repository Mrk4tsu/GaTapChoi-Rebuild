using GaVL.Application.Profiles;
using GaVL.DTO.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaVL.API.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfilesController : BasesController
    {
        private readonly IProfileService _profileService;
        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProfile(string username)
        {
            var result = await _profileService.GetUserByUsername(username);
            return Ok(result);
        }
        [HttpPost("avatar"), Authorize]
        public async Task<IActionResult> UploadAvatar([FromForm] AvatarRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized("Mạnh gay");
            }
            var result = await _profileService.UploadAvatar(userId.Value, request);
            return Ok(result);
        }
        [HttpPut("fullname"), Authorize]
        public async Task<IActionResult> UpdateFullname(string newName)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized();
            }
            var result = await _profileService.UpdateFullname(userId.Value, newName);
            return Ok(result);
        }
        [HttpGet("test"), Authorize]
        public async Task<IActionResult> TestEndpoint()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized();
            }
            return Ok(new { message = "Test endpoint is working!", userId = userId.Value });
        }
    }
}
