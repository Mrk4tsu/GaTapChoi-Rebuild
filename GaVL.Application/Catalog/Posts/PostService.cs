using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.Data.Entities;
using GaVL.DTO.APIResponse;
using GaVL.DTO.Paging;
using GaVL.DTO.Posts;
using GaVL.Utilities;
using Microsoft.EntityFrameworkCore;

namespace GaVL.Application.Catalog.Posts
{
    public interface IPostService
    {
        Task<ApiResult<int>> CreatePost(PostRequest request, Guid userId);
        Task<ApiResult<PagedResult<PostDTO>>> GetPosts(PagingRequest request);
    }
    public class PostService : IPostService
    {
        private readonly AppDbContext _db;
        private readonly IRedisService _redisService;
        private readonly IR2Service _r2Service;
        private readonly string KeyPrefix = "posts";
        public PostService(AppDbContext db, IR2Service r2Service, IRedisService redisService)
        {
            _db = db;
            _redisService = redisService;
            _r2Service = r2Service;
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
                AuthorName = x.User.Username,
                SeoAlias = x.SeoAlias,
                Thumbnail = x.MainImage,
                Title = x.Title
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
        public async Task<ApiResult<int>> CreatePost(PostRequest request, Guid userId)
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
    }
}
