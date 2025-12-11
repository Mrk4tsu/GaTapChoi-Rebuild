using GaVL.Application.Catalog.Mods;
using GaVL.DTO.Mods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GaVL.API.Controllers
{
    [Route("api/mod")]
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
        [HttpGet("get")]
        public async Task<IActionResult> GetModById(int modId)
        {
            var result = await _modService.GetModById(modId);
            return Ok(result);
        }
        [HttpGet("seo")]
        public async Task<IActionResult> GetModBySeoAlias(int modId)
        {
            var result = await _modService.GetSeoModById(modId);
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
        [HttpPut, Authorize]
        public async Task<IActionResult> UpdateMod(int modId, ModUpdateCombineRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _modService.UpdateMod(modId, request, userId.Value);
            return Ok(result);
        }
    }
}
