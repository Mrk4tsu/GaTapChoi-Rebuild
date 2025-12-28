using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.Data.Entities;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Mods;
using GaVL.Utilities;

namespace GaVL.Application.Catalog.Mods
{
    public class CreateModFacade(AppDbContext _db, IRedisService _redis, DateTime _now)
    {
        public async Task<ApiResult<int>> CreateMod(ModCombineRequest request, Guid userId)
        {
            var newMod = new ModRequest()
            {
                Name = request.Name,
                Description = request.Description,
                IsPrivate = request.IsPrivate,
                CategoryId = request.CategoryId,
            };
            var modResult = await CreateInternalMod(newMod, userId);
            if (!modResult.Success) return modResult;
            var newUrl = new UrlRequest()
            {
                Url = request.NewUrls
            };
            var urlResult = await CreateInternalUrl(newUrl, modResult.Data);
            if (!urlResult.Success) return urlResult;
            _ = Task.Run(async () =>
            {
                await removeCache();
            });
            return new ApiSuccessResult<int>(modResult.Data);
        }
        private async Task<ApiResult<int>> CreateInternalMod(ModRequest request, Guid userId)
        {
            var mod = new Mod()
            {
                Name = request.Name,
                Description = request.Description,
                UserId = userId,
                CreatedAt = _now,
                UpdatedAt = _now,
                CategoryId = request.CategoryId,
                IsPrivate = request.IsPrivate,
                IsDeleted = false,
                Thumbnail = $"https://storage.gavl.io.vn/temp/{request.CategoryId - 1}.png",
                SeoAlias = StringHelper.GenerateSeoAlias(request.Name)
            };
            _db.Mods.Add(mod);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(mod.Id);
        }
        private async Task<ApiResult<int>> CreateInternalUrl(UrlRequest request, int modId)
        {
            if (request.Url == null || !request.Url.Any())
            {
                return new ApiSuccessResult<int>(modId);
            }

            foreach (var u in request.Url)
            {
                var item = new Url()
                {
                    ModId = modId,
                    UrlString = u,
                    CreatedAt = _now,
                    IsDeleted = false,
                    UpdatedAt = _now
                };
                await _db.Urls.AddAsync(item).ConfigureAwait(false);
            }
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(modId);
        }
        private async Task removeCache()
        {
            await _redis.DeleteKeysByPatternAsync("mods:*");
        }
    }
}
