using Amazon.Runtime.Internal.Util;
using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.DTO.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace GaVL.Application.Catalog.Common
{
    public class SearchService
    {
        private readonly AppDbContext _context;
        private readonly IRedisService _redis;
        public SearchService(AppDbContext context, IRedisService redis)
        {
            _context = context;
            _redis = redis;
        }
        public async Task<List<SearchSuggestionDto>> GetSuggestionsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword) || keyword.Length <= 1)
                return new List<SearchSuggestionDto>();
            var normalizedKeyword = keyword.Trim().ToLower();
            var cacheKey = $"search_suggest_{normalizedKeyword}";

            var cachedData = await _redis.GetValue<List<SearchSuggestionDto>>(cacheKey);
            if (cachedData != null)
                return cachedData;
            var searchPattern = $"%{normalizedKeyword}%";
            var modsQuery = _context.Mods
                .AsNoTracking() // Tối ưu bộ nhớ, không theo dõi Entity
                .Where(m => !m.IsDeleted && (EF.Functions.ILike(m.Name, searchPattern) || EF.Functions.ILike(m.Description, searchPattern)))
                .Select(m => new SearchSuggestionDto
                {
                    Id = m.Id,
                    Title = m.Name,
                    Type = "Mod",
                    SeoAlias = m.SeoAlias,
                    Thumbnail = m.Thumbnail
                })
                .Take(5);
            var postsQuery = _context.Posts
                .AsNoTracking()
                .Where(p => !p.IsDeleted && (EF.Functions.ILike(p.Title, searchPattern) || EF.Functions.ILike(p.Sumary, searchPattern)))
                .Select(p => new SearchSuggestionDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Type = "Post",
                    SeoAlias = p.SeoAlias,
                    Thumbnail = p.MainImage
                })
                .Take(5);
            var results = new List<SearchSuggestionDto>();
            results.AddRange(await modsQuery.ToListAsync());
            results.AddRange(await postsQuery.ToListAsync());

            await _redis.SetValue(cacheKey, results, TimeSpan.FromMinutes(10));

            return results;
        }
    }
}
