using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GaVL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModsController : ControllerBase
    {
        [HttpPost, Authorize]
        public IActionResult UploadMod()
        {
            // Placeholder for mod upload logic
            return Ok(new { Message = "Mod uploaded successfully." });
        }
    }
}
