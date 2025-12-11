using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GaVL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasesController : ControllerBase
    {
        protected Guid? GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }

            return null;
        }
    }
}
