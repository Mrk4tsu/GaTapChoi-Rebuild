using GaVL.Data;
using GaVL.Data.Entities;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Auths;
using GaVL.DTO.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace GaVL.Application.Auths
{
    public interface IAuthService
    {
        Task<ApiResult<Guid>> Register(RegisterRequest request);
        Task<ApiResult<TokenResponse>> Login(LoginRequest request);
    }
    public class AuthService : IAuthService
    {
        private ILogger<AuthService> _logger;
        private AppDbContext _dbContext;

        private readonly JwtSettings _jwtSettings;
        private readonly ITokenService _tokenService;
        public AuthService(AppDbContext dbContext, ILogger<AuthService> logger, ITokenService tokenService, IOptions<JwtSettings> options)
        {
            _dbContext = dbContext;
            _logger = logger;
            _tokenService = tokenService;
            _jwtSettings = options.Value;
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
            //var userRefreshToken = new UserRefreshToken
            //{
            //    UserId = user.Id,
            //    RefreshToken = refreshToken,
            //    ExpiresAt = DateTime.UtcNow.AddDays(
            //    double.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"])),
            //    CreatedAt = DateTime.UtcNow,
            //    CreatedByIp = ipAddress,
            //    IsRevoked = false
            //};
            var loginResult = new TokenResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
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
                AvatarUrl = request.AvatarUrl,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                LastLoginAt = null
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
            return new ApiSuccessResult<Guid>(newUser.Id, "User registered successfully.");
        }
        private async Task<bool> isExistUsernameInDatabase(string username) => await _dbContext.Users.AnyAsync(u => u.Username == username);
    }
}
