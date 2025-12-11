using GaVL.Application.Catalog.Mods;
using GaVL.DTO.Mods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GaVL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModsController : BasesController
    {
        private readonly IModService _modService;
        public ModsController(IModService modService)
        {
            _modService = modService;
        }
        [HttpGet]
        public async Task<IActionResult> GetMods([FromQuery] ModQueryRequest request)
        {
            var result = await _modService.GetMods(request);
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
    }
}
