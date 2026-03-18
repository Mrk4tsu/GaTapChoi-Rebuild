using GaVL.Data;
using GaVL.Data.Entities;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Notification;
using Microsoft.EntityFrameworkCore;

namespace GaVL.Application.Systems
{
    public interface INotifyService
    {
        Task<ApiResult<List<NotifyViewModel>>> GetListNotify();
        Task<ApiResult<NotifyViewModel>> GetById(int id);
        Task<ApiResult<int>> Create(NotifyRequest request, Guid userId, CancellationToken ct = default);
        Task<ApiResult<bool>> Update(int id, NotifyRequest request, Guid userId, CancellationToken ct = default);
        Task<ApiResult<bool>> Delete(int id, Guid userId, CancellationToken ct = default);
    }
    public class NotifyService : INotifyService
    {
        private const string PREFIX = "notify";
        private const int STAFF_MAX_ROLE_ID = 3;
        private readonly AppDbContext _context;
        private readonly IRedisService _redis;
        private readonly IR2Service _r2Service;
        public NotifyService(AppDbContext context, IRedisService redis, IR2Service r2Service)
        {
            _context = context;
            _redis = redis;
            _r2Service = r2Service;
        }
        public async Task<ApiResult<int>> Create(NotifyRequest request, Guid userId, CancellationToken ct = default)
        {
            var canManage = await CanManageNotifications(userId, ct);
            if (!canManage)
                return new ApiErrorResult<int>("Access denied. Admins or Modderator only.");

            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Message))
                return new ApiErrorResult<int>("Không được để trống Title/Message");

            if (request.Thumbnail is null || request.Thumbnail.Length == 0)
                return new ApiErrorResult<int>("Thumbnail là bắt buộc");

            var safeFileName = Path.GetFileName(request.Thumbnail.FileName);
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var key = $"{PREFIX}/{userId}/{timestamp}_{safeFileName}";
            var thumbnailUrl = await _r2Service.UploadFileGetUrl(request.Thumbnail, key);
            if (string.IsNullOrEmpty(thumbnailUrl))
                return new ApiErrorResult<int>("Upload thumbnail thất bại");

            var now = DateTime.UtcNow;
            var newNotify = new Notify()
            {
                UserId = userId,
                Title = request.Title,
                Content = request.Message,
                Url = request.Url ?? string.Empty,
                Thumbnail = thumbnailUrl,
                CreatedAt = now,
                LastUpdatedAt = now,
                IsDeleted = false
            };
            _context.Notifies.Add(newNotify);
            await _context.SaveChangesAsync(ct);
            await InvalidateCache();
            return new ApiSuccessResult<int>(newNotify.Id);
        }

        public async Task<ApiResult<bool>> Delete(int id, Guid userId, CancellationToken ct = default)
        {
            var canManage = await CanManageNotifications(userId, ct);
            if (!canManage)
                return new ApiErrorResult<bool>("Access denied. Admins or Modderator only.");

            var notify = await _context.Notifies.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
            if (notify is null)
                return new ApiErrorResult<bool>("Notification không tồn tại hoặc đã bị xóa");

            notify.IsDeleted = true;
            notify.LastUpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            await InvalidateCache();
            return new ApiSuccessResult<bool>(true);
        }

        public async Task<ApiResult<List<NotifyViewModel>>> GetListNotify()
        {
            var key = $"{PREFIX}:list";
            if (await _redis.KeyExist(key))
            {
                var cachedData = await _redis.GetValue<List<NotifyViewModel>>(key);
                if (cachedData != null)
                {
                    return new ApiSuccessResult<List<NotifyViewModel>>(cachedData);
                }
            }
            var notify = await _context.Notifies.AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new NotifyViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Message = x.Content,
                    CreateDate = x.CreatedAt,
                    LastUpdatedAt = x.LastUpdatedAt,
                    Thumbnail = x.Thumbnail,
                    Url = x.Url
                }).ToListAsync();
            await _redis.SetValue(key, notify, TimeSpan.FromHours(1));
            return new ApiSuccessResult<List<NotifyViewModel>>(notify);
        }

        public async Task<ApiResult<NotifyViewModel>> GetById(int id)
        {
            var key = $"{PREFIX}:detail:{id}";
            var cached = await _redis.GetValue<NotifyViewModel>(key);
            if (cached != null) return new ApiSuccessResult<NotifyViewModel>(cached);

            var notify = await _context.Notifies.AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new NotifyViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Message = x.Content,
                    CreateDate = x.CreatedAt,
                    LastUpdatedAt = x.LastUpdatedAt,
                    Thumbnail = x.Thumbnail,
                    Url = x.Url
                })
                .FirstOrDefaultAsync();

            if (notify is null)
                return new ApiErrorResult<NotifyViewModel>("Notification không tồn tại hoặc đã bị xóa");

            await _redis.SetValue(key, notify, TimeSpan.FromHours(1));
            return new ApiSuccessResult<NotifyViewModel>(notify);
        }

        public async Task<ApiResult<bool>> Update(int id, NotifyRequest request, Guid userId, CancellationToken ct = default)
        {
            var canManage = await CanManageNotifications(userId, ct);
            if (!canManage)
                return new ApiErrorResult<bool>("Access denied. Admins or Modderator only.");

            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Message))
                return new ApiErrorResult<bool>("Không được để trống Title/Message");

            var notify = await _context.Notifies.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
            if (notify is null)
                return new ApiErrorResult<bool>("Notification không tồn tại hoặc đã bị xóa");

            notify.Title = request.Title;
            notify.Content = request.Message;
            notify.Url = request.Url ?? string.Empty;
            notify.LastUpdatedAt = DateTime.UtcNow;

            if (request.Thumbnail is not null && request.Thumbnail.Length > 0)
            {
                var safeFileName = Path.GetFileName(request.Thumbnail.FileName);
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var key = $"{PREFIX}/{userId}/{timestamp}_{safeFileName}";
                var newThumbnailUrl = await _r2Service.UploadFileGetUrl(request.Thumbnail, key);
                if (string.IsNullOrEmpty(newThumbnailUrl))
                    return new ApiErrorResult<bool>("Upload thumbnail thất bại");

                // Best-effort delete old
                if (!string.IsNullOrEmpty(notify.Thumbnail))
                {
                    try { await _r2Service.DeleteOldImage(notify.Thumbnail); } catch { }
                }
                notify.Thumbnail = newThumbnailUrl;
            }

            await _context.SaveChangesAsync(ct);
            await InvalidateCache();
            return new ApiSuccessResult<bool>(true);
        }
        private async Task<bool> CanManageNotifications(Guid userId, CancellationToken ct)
        {
            var roleId = await _context.Users
                .AsNoTracking()
                .Where(x => x.Id == userId)
                .Select(x => x.RoleId)
                .FirstOrDefaultAsync(ct);

            // If user not found => roleId = 0; treat as denied
            if (roleId <= 0) return false;
            return roleId <= STAFF_MAX_ROLE_ID;
        }
        private async Task InvalidateCache()
        {
            var key = $"{PREFIX}:*";
            await _redis.DeleteKeysByPatternAsync(key);
        }
    }
}
