using GaVL.Application.Auths;
using GaVL.DTO.Auths;
using GaVL.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GaVL.API.Controllers
{
    [Route("api/auth"), AllowAnonymous]
    [ApiController]
    public class AuthsController : BasesController
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthsController> _logger;
        public AuthsController(IAuthService authService, ILogger<AuthsController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
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
        [EnableRateLimiting(SystemConstant.POLICY_LOGIN_IP)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.Login(request, GetClientIpAddress());
            if (result.Success)
                return Ok(result);
            else
            {
                _logger.LogWarning("Login failed for user {Username}: {Message}", request.Username, result.Message);
                return BadRequest(result);
            }
        }
        [HttpPost("google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginDto request)
        {
            var result = await _authService.LoginWithGoogleAsync(request);
            if (result.Success)
                return Ok(result);
            else
            {
                _logger.LogWarning("Google login failed for token {Token}: {Message}", request.AccessToken, result.Message);
                return BadRequest(result);
            }
        }
        [HttpPost("admin")]
        public async Task<IActionResult> LoginDashboard([FromBody] LoginDashboardRequest request)
        {
            var result = await _authService.LoginDashboard(request);
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
                _logger.LogWarning("Forgot password failed for email {Email}: {Message}", request.Username, result.Message);
                return BadRequest(result);
            }
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPassword(request);
            if (result.Success)
                return Ok(result);
            else
            {
                _logger.LogWarning("Reset password failed for email {Email}: {Message}", request.Email, result.Message);
                return BadRequest(result);
            }
        }
        [HttpPost("change-password"), Authorize]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                _logger.LogWarning("Change password attempted without valid user ID in claims.");
                return Unauthorized(new { message = "Unauthorized" });
            }
            var result = await _authService.ChangePassword(userId.Value, oldPassword, newPassword);
            if (result.Success)
                return Ok(result);
            else
            {
                _logger.LogWarning("Change password failed for user ID {UserId}: {Message}", userId, result.Message);
                return BadRequest(result);
            }
        }
        [HttpGet("ip")]
        public IActionResult GetClientIp()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Xử lý trường hợp có proxy/load balancer
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
            }
            else if (Request.Headers.ContainsKey("X-Real-IP"))
            {
                ipAddress = Request.Headers["X-Real-IP"].ToString();
            }

            // Loại bỏ port nếu có
            if (!string.IsNullOrEmpty(ipAddress) && ipAddress.Contains(':'))
            {
                ipAddress = ipAddress.Split(':')[0];
            }

            return Ok(ipAddress);
        }
        [NonAction]
        public string GetClientIpAddress()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            // Xử lý trường hợp có proxy/load balancer
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
            }
            else if (Request.Headers.ContainsKey("X-Real-IP"))
            {
                ipAddress = Request.Headers["X-Real-IP"].ToString();
            }
            
            // Loại bỏ port nếu có
            if (!string.IsNullOrEmpty(ipAddress) && ipAddress.Contains(':'))
            {
                ipAddress = ipAddress.Split(':')[0];
            }
            
            return ipAddress ?? string.Empty;
        }

    }
}
