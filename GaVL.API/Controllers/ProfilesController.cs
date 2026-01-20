using GaVL.Application.Profiles;
using GaVL.DTO.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [HttpPost("avatar"), Authorize]
        public async Task<IActionResult> UploadAvatar([FromForm] AvatarRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized();
            }
            var result = await _profileService.UploadAvatarAsync(userId.Value, request);
            return Ok(result);
        }
    }
}
