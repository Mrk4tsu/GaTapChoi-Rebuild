using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.Data.Entities;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Auths;
using GaVL.DTO.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace GaVL.Application.Auths
{
    public interface IAuthService
    {
        Task<ApiResult<Guid>> Register(RegisterRequest request);
        Task<ApiResult<TokenResponse>> Login(LoginRequest request);
        Task<ApiResult<TokenResponse>> Refresh(RefreshRequest request);
    }
    public class AuthService : IAuthService
    {
        private ILogger<AuthService> _logger;
        private AppDbContext _dbContext;

        private readonly JwtSettings _jwtSettings;
        private readonly ITokenService _tokenService;
        private readonly IRedisService _redisService;
        public AuthService(AppDbContext dbContext,
            ILogger<AuthService> logger,
            ITokenService tokenService,
            IRedisService redisService,
            IOptions<JwtSettings> options)
        {
            _dbContext = dbContext;
            _logger = logger;
            _tokenService = tokenService;
            _jwtSettings = options.Value;
            _redisService = redisService;
        }

        public async Task<ApiResult<TokenResponse>> Login(LoginRequest request)
        {
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
    }
}
