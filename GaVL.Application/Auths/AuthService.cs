using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.Data.Entities;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Auths;
using GaVL.DTO.Settings;
using GaVL.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace GaVL.Application.Auths
{
    public interface IAuthService
    {
        Task<ApiResult<Guid>> Register(RegisterRequest request);
        Task<ApiResult<TokenResponse>> Login(LoginRequest request);
        Task<ApiResult<TokenResponse>> Refresh(RefreshRequest request);
        Task<ApiResult<bool>> Logout(LogoutRequest request);
        Task<ApiResult<bool>> ForgotPassword(ForgotPasswordRequest request);
        Task<ApiResult<bool>> ResetPassword(ResetPasswordRequest request);
    }
    public class AuthService : IAuthService
    {
        private ILogger<AuthService> _logger;
        private AppDbContext _dbContext;

        private readonly ITurnstileService _turnstileService;
        private readonly JwtSettings _jwtSettings;
        private readonly AppUrlSetting _appUrlSetting;
        private readonly ITokenService _tokenService;
        private readonly IRedisService _redisService;
        private readonly IMailService _mailService;
        public AuthService(AppDbContext dbContext,
            ILogger<AuthService> logger,
            IConfiguration configuration,
            IMailService mailService,
            ITokenService tokenService,
            IRedisService redisService,
            IOptions<JwtSettings> options,
            ITurnstileService turnstileService,
            IOptions<AppUrlSetting> urlSetting)
        {
            _dbContext = dbContext;
            _turnstileService = turnstileService;
            _logger = logger;
            _tokenService = tokenService;
            _jwtSettings = options.Value;
            _appUrlSetting = urlSetting.Value;
            _redisService = redisService;
            _mailService = mailService;
        }

        public async Task<ApiResult<TokenResponse>> Login(LoginRequest request)
        {
            var isValidCaptcha = await _turnstileService.ValidateTokenAsync(request.CaptchaToken);
            if (!isValidCaptcha) return new ApiErrorResult<TokenResponse>("CAPTCHA validation failed.");

            var user = await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return new ApiErrorResult<TokenResponse>("Invalid username or password.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return new ApiErrorResult<TokenResponse>("Invalid username or password.");
            }
            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var sessionId = Guid.NewGuid().ToString();
            var refreshKey = $"refresh:{user.Id}:{sessionId}";
            await _redisService.SetAsync(refreshKey, refreshToken, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays));

            var loginResult = new TokenResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                SessionId = sessionId
            };
            return new ApiSuccessResult<TokenResponse>(loginResult, "Login successful.");
        }

        public async Task<ApiResult<Guid>> Register(RegisterRequest request)
        {
            var isExistUsername = await isExistUsernameInDatabase(request.Username);
            if (isExistUsername) return new ApiErrorResult<Guid>("Username is already taken.");
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                AvatarUrl = $"assets/images/avatars/{request.AvatarIndex}.png",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                LastLoginAt = null
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
            return new ApiSuccessResult<Guid>(newUser.Id, "User registered successfully.");
        }
        private async Task<bool> isExistUsernameInDatabase(string username) => await _dbContext.Users.AnyAsync(u => u.Username == username);

        public async Task<ApiResult<TokenResponse>> Refresh(RefreshRequest request)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));

            var refreshKey = $"refresh:{userId}:{request.SessionId}";
            var storedRefreshToken = await _redisService.GetStringAsync(refreshKey);
            if (storedRefreshToken != request.RefreshToken) return new ApiErrorResult<TokenResponse>("Invalid refresh token.");

            var user = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            var newAccessToken = await _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            await _redisService.SetAsync(refreshKey, newRefreshToken, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays));

            var result = new TokenResponse()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                SessionId = request.SessionId
            };
            return new ApiSuccessResult<TokenResponse>(result, "Token refreshed successfully.");
        }

        public async Task<ApiResult<bool>> Logout(LogoutRequest request)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
            var refreshKey = $"refresh:{userId}:{request.SessionId}";
            await _redisService.RemoveKeyAsync(refreshKey);

            var jwtHandler = new JwtSecurityTokenHandler();
            var tokenExpiry = jwtHandler.ReadJwtToken(request.AccessToken).ValidTo;
            var remainingTime = tokenExpiry - DateTime.UtcNow;
            if (remainingTime > TimeSpan.Zero)
            {
                var blacklistKey = $"blacklist:{request.AccessToken}";
                await _redisService.SetAsync(blacklistKey, "revoked", remainingTime);
            }
            return new ApiSuccessResult<bool>(true, "Logged out successfully.");
        }

        public async Task<ApiResult<bool>> ForgotPassword(ForgotPasswordRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return new ApiSuccessResult<bool>(true, "If the email is registered, a reset link has been sent.");
            var resetToken = GenerateResetToken();
            var resetKey = $"reset:{user.Id}";
            await _redisService.SetAsync(resetKey, resetToken, TimeSpan.FromMinutes(30));

            var appUrl = _appUrlSetting.Forum;
            var resetLink = $"{appUrl}/confirm-password?token={resetToken}&email={request.Email}";
            var objects = new JObject{{"plink", resetLink } };
            await _mailService.SendMail(request.Email, $"Xác nhận khôi phục mật khẩu", SystemConstant.RESET_PASSWORD_TEMPLATE, objects);
            return new ApiSuccessResult<bool>(true, "If the email is registered, a reset link has been sent.");
        }
        private string GenerateResetToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber).Replace("+", "").Replace("/", "").Replace("=", "");  // URL-safe
        }

        public async Task<ApiResult<bool>> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return new ApiErrorResult<bool>("Invalid email or token.");
            var resetKey = $"reset:{user.Id}";
            var storedToken = await _redisService.GetStringAsync(resetKey);
            if (storedToken != request.Token) return new ApiErrorResult<bool>("Invalid email or token.");
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            await _redisService.RemoveKeyAsync(resetKey);
            return new ApiSuccessResult<bool>(true, "Password has been reset successfully.");
        }
    }
}
