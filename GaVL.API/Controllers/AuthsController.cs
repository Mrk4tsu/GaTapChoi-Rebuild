using GaVL.Application.Auths;
using GaVL.DTO.Auths;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaVL.API.Controllers
{
    [Route("api/auth"), AllowAnonymous]
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
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var result = await _authService.Refresh(request);
            if (result.Success)
                return Ok(result);
            else
            {
                _logger.LogWarning("Token refresh failed: {Message}", result.Message);
                return BadRequest(result);
            }
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var result = await _authService.Logout(request);
            if (result.Success)
                return Ok(result);
            else
            {
                _logger.LogWarning("Logout failed: {Message}", result.Message);
                return BadRequest(result);
            }
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _authService.ForgotPassword(request);
            if (result.Success)
                return Ok(result);
            else
            {
                _logger.LogWarning("Forgot password failed for email {Email}: {Message}", request.Email, result.Message);
                return BadRequest(result);
            }
        }
    }
}
