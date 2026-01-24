using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Profiles;
using Microsoft.EntityFrameworkCore;

namespace GaVL.Application.Profiles
{
    public interface IProfileService
    {
        Task<ApiResult<UserViewModel>> GetUserByUsername(string username);
        Task<ApiResult<string>> UploadAvatar(Guid userId, AvatarRequest request);
        Task<ApiResult<string>> UpdateFullname(Guid userId, string newName);
    }
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _db;
        private readonly IR2Service _r2Service;
        private readonly IRedisService _redis;
        private string _avatarPref = "upload";
        private string _profilePref = "profile";
        public ProfileService(AppDbContext db, IRedisService redis, IR2Service r2Service)
        {
            _db = db;
            _redis = redis;
            _r2Service = r2Service;
        }

        public async Task<ApiResult<UserViewModel>> GetUserByUsername(string username)
        {
            var profileKey = $"{_profilePref}:username:{username.ToLower()}";
            var cachedProfile = await _redis.GetValue<UserViewModel>(profileKey);
            if (cachedProfile != null)
            {
                return new ApiSuccessResult<UserViewModel>(cachedProfile);
            }
            var user = await _db.Users.AsNoTracking()
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return new ApiErrorResult<UserViewModel>("User not found.");
            }
            var userResult = new UserViewModel
            {
                Id = user.Id,
                Fullname = user.FullName,
                Username = user.Username,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                CreateAt = user.CreatedAt
            };
            await _redis.SetValue(profileKey, userResult, TimeSpan.FromHours(1));
            return new ApiSuccessResult<UserViewModel>(userResult);
        }

        public async Task<ApiResult<string>> UpdateFullname(Guid userId, string newName)
        {
            var rateLimitKey = $"{_profilePref}:fullname:ratelimit:{userId}";
            var lastUpdateTimestamp = await _redis.GetValue<long?>(rateLimitKey);

            if (lastUpdateTimestamp.HasValue)
            {
                var lastUpdateTime = DateTimeOffset.FromUnixTimeSeconds(lastUpdateTimestamp.Value);
                var nextAllowedTime = lastUpdateTime.AddDays(3);

                if (DateTimeOffset.UtcNow < nextAllowedTime)
                {
                    var remainingTime = nextAllowedTime - DateTimeOffset.UtcNow;
                    var daysRemaining = (int)Math.Ceiling(remainingTime.TotalDays);
                    return new ApiErrorResult<string>($"You can only change your name once every 3 days. Please wait {daysRemaining} more day(s).");
                }
            }

            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiErrorResult<string>("User not found.");
            }

            user.FullName = newName;
            await _db.SaveChangesAsync();
            _ = removeCache();
            var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            await _redis.SetValue(rateLimitKey, currentTimestamp, TimeSpan.FromDays(3));
            
            return new ApiSuccessResult<string>(newName);
        }

        public async Task<ApiResult<string>> UploadAvatar(Guid userId, AvatarRequest request)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiErrorResult<string>("User not found.");
            }

            var fileExtension = Path.GetExtension(request.AvatarImage.FileName);
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var newKey = $"{_avatarPref}/{user.Username}/{user.Username}_{timestamp}{fileExtension}";

            var avatarUrl = await _r2Service.UploadFileGetUrl(request.AvatarImage, newKey);
            if (string.IsNullOrEmpty(avatarUrl))
            {
                return new ApiErrorResult<string>("Upload fail");
            }
            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                try
                {
                    await _r2Service.DeleteOldImage(user.AvatarUrl);
                }
                catch { /* Ignore delete errors */ }
            }

            user.AvatarUrl = avatarUrl;
            await _db.SaveChangesAsync();
            _ = removeCache();
            return new ApiSuccessResult<string>(avatarUrl);
        }
        private async Task removeCache()
        {
            await _redis.DeleteKeysByPatternAsync($"{_profilePref}:*");
        }
    }
}
