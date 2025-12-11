using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Mods;
using GaVL.DTO.Paging;
using Microsoft.EntityFrameworkCore;

namespace GaVL.Application.Catalog.Mods
{
    public interface IModService
    {
        Task<ApiResult<PagedResult<ModDTO>>> GetModsAsync(ModQueryRequest request);
    }
    public class ModService : IModService
    {
        private readonly AppDbContext _dbContext;
        private readonly IRedisService _redisService;
        private const int CacheExpiryMinutes = 10;
        public ModService(AppDbContext context, IRedisService redisService)
        {
            _dbContext = context;
            _redisService = redisService;
        }

        public async Task<ApiResult<PagedResult<ModDTO>>> GetModsAsync(ModQueryRequest request)
        {
            var cacheKey = $"mods:p{request.PageIndex}:s{request.PageSize}:u{request.Username?.ToLower() ?? "all"}:c{request.CategoryId ?? 0}";
            var cachedResult = await _redisService.GetValue<PagedResult<ModDTO>>(cacheKey);
            if (cachedResult != null)
            {
                var pageResult = new PagedResult<ModDTO>
                {
                    Items = cachedResult.Items,
                    TotalRecords = cachedResult.TotalRecords,
                    PageIndex = cachedResult.PageIndex,
                    PageSize = cachedResult.PageSize
                };
                return new ApiSuccessResult<PagedResult<ModDTO>>(pageResult);
            }
            var query = _dbContext.Mods
            .Include(m => m.User)
            .Include(m => m.Category)
            .Where(m => !m.IsDeleted);
            if (!string.IsNullOrWhiteSpace(request.Username))
                query = query.Where(m => m.User.Username.ToLower().Contains(request.Username.ToLower()));
            if (request.CategoryId.HasValue)
                query = query.Where(m => m.CategoryId == request.CategoryId.Value);

            var totalRecords = await query.CountAsync();
            var mods = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

            var modDtos = mods.Select(m => new ModDTO
            {
                Id = m.Id,
                Name = m.Name,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt,
                Username = m.User.Username,
                CategoryName = m.Category.Name,
                CrackType = m.CrackType,
                SeoAlias = m.SeoAlias,
                IsPrivate = m.IsPrivate
            }).ToList();

            var result = new PagedResult<ModDTO>
            {
                Items = modDtos,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalRecords = totalRecords
            };
            await _redisService.SetValue(cacheKey, result, TimeSpan.FromMinutes(CacheExpiryMinutes));
            return new ApiSuccessResult<PagedResult<ModDTO>>(result);
        }
    }
}