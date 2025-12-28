using GaVL.Application.Catalog.Mods;
using GaVL.DTO.Mods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaVL.API.Controllers
{
    [Route("api/mod")]
    [ApiController]
    public class ModsController : BasesController
    {
        private readonly IModService _modService;
        private readonly ILogger<ModsController> _logger;
        public ModsController(IModService modService, ILogger<ModsController> logger)
        {
            _modService = modService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetMods([FromQuery] ModQueryRequest request)
        {
            var result = await _modService.GetMods(request);
            return Ok(result);
        }
        [HttpGet("{modId}")]
        public async Task<IActionResult> GetModById(int modId)
        {
            try
            {
                var result = await _modService.GetModById(modId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting mod {ModId}", modId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpGet("seo")]
        public async Task<IActionResult> GetSeoMod(int modId)
        {
            var result = await _modService.GetSeoModById(modId);
            return Ok(result);
        }
        [HttpGet("inner")]
        public async Task<IActionResult> GetInternalMod(int modId)
        {
            var result = await _modService.GetModInternalById(modId);
            return Ok(result);
        }
        [HttpPost, Authorize]
        public async Task<IActionResult> CreateMod(ModCombineRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _modService.CreateMod(request, userId.Value);
            return Ok(result);
        }
        [HttpPost("crack"), AllowAnonymous]
        public async Task<IActionResult> CreateCrack(ModCombineRequest request, string key, byte type)
        {
            var result = await _modService.CreateCrack(request, key, type);
            return Ok(result);
        }
        [HttpPut, Authorize]
        public async Task<IActionResult> UpdateMod(int modId, ModUpdateCombineRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _modService.UpdateMod(modId, request, userId.Value);
            return Ok(result);
        }
        [HttpPut("lock"), Authorize]
        public async Task<IActionResult> UpdateStatus(int modId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _modService.UpdateStatus(modId, userId.Value);
            return Ok(result);
        }
        [HttpDelete, Authorize]
        public async Task<IActionResult> DeleteMod(int modId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _modService.DeleteMod(modId, userId.Value);
            return Ok(result);
        }
    }
}
