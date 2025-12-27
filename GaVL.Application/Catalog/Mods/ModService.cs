using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Mods;
using GaVL.DTO.Paging;
using GaVL.DTO.Settings;
using GaVL.Utilities;
using Markdig;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GaVL.Application.Catalog.Mods
{
    public interface IModService
    {
        Task<ApiResult<PagedResult<ModDTO>>> GetMods(ModQueryRequest request);
        Task<ApiResult<ModDetailDTO>> GetModById(int modId);
        Task<ApiResult<SeoModDTO>> GetSeoModById(int modId);
        Task<ApiResult<int>> CreateMod(ModCombineRequest request, Guid userId);
        Task<ApiResult<int>> CreateCrack(ModCombineRequest request, string key, byte type);
        Task<ApiResult<int>> UpdateMod(int modId, ModUpdateCombineRequest request, Guid userId);
        Task<ApiResult<int>> UpdateStatus(int modId, Guid userId);
        Task<ApiResult<bool>> IncreaseView(int modId);
        Task<ApiResult<bool>> DeleteMod(int modId, Guid userId);
    }
    public class ModService : IModService
    {
        private readonly AppDbContext _dbContext;
        private readonly IRedisService _redisService;
        private readonly MarkdownPipeline _pipeline;
        private DateTime _now;
        private readonly ModderSetting _modderSetting;
        private const int CacheExpiryValue = 10;
        public ModService(AppDbContext context, IOptions<ModderSetting> options, MarkdownPipeline pipeline, IRedisService redisService)
        {
            _dbContext = context;
            _modderSetting = options.Value;
            _redisService = redisService;
            _pipeline = pipeline;
            _now = new TimeHelperBuilder.Builder()
               .SetTimestamp(DateTime.UtcNow)
               .SetTimeZone("SE Asia Standard Time")
               .SetRemoveTick(true).Build();
        }

        public async Task<ApiResult<PagedResult<ModDTO>>> GetMods(ModQueryRequest request)
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
            .OrderByDescending(x => x.UpdatedAt)
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
                CategoryId = m.Category.Id,
                CrackType = m.CrackType,
                SeoAlias = m.SeoAlias,
                IsPrivate = m.IsPrivate,
                Thumbnail = m.Thumbnail
            }).ToList();

            var result = new PagedResult<ModDTO>
            {
                Items = modDtos,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalRecords = totalRecords
            };
            await _redisService.SetValue(cacheKey, result, TimeSpan.FromMinutes(CacheExpiryValue));
            return new ApiSuccessResult<PagedResult<ModDTO>>(result);
        }

        public async Task<ApiResult<int>> CreateMod(ModCombineRequest request, Guid userId)
        {
            var createMod = new CreateModFacade(_dbContext, _redisService, _now);
            var result = await createMod.CreateMod(request, userId);
            return result;
        }
        public async Task<ApiResult<int>> CreateCrack(ModCombineRequest request, string key, byte type)
        {
            var createCrack = new CreateCrackFacade(
                 _dbContext,
                 _pipeline,
                 Options.Create(_modderSetting),
                 _redisService
             );
            var result = await createCrack.CreateCrack(request, key, type);
            return result;
        }
        public async Task<ApiResult<ModDetailDTO>> GetModById(int modId)
        {
            var cacheKey = $"mod:detail:{modId}";
            var cachedMod = await _redisService.GetValue<ModDetailDTO>(cacheKey);
            if (cachedMod != null) return new ApiSuccessResult<ModDetailDTO>(cachedMod);
            var mod = await _dbContext.Mods.AsNoTracking()
               .Where(m => m.Id == modId && !m.IsDeleted)
               .Select(x => new ModDetailDTO
               {
                   Id = x.Id,
                   AuthorId = x.UserId,
                   Name = x.Name,
                   Description = x.Description,
                   CreatedAt = x.CreatedAt,
                   UpdatedAt = x.UpdatedAt,
                   Username = x.User.Username,
                   CrackType = x.CrackType,
                   SeoAlias = x.SeoAlias,
                   IsPrivate = x.IsPrivate,
                   ViewCount = x.ViewCount,
                   Thumbnail = x.Thumbnail,
                   Urls = x.Urls
                        .Where(u => !u.IsDeleted)
                        .Select(u => new UrlModDTO
                        {
                            Id = u.Id,
                            Url = u.UrlString
                        }).ToList()
               }).FirstOrDefaultAsync();
            if (mod != null)
            {
                await _redisService.SetValue(cacheKey, mod, TimeSpan.FromHours(1));
                return new ApiSuccessResult<ModDetailDTO>(mod);
            }
            return new ApiErrorResult<ModDetailDTO>("Mod not found");
        }

        public async Task<ApiResult<SeoModDTO>> GetSeoModById(int modId)
        {
            var cacheKey = $"seoMod:{modId}";
            var cachedSeoMod = await _redisService.GetValue<SeoModDTO>(cacheKey);
            if (cachedSeoMod != null)
            {
                return new ApiSuccessResult<SeoModDTO>(cachedSeoMod);
            }
            var mod = await _dbContext.Mods.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == modId && !m.IsDeleted);
            if (mod == null)
            {
                return new ApiErrorResult<SeoModDTO>("Mod not found");
            }
            var seoModDto = new SeoModDTO
            {
                Id = mod.Id,
                Name = mod.Name,
                SeoAlias = mod.SeoAlias,
                Thumbnail = mod.Thumbnail
            };
            await _redisService.SetValue(cacheKey, seoModDto);
            return new ApiSuccessResult<SeoModDTO>(seoModDto);
        }

        public async Task<ApiResult<int>> UpdateMod(int modId, ModUpdateCombineRequest request, Guid userId)
        {
            var updateMod = new UpdateModFacade(_dbContext, _redisService, _now);
            var result = await updateMod.UpdateMod(modId, request, userId);
            return result;
        }

        public async Task<ApiResult<int>> UpdateStatus(int modId, Guid userId)
        {
            var mod = await _dbContext.Mods
                .FirstOrDefaultAsync(m => m.Id == modId && m.UserId == userId && !m.IsDeleted);
            if (mod == null)
            {
                return new ApiErrorResult<int>("Mod not found or has been deleted");
            }
            mod.IsLocked = !mod.IsLocked;
            mod.UpdatedAt = _now;
            _dbContext.Mods.Update(mod);
            await _dbContext.SaveChangesAsync();
            return new ApiSuccessResult<int>(mod.Id);
        }

        public async Task<ApiResult<bool>> IncreaseView(int modId)
        {
            var mod = await _dbContext.Mods.FirstOrDefaultAsync(x => x.Id == modId && !x.IsDeleted);
            if (mod == null) return new ApiErrorResult<bool>("Mod not found or has been deleted");
            mod.ViewCount += 1;
            _dbContext.Mods.Update(mod);
            await _dbContext.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true);
        }

        public async Task<ApiResult<bool>> DeleteMod(int modId, Guid userId)
        {
            var mod = await _dbContext.Mods
                .Include(x => x.User)
                .FirstOrDefaultAsync(m => m.Id == modId && !m.IsDeleted && m.UserId == userId);
            if (mod == null)
                return new ApiErrorResult<bool>("Mod not found or has been deleted");
            mod.IsDeleted = true;
            _dbContext.Mods.Update(mod);
            await _dbContext.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true);
        }
    }
}