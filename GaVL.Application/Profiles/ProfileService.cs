using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Profiles;

namespace GaVL.Application.Profiles
{
    public interface IProfileService
    {
        Task<ApiResult<string>> UploadAvatarAsync(Guid userId, AvatarRequest request);
    }
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _db;
        private readonly IR2Service _r2Service;
        private string _avatarPref = "upload";
        public ProfileService(AppDbContext db, IR2Service r2Service)
        {
            _db = db;
            _r2Service = r2Service;
        }
        public async Task<ApiResult<string>> UploadAvatarAsync(Guid userId, AvatarRequest request)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiErrorResult<string>("User not found.");
            }
            var fileExtension = Path.GetExtension(request.AvatarImage.FileName);
            var avatarUrl = await _r2Service.UploadFileGetUrl(request.AvatarImage, $"{_avatarPref}/{user.Username}/{user.Username}{fileExtension}");
            if (string.IsNullOrEmpty(avatarUrl))
            {
                return new ApiErrorResult<string>("Upload fail");
            }
            user.AvatarUrl = avatarUrl;
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<string>(avatarUrl);
        }
    }
}
