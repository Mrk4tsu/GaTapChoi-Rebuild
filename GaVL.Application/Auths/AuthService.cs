using GaVL.Data;
using GaVL.Data.Entities;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Auths;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GaVL.Application.Auths
{
    public interface IAuthService
    {
        Task<ApiResult<Guid>> Register(RegisterRequest request);
    }
    public class AuthService : IAuthService
    {
        private ILogger<AuthService> _logger;
        private AppDbContext _dbContext;
        public AuthService(AppDbContext dbContext, ILogger<AuthService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
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
