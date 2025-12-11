using GaVL.Application.Catalog.Mods;
using GaVL.DTO.Mods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GaVL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModsController : ControllerBase
    {
        private readonly IModService _modService;
        public ModsController(IModService modService)
        {
            _modService = modService;
        }
        [HttpGet]
        public async Task<IActionResult> GetMods([FromQuery] ModQueryRequest request)
        {
            var result = await _modService.GetModsAsync(request);
            return Ok(result);
        }
    }
}
