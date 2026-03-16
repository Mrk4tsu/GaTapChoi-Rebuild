using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.Data.Entities;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Mods;
using GaVL.DTO.Settings;
using GaVL.Utilities;
using Markdig;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace GaVL.Application.Catalog.Mods
{
    public class CreateCrackFacade
    {
        private readonly AppDbContext _dbContext;
        private readonly IRedisService _redisService;
        private readonly ModderSetting _modderSetting;
        private readonly MarkdownPipeline _pipeline;
        private readonly DateTime _now;
        public CreateCrackFacade(
            AppDbContext dbContext,
            MarkdownPipeline pipeline,
            IOptions<ModderSetting> modderSetting,
            IRedisService redisService)
        {
            _dbContext = dbContext;
            _redisService = redisService;
            _modderSetting = modderSetting.Value;
            _pipeline = pipeline;
            _now = new TimeHelperBuilder.Builder()
               .SetTimestamp(DateTime.UtcNow)
               .SetTimeZone("SE Asia Standard Time")
               .SetRemoveTick(true).Build();
        }
        public async Task<ApiResult<int>> CreateCrack(ModCombineRequest request, string key, byte type)
        {
            if (!string.Equals(key, _modderSetting.EHVN, StringComparison.Ordinal))
            {
                return new ApiErrorResult<int>("Invalid key provided.");
            }
            var markDownDescription = ConvertMarkdownToHtml(request.Description);
            string prefix = "";
            switch (type)
            {
                case 1:
                    prefix = "[DP] ";
                    break;
                case 2:
                    prefix = "[TLC] ";
                    break;
                case 3:
                    prefix = "[TuanXinh] ";
                    break;
                case 4:
                    prefix = "[CVQ] ";
                    break;
            }
            var newMod = new CrackRequest()
            {
                Name = prefix + request.Name,
                Description = markDownDescription,
                IsPrivate = request.IsPrivate,
                CategoryId = request.CategoryId,
                CrackType = type
            };
            var ehvnId = Guid.Parse(_modderSetting.EHVNID);
            var modResult = await CreateInternalCrack(newMod, ehvnId, type);
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
        private async Task<ApiResult<int>> CreateInternalCrack(ModRequest request, Guid userId, byte type)
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
                SeoAlias = StringHelper.GenerateSeoAlias(request.Name),
                CrackType = type
            };
            await _dbContext.Mods.AddAsync(mod);
            await _dbContext.SaveChangesAsync();
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
                await _dbContext.Urls.AddAsync(item).ConfigureAwait(false);
            }
            await _dbContext.SaveChangesAsync();
            return new ApiSuccessResult<int>(modId);
        }
        private string ConvertMarkdownToHtml(string markdownContent)
        {
            return Markdown.ToHtml(markdownContent, _pipeline);
        }
        private async Task removeCache()
        {
            await _redisService.DeleteKeysByPatternAsync("mods:*");
        }
    }
}
