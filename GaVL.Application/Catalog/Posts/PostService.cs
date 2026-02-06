using Amazon.Runtime.Internal;
using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.Data.Entities;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Paging;
using GaVL.DTO.Posts;
using GaVL.Utilities;
using Mailjet.Client.Resources;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GaVL.Application.Catalog.Posts
{
    public interface IPostService
    {
        Task<ApiResult<int>> CreatePost(PostRequest request, Guid userId, CancellationToken ct = default);
        Task<ApiResult<int>> CreatePostAdvanced(PostRequest request, Guid userId, CancellationToken ct = default);
        Task<ApiResult<PagedResult<PostDTO>>> GetPosts(PagingRequest request);
        Task<ApiResult<PostDetailDTO>> GetPostById(int id);
        Task<ApiResult<SeoPostDTO>> GetSeoPostById(int postId);
        Task<ApiResult<PagedResult<PostDTO>>> GetPostByUsername(string username, PagingRequest request);
    }
    public class PostService : IPostService
    {
        private readonly AppDbContext _db;
        private readonly IRedisService _redisService;
        private readonly IR2Service _r2Service;
        private readonly string KeyPrefix = "posts";
        private DateTime _now;
        public PostService(AppDbContext db, IR2Service r2Service, IRedisService redisService)
        {
            _db = db;
            _redisService = redisService;
            _r2Service = r2Service;
            _now = new TimeHelperBuilder.Builder()
              .SetTimestamp(DateTime.UtcNow)
              .SetTimeZone("SE Asia Standard Time")
              .SetRemoveTick(true).Build();
        }
        public async Task<ApiResult<PostDetailDTO>> GetPostById(int id)
        {
            var cacheKey = $"{KeyPrefix}:detail:{id}";
            var cachedData = await _redisService.GetValue<PostDetailDTO>(cacheKey);
            if (cachedData != null)
            {
                return new ApiSuccessResult<PostDetailDTO>(cachedData);
            }
            var post = await _db.Posts.AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Category)
                .Include(x => x.PostTags)
                    .ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (post == null)
            {
                return new ApiErrorResult<PostDetailDTO>("Bài viết không tồn tại");
            }
            var postDTO = new PostDetailDTO
            {
                Id = post.Id,
                AuthorId = post.UserId,
                AuthorName = post.User.FullName,
                Username = post.User.Username,
                Avatar = post.User.AvatarUrl,
                SeoAlias = post.SeoAlias,
                Thumbnail = post.MainImage,
                Title = post.Title,
                Sumary = post.Sumary,
                Content = post.Description,
                CreatedAt = post.CreateAt,
                UpdatedAt = post.UpdateAt,
                CategoryId = post.CategoryId,
                CategoryName = post.Category?.Name,
                Tags = new()
            };
            await SaveCache(cacheKey, postDTO);
            return new ApiSuccessResult<PostDetailDTO>(postDTO);
        }

        public async Task<ApiResult<SeoPostDTO>> GetSeoPostById(int postId)
        {
            var key = $"{KeyPrefix}:seo:{postId}";
            var cachedData = await _redisService.GetValue<SeoPostDTO>(key);
            if (cachedData != null)
            {
                return new ApiSuccessResult<SeoPostDTO>(cachedData);
            }
            var post = await _db.Posts.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == postId && !x.IsDeleted);
            if (post == null)
            {
                return new ApiErrorResult<SeoPostDTO>("Bài viết không tồn tại");
            }
            var seoPostDTO = new SeoPostDTO
            {
                Id = post.Id,
                Name = post.Title,
                SeoAlias = post.SeoAlias,
                Thumbnail = post.MainImage
            };
            await SaveCache(key, seoPostDTO);
            return new ApiSuccessResult<SeoPostDTO>(seoPostDTO);
        }
        public async Task<ApiResult<PagedResult<PostDTO>>> GetPosts(PagingRequest request)
        {            
            var query = _db.Posts.AsNoTracking().Include(x => x.User).Where(x => x.IsDeleted == false)
                .AsQueryable();

            var totalRecord = await query.CountAsync();
            var posts = query.OrderByDescending(x => x.CreateAt)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);
            var postsDTO = await posts.Select(x => new PostDTO
            {
                Id = x.Id,
                AuthorId = x.UserId,
                AuthorName = x.User.FullName,
                Username = x.User.Username,
                SeoAlias = x.SeoAlias,
                Thumbnail = x.MainImage,
                Title = x.Title,
                Sumary = x.Sumary,
                CreatedAt = x.CreateAt,
                UpdatedAt = x.UpdateAt
            }).ToListAsync();

            var result = new PagedResult<PostDTO>
            {
                Items = postsDTO,
                TotalRecords = totalRecord,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
            return new ApiSuccessResult<PagedResult<PostDTO>>(result);
        }
        public async Task<ApiResult<PagedResult<PostDTO>>> GetPostByUsername(string username, PagingRequest request)
        {
            var key = $"{KeyPrefix}:{username}:page:{request.PageIndex}-size:{request.PageSize}";
            var query = _db.Posts.AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.User.Username == username && !x.IsDeleted)
                .AsQueryable();
            var totalRecord = await query.CountAsync();
            var posts = query.OrderByDescending(x => x.CreateAt)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);
            var postsDTO = await posts.Select(x => new PostDTO
            {
                Id = x.Id,
                AuthorId = x.UserId,
                AuthorName = x.User.Username,
                SeoAlias = x.SeoAlias,
                Thumbnail = x.MainImage,
                Title = x.Title,
                Sumary = x.Sumary,
                CreatedAt = x.CreateAt,
                UpdatedAt = x.UpdateAt
            }).ToListAsync();
            var result = new PagedResult<PostDTO>
            {
                Items = postsDTO,
                TotalRecords = totalRecord,
                PageIndex = 1,
                PageSize = totalRecord
            };
            return new ApiSuccessResult<PagedResult<PostDTO>>(result);
        }
        public async Task<ApiResult<int>> CreatePost(PostRequest request, Guid userId, CancellationToken ct = default)
        {
            var code = StringHelper.GenerateRandomCode();
            if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
            {
                return new ApiErrorResult<int>("Không đc để trống input");
            }
            string keyFile = $"{KeyPrefix}/{code}";

            string mainImageCleanFileName = Path.GetFileName(request.Thumbnail.FileName);
            var mainImgKey = $"{keyFile}/{mainImageCleanFileName}";
            var mainImageUrl = await _r2Service.UploadFileGetUrl(request.Thumbnail, mainImgKey);
            var newPost = new Post
            {
                UserId = userId,
                Title = request.Title,
                SeoAlias = StringHelper.GenerateSeoAlias(request.Title),
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Description = request.Description,
                MainImage = mainImageUrl,
                IsDeleted = false,
                Code = code,
            };

            _db.Posts.Add(newPost);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(newPost.Id);
        }

        public async Task<ApiResult<int>> CreatePostAdvanced(PostRequest request, Guid userId, CancellationToken ct = default)
        {
            try
            {
                // 1. Validate Category
                var category = await _db.PostCategories.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.CategoryId && !x.IsDeleted, ct);

                if (category is null)
                    return new ApiErrorResult<int>("Danh mục không tồn tại hoặc đã bị xóa");

                // 2. Validate User
                var user = await _db.Users
                   .AsNoTracking()
                   .FirstOrDefaultAsync(x => x.RoleId <= 3, ct);
                if (user is null)
                    return new ApiErrorResult<int>("Người dùng không tồn tại hoặc đã bị vô hiệu hóa");

                // 3. Generate Code
                var code = StringHelper.GenerateRandomCode();
                // 4. Generate SeoAlias
                var seoAlias = StringHelper.GenerateSeoAlias(request.Title);

                // 5. Upload Thumbnail
                string mainImageUrl;
                if (request.Thumbnail is not null && request.Thumbnail.Length > 0)
                {
                    string keyFile = $"{KeyPrefix}/{code}";

                    string mainImageCleanFileName = SanitizeFileName(request.Thumbnail.FileName);
                    var mainImgKey = $"{keyFile}/{mainImageCleanFileName}";
                    mainImageUrl = await _r2Service.UploadFileGetUrl(request.Thumbnail, mainImgKey);
                }
                else
                {
                    return new ApiErrorResult<int>("Vui lòng cung cấp thumbnail");
                }
                var post = new Post
                {
                    UserId = userId,
                    Code = code,
                    MainImage = mainImageUrl,
                    Title = request.Title.Trim(),
                    Description = request.Description.Trim(),
                    Sumary = request.Sumary.Trim(),
                    SeoAlias = seoAlias,
                    CategoryId = request.CategoryId,
                    CreateAt = _now,
                    UpdateAt = _now,
                    IsDeleted = false,
                    PostTags = new List<PostTag>()
                };

                // 8. Sync Tags
                var tagIds = await GetOrCreateTagsAsync(request.Tags, ct);
                post.PostTags = tagIds.Select(tid => new PostTag { TagId = tid, Post = post }).ToList();
                _db.Posts.Add(post);
                await _db.SaveChangesAsync(ct);

                var revision = new PostRevision
                {
                    PostId = post.Id,
                    UserId = userId,
                    ContentSnapshot = BuildSnapshotJson(post, tagIds),
                    CreatedAt = _now
                };

                _db.PostRevisions.Add(revision);
                await _db.SaveChangesAsync(ct);

                // 10. Clear cache
                await RemoveCache();

                return new ApiSuccessResult<int>(post.Id, "Tạo bài viết thành công");
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<int>($"Lỗi khi tạo bài viết: {ex.Message}");
            }
        }

        private async Task<List<int>> GetOrCreateTagsAsync(List<string> tags, CancellationToken ct)
        {
            var normalized = tags
                .Select(StringHelper.NormalizeTagName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .Take(20) // Giới hạn tối đa 20 tags
                .ToList();

            if (!normalized.Any()) return new List<int>();

            var seoAliases = normalized.Select(StringHelper.GenerateSeoAlias).ToList();

            var existing = await _db.Tags
                .Where(t => seoAliases.Contains(t.SeoAlias))
                .ToListAsync(ct);

            var existingMap = existing.ToDictionary(x => x.SeoAlias, x => x);
            var result = new List<int>();

            foreach (var name in normalized)
            {
                var alias = StringHelper.GenerateSeoAlias(name);
                if (string.IsNullOrWhiteSpace(alias)) continue;

                if (existingMap.TryGetValue(alias, out var tag))
                {
                    result.Add(tag.Id);
                    continue;
                }

                // Tạo tag mới
                var uniqueAlias = await EnsureUniqueTagSeoAliasAsync(alias, ct);

                var newTag = new Tag
                {
                    Name = name,
                    SeoAlias = uniqueAlias,
                    PostTags = new List<PostTag>()
                };

                _db.Tags.Add(newTag);
                await _db.SaveChangesAsync(ct);

                existingMap[newTag.SeoAlias] = newTag;
                result.Add(newTag.Id);
            }

            return result;
        }
        private async Task<string> EnsureUniqueTagSeoAliasAsync(string baseAlias, CancellationToken ct, int counter = 1)
        {
            var alias = counter == 1 ? baseAlias : $"{baseAlias}-{counter}";

            var exists = await _db.Tags.AnyAsync(t => t.SeoAlias == alias, ct);

            return exists
                ? await EnsureUniqueTagSeoAliasAsync(baseAlias, ct, counter + 1)
                : alias;
        }
        private static string BuildSnapshotJson(Post post, List<int> tagIds)
        {
            var snapshot = new
            {
                post.Id,
                post.UserId,
                post.Code,
                post.MainImage,
                post.Title,
                post.Description,
                post.Sumary,
                post.SeoAlias,
                post.CategoryId,
                post.CreateAt,
                post.UpdateAt,
                TagIds = tagIds
            };

            return JsonSerializer.Serialize(snapshot);
        }
        private static string SanitizeFileName(string fileName)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);

            name = Regex.Replace(name, @"[^\w\-]", "-");
            name = Regex.Replace(name, @"-+", "-").Trim('-');

            return $"{name}{ext}".ToLowerInvariant();
        }
        private async Task SaveCache<T>(string key, T value)
        {
            await _redisService.SetValue(key, value, TimeSpan.FromDays(3));
        }
        private async Task RemoveCache()
        {
            var pattern = $"{KeyPrefix}:*";
            await _redisService.DeleteKeysByPatternAsync(pattern);
        }
        private async Task InvalidatePostCacheAsync(int postId)
        {
            var cacheKey = $"{KeyPrefix}:{postId}";
            await _redisService.RemoveKeyAsync(cacheKey);
        }
    }
}
