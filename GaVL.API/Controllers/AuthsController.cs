using GaVL.Application.Auths;
using GaVL.DTO.Auths;
using Microsoft.AspNetCore.Mvc;

namespace GaVL.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthsController : ControllerBase
    {      
        private readonly IAuthService _authService;
        private readonly ILogger<AuthsController> _logger;       
        public AuthsController(IAuthService authService, ILogger<AuthsController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.Register(request);
            if (result.Success)
                return Ok(result);
            else
            {
                _logger.LogWarning("Registration failed: {Message}", result.Message);
                return BadRequest(result);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.Login(request);
            if (result.Success)
                return Ok(result);
            else
            {
                _logger.LogWarning("Login failed for user {Username}: {Message}", request.Username, result.Message);
                return BadRequest(result);
            }
        }
    }
}
