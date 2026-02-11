using Amazon.Runtime.Internal;
using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.Data.EntityTypes;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Auths;
using GaVL.DTO.Settings;
using GaVL.Utilities;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;

namespace GaVL.Application.Auths
{
    public interface IAuthService
    {
        Task<ApiResult<Guid>> Register(RegisterRequest request);
        Task<ApiResult<TokenResponse>> Login(LoginRequest request, string ipAddress);
        Task<ApiResult<TokenResponse>> LoginDashboard(LoginDashboardRequest request);
        Task<ApiResult<TokenResponse>> LoginWithGoogleAsync(GoogleLoginDto dto);
        Task<ApiResult<TokenResponse>> Refresh(RefreshRequest request);
        Task<ApiResult<bool>> Logout(LogoutRequest request);
        Task<ApiResult<bool>> ForgotPassword(ForgotPasswordRequest request);
        Task<ApiResult<bool>> ResetPassword(ResetPasswordRequest request);
        Task<ApiResult<bool>> ChangePassword(Guid userId, string oldPassword, string newPassword);
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
        private readonly IR2Service _r2Service;
        private readonly IHttpClientFactory _httpClientFactory;
        public AuthService(AppDbContext dbContext,
            ILogger<AuthService> logger,
            IR2Service r2Service,
            IConfiguration configuration,
            IMailService mailService,
            ITokenService tokenService,
            IRedisService redisService,
            IOptions<JwtSettings> options,
            ITurnstileService turnstileService,
            IOptions<AppUrlSetting> urlSetting,
            IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _turnstileService = turnstileService;
            _logger = logger;
            _tokenService = tokenService;
            _jwtSettings = options.Value;
            _appUrlSetting = urlSetting.Value;
            _redisService = redisService;
            _mailService = mailService;
            _r2Service = r2Service;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ApiResult<TokenResponse>> LoginDashboard(LoginDashboardRequest request)
        {
            var user = await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return new ApiErrorResult<TokenResponse>("Invalid username or password.");
            }
            if (user.Role == null || user.RoleId > 3)
            {
                return new ApiErrorResult<TokenResponse>("Access denied. Admins or Modderator only.");
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
        public async Task<ApiResult<TokenResponse>> Login(LoginRequest request, string ipAddress)
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
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                SessionId = sessionId
            };
            return new ApiSuccessResult<TokenResponse>(loginResult, "Login successful.");
        }
        public async Task<ApiResult<Guid>> Register(RegisterRequest request)
        {
            var isExistUsername = await isExistUsernameInDatabase(request.Username);
            if (isExistUsername) return new ApiErrorResult<Guid>("Username is already taken.");

            var isValidCaptcha = await _turnstileService.ValidateTokenAsync(request.CaptchaToken);
            if (!isValidCaptcha) return new ApiErrorResult<Guid>("CAPTCHA validation failed.");

            string? avatarUrl = null;
            if (request.Avatar == null)
            {
                avatarUrl = "https://gavl.io.vn/assets/img/avatars/2212.png";
            }
            else
            {
                var fileExtension = Path.GetExtension(request.Avatar.FileName);
                avatarUrl = await _r2Service.UploadFileGetUrl(request.Avatar, $"upload/{request.Username}/{request.Username}{fileExtension}");
                if (string.IsNullOrEmpty(avatarUrl))
                {
                    return new ApiErrorResult<Guid>("Upload ảnh không thành công");
                }
            }
            var newUser = new Data.Entities.User
            {
                Username = request.Username,
                FullName = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                AvatarUrl = avatarUrl,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                LastLoginAt = null,
                AuthProvider = ProviderType.Local
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
            return new ApiSuccessResult<Guid>(newUser.Id, "Đăng ký tài khoản thành công.");
        }
        private async Task<bool> isExistUsernameInDatabase(string username) => await _dbContext.Users.AnyAsync(u => u.Username == username);
        private async Task<bool> isExistEmailInDatabase(string email) => await _dbContext.Users.AnyAsync(u => u.Email == email);
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
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
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
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null) return new ApiSuccessResult<bool>(true, "If the email is registered, a reset link has been sent.");
            var resetToken = GenerateResetToken();
            var resetKey = $"reset:{user.Id}";
            var userEmail = user.Email;
            await _redisService.SetAsync(resetKey, resetToken, TimeSpan.FromMinutes(15));

            var appUrl = _appUrlSetting.Website;
            var resetLink = $"{appUrl}/confirm-password?token={resetToken}&email={userEmail}";
            var objects = new JObject { { "plink", resetLink } };
            await _mailService.SendMail(userEmail, $"Xác nhận khôi phục mật khẩu", SystemConstant.RESET_PASSWORD_TEMPLATE, objects);
            return new ApiSuccessResult<bool>(true, MaskEmail(userEmail));
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
        private string GenerateResetToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber).Replace("+", "").Replace("/", "").Replace("=", "");  // URL-safe
        }
        private string MaskEmail(string email)
        {
            var parts = email.Split('@');
            var localPart = parts[0];
            var visiblePart = localPart.Substring(Math.Max(0, localPart.Length - 3));
            return $"***{visiblePart}@{parts[1]}";
        }

        public async Task<ApiResult<bool>> ChangePassword(Guid userId, string oldPassword, string newPassword)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiErrorResult<bool>("User not found.");
            bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash);
            if (!isOldPasswordValid)
            {
                return new ApiErrorResult<bool>("Old password is incorrect.");
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Password changed successfully.");
        }
        public async Task<ApiResult<TokenResponse>> LoginWithGoogleAsync(GoogleLoginDto dto)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var googleUserInfoUrl = $"https://www.googleapis.com/oauth2/v3/userinfo?access_token={dto.AccessToken}";
            var response = await httpClient.GetAsync(googleUserInfoUrl);
            if (!response.IsSuccessStatusCode) return new ApiErrorResult<TokenResponse>("Login thất bại");
            var googleUser = await response.Content.ReadFromJsonAsync<GoogleUserInfoResponse>();
            if (await isExistEmailInDatabase(googleUser.Email))
            {
                var userExisted = await _dbContext.Users.AsNoTracking()
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == googleUser.Email);
                var sessionIdForUser = Guid.NewGuid().ToString();
                var refreshKeyForUserExisted = $"refresh:{userExisted.Id}:{sessionIdForUser}";

                var accessTokenForUser = await _tokenService.GenerateAccessToken(userExisted);
                var refreshTokenForUser = _tokenService.GenerateRefreshToken();

                await _redisService.SetAsync(refreshKeyForUserExisted, refreshTokenForUser, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays));
                var resultRes = new TokenResponse()
                {
                    AccessToken = accessTokenForUser,
                    RefreshToken = refreshTokenForUser,
                    ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                    SessionId = sessionIdForUser
                };
                return new ApiSuccessResult<TokenResponse>(resultRes);
            }
            var user = new Data.Entities.User
            {
                Email = googleUser.Email,
                FullName = googleUser.Name,
                AvatarUrl = googleUser.Picture,
                AuthProvider = ProviderType.Google,
                GoogleSubjectId = googleUser.Sub,
                Username = googleUser.Email,
                CreatedAt = DateTime.UtcNow,
                RoleId = 4,
                IsActive = true,
                LastLoginAt = null
            };

            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();
            await _dbContext.Entry(user).Reference(u => u.Role).LoadAsync();

            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var sessionId = Guid.NewGuid().ToString();
            var refreshKey = $"refresh:{user.Id}:{sessionId}";
            await _redisService.SetAsync(refreshKey, refreshToken, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays));

            var result = new TokenResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                SessionId = sessionId
            };
            return new ApiSuccessResult<TokenResponse>(result, "Token refreshed successfully.");
        }
        //public async Task<ApiResult<TokenResponse>> LoginWithGoogleAsync(GoogleLoginDto dto)
        //{
        //    try
        //    {
        //        var settings = new GoogleJsonWebSignature.ValidationSettings()
        //        {
        //            Audience = new List<string> { _jwtSettings.GoogleClientID }
        //        };
        //        var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, settings);
        //        if (await isExistEmailInDatabase(payload.Email))
        //        {
        //            var userExisted = await _dbContext.Users.AsNoTracking()
        //                .Include(u => u.Role)
        //                .FirstOrDefaultAsync(u => u.Email == payload.Email);
        //            var sessionIdForUser = Guid.NewGuid().ToString();
        //            var refreshKeyForUserExisted  = $"refresh:{userExisted.Id}:{sessionIdForUser}";

        //            var accessTokenForUser = await _tokenService.GenerateAccessToken(userExisted);
        //            var refreshTokenForUser = _tokenService.GenerateRefreshToken();

        //            await _redisService.SetAsync(refreshKeyForUserExisted, refreshTokenForUser, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays));
        //            var resultRes = new TokenResponse()
        //            {
        //                AccessToken = accessTokenForUser,
        //                RefreshToken = refreshTokenForUser,
        //                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
        //                SessionId = sessionIdForUser
        //            };
        //            return new ApiSuccessResult<TokenResponse>(resultRes);
        //        }
        //        var user = new Data.Entities.User
        //        {
        //            Email = payload.Email,
        //            FullName = payload.Name,
        //            AvatarUrl = payload.Picture,
        //            AuthProvider = ProviderType.Google,
        //            GoogleSubjectId = payload.Subject,
        //            Username = payload.Email,
        //            CreatedAt = DateTime.UtcNow,
        //            RoleId = 4,
        //            IsActive = true,
        //            LastLoginAt = null
        //        };

        //        _dbContext.Add(user);
        //        await _dbContext.SaveChangesAsync();
        //        await _dbContext.Entry(user).Reference(u => u.Role).LoadAsync();

        //        var accessToken = await _tokenService.GenerateAccessToken(user);
        //        var refreshToken = _tokenService.GenerateRefreshToken();

        //        var sessionId = Guid.NewGuid().ToString();
        //        var refreshKey = $"refresh:{user.Id}:{sessionId}";
        //        await _redisService.SetAsync(refreshKey, refreshToken, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays));

        //        var result = new TokenResponse()
        //        {
        //            AccessToken = accessToken,
        //            RefreshToken = refreshToken,
        //            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
        //            SessionId = sessionId
        //        };
        //        return new ApiSuccessResult<TokenResponse>(result, "Token refreshed successfully.");
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Google login failed.");
        //        return new ApiErrorResult<TokenResponse>("Google login failed.");
        //    }
        //}
    }
}
