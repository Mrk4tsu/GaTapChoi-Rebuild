using GaVL.Data;
using GaVL.Data.Entities;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Notification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaVL.Application.Systems
{
    public interface INotifyService
    {
        Task<ApiResult<List<NotifyViewModel>>> GetListNotify();
        Task<ApiResult<int>> Create(NotifyRequest request);
        Task<ApiResult<bool>> Update();
        Task<ApiResult<bool>> Delete();
    }
    public class NotifyService : INotifyService
    {
        private const string PREFIX = "notify";
        private readonly AppDbContext _context;
        private readonly IRedisService _redis;
        public NotifyService(AppDbContext context, IRedisService redis)
        {
            _context = context;
            _redis = redis;
        }
        public async Task<ApiResult<int>> Create(NotifyRequest request)
        {
            var newNotify = new Notify()
            {
                Title = request.Title,
                Content = request.Message,
                Url = request.Url,
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifies.Add(newNotify);
            await _context.SaveChangesAsync();
            await InvalidateCache();
            return new ApiSuccessResult<int>(newNotify.Id);
        }

        public Task<ApiResult<bool>> Delete()
        {
            throw new NotImplementedException();
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
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new NotifyViewModel()
                {
                    Title = x.Title,
                    Message = x.Content,
                    CreateDate = x.CreatedAt,
                    Url = x.Url
                }).ToListAsync();
            await _redis.SetValue(key, notify, TimeSpan.FromHours(1));
            return new ApiSuccessResult<List<NotifyViewModel>>(notify);
        }

        public Task<ApiResult<bool>> Update()
        {
            throw new NotImplementedException();
        }
        private async Task InvalidateCache()
        {
            var key = $"{PREFIX}:*";
            await _redis.DeleteKeysByPatternAsync(key);
        }
    }
}
